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

        public static void TranslateControls(Control control)
        {
            var setterList = new List<Action>();

            setterList.Add(()=>UpdateTextBasedOnName(control));

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

        public static string GetString(string name)
        {
            return _resourceManager.GetString(name, _culture);
        }
    }
}
