using System;

namespace ACT.FoxCommon.logging
{
    public static class Logger
    {
        public static bool IsDebugLevelEnabled { get; set; }

        public static Action<string> OnLogging = null;

        public static void Debug(string msg, Exception ex = null)
        {
            if (!IsDebugLevelEnabled)
            {
                return;
            }

            LogInternal("Debug", msg, ex);
        }

        public static void Info(string msg, Exception ex = null)
        {
            LogInternal("Info", msg, ex);
        }

        public static void Warn(string msg, Exception ex = null)
        {
            LogInternal("Warn", msg, ex);
        }

        public static void Error(string msg, Exception ex = null)
        {
            LogInternal("Error", msg, ex);
        }

        private static void LogInternal(string level, string msg, Exception ex)
        {
            var outMsg = $"[{level}] {msg}";
            if (ex != null)
            {
                outMsg = $"{outMsg}\n{ex}";
            }
            System.Diagnostics.Debug.WriteLine(outMsg);
            OnLogging?.Invoke(outMsg);
        }
    }
}
