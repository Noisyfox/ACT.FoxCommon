using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ACT.FoxCommon.shortcut;
using ACT.FoxCommon.update;

namespace ACT.FoxCommon.core
{
    public abstract class MainControllerBase
    {
        public event Action SettingsLoaded;

        public void NotifySettingsLoaded()
        {
            SettingsLoaded?.Invoke();
        }

        public delegate void OnLogMessageAppendDelegate(bool fromView, string log);

        public event OnLogMessageAppendDelegate LogMessageAppend;

        public void NotifyLogMessageAppend(bool fromView, string log)
        {
            LogMessageAppend?.Invoke(fromView, log);
        }

        public delegate void OnUpdateCheckingStarted(bool fromView);

        public event OnUpdateCheckingStarted UpdateCheckingStarted;

        public void NotifyUpdateCheckingStarted(bool fromView)
        {
            UpdateCheckingStarted?.Invoke(fromView);
        }

        public delegate void OnNewVersionIgnored(bool fromView, string ignoredVersion);

        public event OnNewVersionIgnored NewVersionIgnored;

        public void NotifyNewVersionIgnored(bool fromView, string ignoredVersion)
        {
            NewVersionIgnored?.Invoke(fromView, ignoredVersion);
        }

        public delegate void OnVersionChecked(bool fromView, VersionInfo versionInfo, bool forceNotify);

        public event OnVersionChecked VersionChecked;

        public void NotifyVersionChecked(bool fromView, VersionInfo versionInfo, bool forceNotify)
        {
            VersionChecked?.Invoke(fromView, versionInfo, forceNotify);
        }


        public delegate void OnActivatedProcessPathChangedDelegate(bool fromView, ProcessInfo process);

        public event OnActivatedProcessPathChangedDelegate ActivatedProcessPathChanged;

        public void NotifyActivatedProcessPathChanged(bool fromView, ProcessInfo process)
        {
            ActivatedProcessPathChanged?.Invoke(fromView, process);
        }

        public delegate void OnGameProcessUpdated(bool fromView, HashSet<ProcessInfo> processes);

        public event OnGameProcessUpdated GameProcessUpdated;

        public void NotifyGameProcessUpdated(bool fromView, HashSet<ProcessInfo> processes)
        {
            GameProcessUpdated?.Invoke(fromView, processes);
        }

        public delegate void OnShortcutChanged(bool fromView, PluginShortcut shortcut, Keys key);

        public event OnShortcutChanged ShortcutChanged;

        public void NotifyShortcutChanged(bool fromView, PluginShortcut shortcut, Keys key)
        {
            ShortcutChanged?.Invoke(fromView, shortcut, key);
        }

        public delegate void OnShortcutRegister(bool fromView, PluginShortcut shortcut, bool isRegister,
            bool success);

        public event OnShortcutRegister ShortcutRegister;


        public void NotifyShortcutRegister(bool fromView, PluginShortcut shortcut, bool isRegister,
            bool success)
        {
            ShortcutRegister?.Invoke(fromView, shortcut, isRegister, success);
        }

        public delegate void OnShortcutFired(bool fromView, PluginShortcut shortcut);

        public event OnShortcutFired ShortcutFired;

        public void NotifyShortcutFired(bool fromView, PluginShortcut shortcut)
        {
            ShortcutFired?.Invoke(fromView, shortcut);
        }
    }
}
