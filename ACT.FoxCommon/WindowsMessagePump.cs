using System;
using ACT.FoxCommon.core;
using ACT.FoxCommon.logging;

namespace ACT.FoxCommon
{
    public class WindowsMessagePumpBase<TMainController, TPlugin> : IPluginComponentBase<TMainController, TPlugin>, IDisposable
        where TPlugin : PluginBase<TMainController>
        where TMainController : MainControllerBase
    {
        protected TPlugin Plugin { get; private set; }
        protected TMainController Controller { get; private set; }

//        private Win32APIUtils.WinEventDelegate _hookPtrDele;
//        private IntPtr _hookPtrForeground = IntPtr.Zero;
        private ProcessInfo _lastActivatedProcess = null;
        private readonly ForgeProcessDetector _forgeProcessDetector = new ForgeProcessDetector();

        public virtual void AttachToAct(TPlugin plugin)
        {
            Plugin = plugin;
            Controller = plugin.Controller;
        }

        public virtual void PostAttachToAct(TPlugin plugin)
        {
//            _hookPtrDele = WinEventProc;
//            _hookPtrForeground = Win32APIUtils.SetWinEventHook(Win32APIUtils.EVENT_SYSTEM_FOREGROUND, Win32APIUtils.EVENT_SYSTEM_MINIMIZEEND, IntPtr.Zero, _hookPtrDele, 0, 0, Win32APIUtils.WINEVENT_OUTOFCONTEXT);
//            _hookPtrDele(IntPtr.Zero, Win32APIUtils.EVENT_SYSTEM_FOREGROUND, IntPtr.Zero, 0, 0, 0, 0);
            _forgeProcessDetector.StartWorkingThread(this);
        }

        public virtual void Dispose()
        {
            _forgeProcessDetector.StopWorkingThread();
//            Win32APIUtils.UnhookWinEvent(_hookPtrForeground);
//            _hookPtrForeground = IntPtr.Zero;
//            _hookPtrDele = null;
            Plugin = null;
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
            ProcessInfo process = null;
            if (hwnd != IntPtr.Zero)
            {
                process = Win32APIUtils.GetProcessPathByWindow(hwnd);
            }
            if (process == null)
            {
                process = Win32APIUtils.GetForegroundProcessPath();
            }
            if (process == null)
            {
                return;
            }

            if (_lastActivatedProcess != process)
            {
                _lastActivatedProcess = process;
                Controller.NotifyActivatedProcessPathChanged(false, process);
                Logger.Debug($"Activated process changed: {process}");
            }
            //            Log.Text += GetActiveWindowTitle() + "\r\n";
//            _controller.NotifyLogMessageAppend(false, path + "\r\n");

        }

        private class ForgeProcessDetector : BaseThreading<WindowsMessagePumpBase<TMainController, TPlugin>>
        {
            protected override void DoWork(WindowsMessagePumpBase<TMainController, TPlugin> context)
            {
                while (!WorkingThreadStopping)
                {
                    context.WinEventProc(IntPtr.Zero, Win32APIUtils.EVENT_SYSTEM_FOREGROUND, IntPtr.Zero, 0, 0, 0, 0);

                    SafeSleep(1000);
                }
            }
        }
    }
}
