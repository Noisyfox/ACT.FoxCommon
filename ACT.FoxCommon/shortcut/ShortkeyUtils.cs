using System;
using System.Windows.Forms;

namespace ACT.FoxCommon.shortcut
{
    public class ShortkeyUtils
    {
        public static string KeyToString(Keys key)
        {
            try
            {
                return new KeysConverter().ConvertToInvariantString(key);
            }
            catch (Exception)
            {
                return "None";
            }
        }

        public static Keys StringToKey(string str)
        {
            try
            {
                return (Keys)new KeysConverter().ConvertFromInvariantString(str);
            }
            catch (Exception)
            {
                return Keys.None;
            }
        }
    }
}
