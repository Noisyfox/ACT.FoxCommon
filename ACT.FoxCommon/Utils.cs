using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using Advanced_Combat_Tracker;
using FontFamily = System.Windows.Media.FontFamily;

namespace ACT.FoxCommon
{
    public static class Utils
    {

        public static bool IsGameExePath(string path)
        {
            var exe = Path.GetFileName(path);
            return exe == "ffxiv.exe" || exe == "ffxiv_dx11.exe";
        }

        public static bool IsActExePath(string path)
        {
            return path == Process.GetCurrentProcess().MainModule.FileName;
        }

        public static Typeface NewTypeFaceFromFont(Font f)
        {
            var ff = new FontFamily(f.Name);

            var typeface = new Typeface(ff,
                f.Style.HasFlag(System.Drawing.FontStyle.Italic) ? FontStyles.Italic : FontStyles.Normal,
                f.Style.HasFlag(System.Drawing.FontStyle.Bold) ? FontWeights.Bold : FontWeights.Normal,
                FontStretches.Normal);

            return typeface;
        }

        public static bool IsGameExeProcess(uint pid)
        {
            return IsGameExePath(Process.GetProcessById((int) pid).MainModule.FileName);
        }

        public static bool IsGameExeProcess(Process p)
        {
            return IsGameExePath(p.MainModule.FileName);
        }

        public static long TimestampMillisFromDateTime(DateTime date)
        {
            var unixTimestamp = date.Ticks - new DateTime(1970, 1, 1).Ticks;
            unixTimestamp /= TimeSpan.TicksPerMillisecond;
            return unixTimestamp;
        }

        public static void SetValue(this TrackBar bar, int value, int defaultValue)
        {
            try
            {
                bar.Value = value;
            }
            catch (Exception ex)
            {
                bar.Value = defaultValue;
            }
        }

        public static ActPluginData FindFFXIVPlugin() => ActGlobals.oFormActMain.ActPlugins
            .FirstOrDefault(data =>
            {
                if (data.pluginFile.Name.ToUpper() != "FFXIV_ACT_Plugin.dll".ToUpper())
                {
                    return false;
                }

                dynamic plugin = data.pluginObj;

                return plugin?.PluginStarted ?? false;
            });

        /// <summary>
        /// Whether the plugin is currently running in CafeACT (a modified version of the original ACT).
        /// </summary>
        public static bool IsCafeACT => System.Reflection.Assembly.GetEntryAssembly()?.GetName().Name == "CafeACT";

        public static bool IsLoopback(this IPAddress address)
        {
            return Equals(address, IPAddress.Loopback) || Equals(address, IPAddress.IPv6Loopback);
        }
    }
}
