using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Windows.Forms;

namespace ACT.FoxCommon.localization
{
    public static class LocalizationBase
    {
        private static ResourceManager _resourceManager;
        public static LanguageDef[] SupportedLanguages { get; private set; }

        private static string _defaultLanguage;

        private static CultureInfo _culture;

        public static void InitLocalization(ResourceManager resourceManager, LanguageDef[] supportedLanguages,
            string defaultLanguage)
        {
            _resourceManager = resourceManager;
            SupportedLanguages = supportedLanguages;
            _defaultLanguage = defaultLanguage;
        }

        public static void ConfigLocalization(string code)
        {
            _culture = CultureInfo.GetCultureInfo(code);
        }


        public static LanguageDef GetLanguage(string code)
        {
            return SupportedLanguages.FirstOrDefault(it => it.LangCode == code) ?? GetLanguage(_defaultLanguage);
        }

        private static void UpdateTextBasedOnName(Control control)
        {
            var t = GetString(control.Name);
            if (t != null)
            {
                control.Text = t;
            }
        }

        private static void UpdateComboBoxValues(Control control)
        {
            if (control is ComboBox combo)
            {
                var items = combo.Items.Cast<object>().ToList();
                if (items.All(obj => obj is string))
                {
                    var translated = items.Cast<string>().Select(key => GetString(control.Name + key) ?? key);

                    combo.Items.Clear();
                    combo.Items.AddRange(translated.ToArray());
                }
            }
        }

        public static void TranslateControls(Control control)
        {
            var setterList = new List<Action>();

            setterList.Add(()=>UpdateTextBasedOnName(control));
            setterList.Add(()=>UpdateComboBoxValues(control));

            foreach (Control child in control.Controls.AsParallel())
            {
                TranslateControls(child);
            }

            foreach (var action in setterList.AsParallel())
            {
                if (control.InvokeRequired)
                {
                    control.SafeInvoke((MethodInvoker)delegate { action(); });
                }
                else
                {
                    action();
                }
            }
        }

        public static string GetString(string name, string defaultValue = null)
        {
            return _resourceManager.GetString(name, _culture) ?? defaultValue;
        }
    }
}
