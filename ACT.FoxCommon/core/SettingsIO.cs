using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using ACT.FoxCommon.localization;
using Advanced_Combat_Tracker;

namespace ACT.FoxCommon.core
{
    /// <summary>
    /// Handles config file load, save and backup.
    /// </summary>
    public class SettingsIO
    {

        public delegate void SettingsWriteDelegate(XmlTextWriter writer);

        public delegate void SettingsReadDelegate(XmlTextReader reader);

        public SettingsWriteDelegate WriteSettings;
        public SettingsReadDelegate ReadSettings;

        private readonly string _settingsFile;

        private string _backupFile => $"{_settingsFile}.backup";

        public SettingsIO(string pluginName)
        {
            _settingsFile = Path.Combine(ActGlobals.oFormActMain.AppDataFolder.FullName,
                $"Config\\{pluginName}.config.xml");
        }

        public void Load(bool isBackup = false)
        {
            try
            {
                LoadFrom(_settingsFile);
            }
            catch (Exception e)
            {
                var dialog = new SettingsRepairDialog();
                var messageBuilder = new StringBuilder();

                messageBuilder.AppendLine(isBackup
                    ? LocalizationBase.GetString("settingsErrorBackup")
                    : LocalizationBase.GetString("settingsError"));

                messageBuilder.AppendLine();

                if (File.Exists(_backupFile) && !isBackup)
                {
                    messageBuilder.AppendLine(LocalizationBase.GetString("settingsErrorHintWithBackup"));
                    dialog.HasBackup = true;
                }
                else
                {
                    messageBuilder.AppendLine(LocalizationBase.GetString("settingsErrorHintNoBackup"));
                    dialog.HasBackup = false;
                }

                messageBuilder.AppendLine();
                messageBuilder.Append($"Unable to read config file: {_settingsFile}\n\nCaused by:\n{e}");

                dialog.Message = messageBuilder.ToString();
                AskForAction(dialog, e);
            }
        }

        private void LoadFrom(string filePath)
        {
            if (File.Exists(filePath))
            {
                var xmlDocument = new XmlDocument();
                using (var reader =
                    new StreamReader(new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite),
                        Encoding.UTF8))
                {
                    xmlDocument.Load(reader);
                }

                var writer = new StringWriter();
                using (var xmlTextWriter = CreateXmlWriter(writer))
                {
                    xmlDocument.WriteTo(xmlTextWriter);
                    xmlTextWriter.Flush();
                }
                var content = writer.ToString();

                using (var reader = new XmlTextReader(new StringReader(content)))
                {
                    while (reader.Read())
                    {
                        if (reader.NodeType != XmlNodeType.Element)
                        {
                            continue;
                        }
                        ReadSettings?.Invoke(reader);
                    }
                }
            }
        }

        private void AskForAction(SettingsRepairDialog dialog, Exception e)
        {
            dialog.ShowDialog();
            var action = dialog.RepairAction;

            switch (action)
            {
                case SettingsRepairDialog.SettingsRepairAction.Ignore:
                    throw new SettingsNotLoadException("Settings error ignored", e);
                case SettingsRepairDialog.SettingsRepairAction.Reset:
                    Save(false);
                    break;
                case SettingsRepairDialog.SettingsRepairAction.Restore:
                    // Delete existing config
                    if (File.Exists(_settingsFile))
                    {
                        File.Delete(_settingsFile);
                    }

                    // Restore config
                    File.Move(_backupFile, _settingsFile);
                    Load(true);
                    break;
            }
        }

        private XmlTextWriter CreateXmlWriter(TextWriter writer)
        {
            var xmlTextWriter = new XmlTextWriter(writer);
            xmlTextWriter.Formatting = Formatting.Indented;
            xmlTextWriter.Indentation = 1;
            xmlTextWriter.IndentChar = '\t';

            return xmlTextWriter;
        }

        public void Save(bool createBackup = true)
        {
            // Generate content of the new config
            var writer = new StringWriter();
            using (var xmlTextWriter = CreateXmlWriter(writer))
            {
                xmlTextWriter.WriteStartDocument(true);
                xmlTextWriter.WriteStartElement("Config");

                WriteSettings?.Invoke(xmlTextWriter);

                xmlTextWriter.WriteEndElement();
                xmlTextWriter.WriteEndDocument();
                xmlTextWriter.Flush();
            }
            var content = writer.ToString();

            // Delete existing backup
            if (File.Exists(_backupFile))
            {
                File.Delete(_backupFile);
            }

            // Backup existing config
            if (createBackup && File.Exists(_settingsFile))
            {
                File.Move(_settingsFile, _backupFile);
            }

            // Save current config
            File.WriteAllText(_settingsFile, content, Encoding.UTF8);
        }

    }

    public class SettingsNotLoadException : Exception
    {
        public SettingsNotLoadException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    public static class SettingsIOExt
    {
        public static void Serialize<T>(this XmlTextWriter writer, T obj) where T : class
        {
            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            var serializer = new XmlSerializer(typeof(T));
            serializer.Serialize(writer, obj, ns);
        }

        public static T Deserialize<T>(this XmlTextReader reader) where T : class
        {
            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            var serializer = new XmlSerializer(typeof(T));
            return serializer.Deserialize(reader) as T;
        }
    }
}
