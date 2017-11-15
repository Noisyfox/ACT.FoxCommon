using System;
using ACT.FoxCommon.core;

namespace ACT.FoxCommon
{
    public class WindowsMessagePump<TMainController, TPlugin> : IPluginComponentBase<TMainController, TPlugin>, IDisposable
        where TPlugin : PluginBase<TMainController>
        where TMainController : MainControllerBase
    {
        private TPlugin _plugin;
        private TMainController _controller;
        
        private Win32APIUtils.WinEventDelegate _hookPtrDele;
        private IntPtr _hookPtrForeground = IntPtr.Zero;
        private string _lastActivatedProcessPath = null;
        private uint _lastActivatedProcessPid = 0;

        public void AttachToAct(TPlugin plugin)
        {
            _plugin = plugin;
            _controller = plugin.Controller;
        }

        public void PostAttachToAct(TPlugin plugin)
        {
            _hookPtrDele = WinEventProc;
            _hookPtrForeground = Win32APIUtils.SetWinEventHook(Win32APIUtils.EVENT_SYSTEM_FOREGROUND, Win32APIUtils.EVENT_SYSTEM_MINIMIZEEND, IntPtr.Zero, _hookPtrDele, 0, 0, Win32APIUtils.WINEVENT_OUTOFCONTEXT);
            _hookPtrDele(IntPtr.Zero, Win32APIUtils.EVENT_SYSTEM_FOREGROUND, IntPtr.Zero, 0, 0, 0, 0);
        }

        public void Dispose()
        {
            Win32APIUtils.UnhookWinEvent(_hookPtrForeground);
            _hookPtrForeground = IntPtr.Zero;
            _hookPtrDele = null;
            _plugin = null;
        }

//        [DllImport("user32.dll")]
//        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);
//
//        private string GetActiveWindowTitle()
//        {
//            const int nChars = 256;
//            IntPtr handle = IntPtr.Zero;
//            StringBuilder Buff = new StringBuilder(nChars);
//            handle = Win32APIUtils.GetForegroundWindow();
//
//            if (GetWindowText(handle, Buff, nChars) > 0)
//            {
//                return Buff.ToString();
//            }
//            return null;
//        }

        private void WinEventProc(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            if (eventType != Win32APIUtils.EVENT_SYSTEM_FOREGROUND && eventType != Win32APIUtils.EVENT_SYSTEM_MENUPOPUPSTART &&
                eventType != Win32APIUtils.EVENT_SYSTEM_MENUPOPUPEND && eventType != Win32APIUtils.EVENT_SYSTEM_MINIMIZEEND)
            {
                return;
            }
            Tuple<string, uint> pathPid = null;
            if (hwnd != IntPtr.Zero)
            {
                pathPid = Win32APIUtils.GetProcessPathByWindow(hwnd);
            }
            if (pathPid == null)
            {
                pathPid = Win32APIUtils.GetForgegroundProcessPath();
            }
            if (pathPid == null)
            {
                return;
            }

            var path = pathPid.Item1;
            var pid = pathPid.Item2;

            if (_lastActivatedProcessPath != path || _lastActivatedProcessPid != pid)
            {
                _lastActivatedProcessPath = path;
                _lastActivatedProcessPid = pid;
                _controller.NotifyActivatedProcessPathChanged(false, path, pid);
            }
            //            Log.Text += GetActiveWindowTitle() + "\r\n";
//            _controller.NotifyLogMessageAppend(false, path + "\r\n");

        }
    }
}
