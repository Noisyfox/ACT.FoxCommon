using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Input;
using ACT.FoxCommon;
using ACT.FoxCommon.core;
using ACT.FoxCommon.logging;
using ACT.FoxCommon.shortcut;
using GlobalHotKey;
using KeyEventArgs = System.Windows.Forms.KeyEventArgs;

namespace ACT.FFXIVPing
{
    public class ShortkeyManager<TMainController, TPlugin> : IPluginComponentBase<TMainController, TPlugin>, IDisposable
        where TPlugin : PluginBase<TMainController>
        where TMainController : MainControllerBase
    {
        private readonly HotKeyManager _hotKeyManager = new HotKeyManager();

        private TMainController _controller;

        private readonly Dictionary<PluginShortcut, HotKey> _registeredHotKeys = new Dictionary<PluginShortcut, HotKey>();

        public void AttachToAct(TPlugin plugin)
        {
            _controller = plugin.Controller;
            _controller.ShortcutChanged += ControllerOnShortcutChanged;

            _hotKeyManager.KeyPressed += HotKeyManagerOnKeyPressed;
        }

        public void PostAttachToAct(TPlugin plugin)
        {
        }

        public void Dispose()
        {
            _hotKeyManager.Dispose();
            _registeredHotKeys.Clear();
        }

        private void ControllerOnShortcutChanged(bool fromView, PluginShortcut shortcut, Keys key)
        {
            if (_registeredHotKeys.ContainsKey(shortcut))
            {
                _hotKeyManager.Unregister(_registeredHotKeys[shortcut]);
                _registeredHotKeys.Remove(shortcut);
                _controller.NotifyShortcutRegister(fromView, shortcut, false, true);
            }

            if (key != Keys.None)
            {
                try
                {
                    var hotKey = KeysToHotKey(key);
                    _hotKeyManager.Register(hotKey);
                    _registeredHotKeys[shortcut] = hotKey;
                    _controller.NotifyShortcutRegister(fromView, shortcut, true, true);
                }
                catch (Exception e)
                {
                    _controller.NotifyShortcutRegister(fromView, shortcut, true, false);
                    Logger.Error("Shortkey register failed", e);
                }
            }
        }

        private void HotKeyManagerOnKeyPressed(object sender, KeyPressedEventArgs e)
        {
            var hotkey = e.HotKey;

            foreach (var registeredHotKey in _registeredHotKeys)
            {
                if (Equals(hotkey, registeredHotKey.Value))
                {
                    _controller.NotifyShortcutFired(true, registeredHotKey.Key);
                    break;
                }
            }
        }


        #region Helper Funcs

        private static HotKey KeysToHotKey(Keys k)
        {
            var e = new KeyEventArgs(k);
            var mk = ModifierKeys.None;
            if (e.Alt)
            {
                mk |= ModifierKeys.Alt;
            }
            if (e.Control)
            {
                mk |= ModifierKeys.Control;
            }
            if (e.Shift)
            {
                mk |= ModifierKeys.Shift;
            }

            return new HotKey(KeyInterop.KeyFromVirtualKey(e.KeyValue), mk);
        }

        private static Keys HotkeyToKeys(HotKey hotKey)
        {
            var k = Keys.None;
            var m = hotKey.Modifiers;
            if (m.HasFlag(ModifierKeys.Alt))
            {
                k |= Keys.Alt;
            }
            if (m.HasFlag(ModifierKeys.Control))
            {
                k |= Keys.Control;
            }
            if (m.HasFlag(ModifierKeys.Shift))
            {
                k |= Keys.Shift;
            }

            k |= (Keys) KeyInterop.VirtualKeyFromKey(hotKey.Key);

            return k;
        }

        #endregion
    }
}
