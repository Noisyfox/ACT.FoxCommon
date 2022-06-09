using System;
using System.Windows.Forms;

namespace ACT.FoxCommon
{
    public static class SafeThreadInvoker
    {
        public static object SafeInvoke(this Control control, Delegate method)
        {
            if (control.InvokeRequired)
            {
                var asyncResult = control.BeginInvoke(method);

                return SafeWait(control, asyncResult);
            }
            else
            {
                return method.DynamicInvoke();
            }
        }

        public static object SafeInvoke(this Control control, Delegate method, object[] args)
        {
            if (control.InvokeRequired)
            {
                var asyncResult = control.BeginInvoke(method, args);

                return SafeWait(control, asyncResult);
            }
            else
            {
                return method.DynamicInvoke(args);
            }
        }

        private static object SafeWait(Control control, IAsyncResult result)
        {
            if (result.CompletedSynchronously)
            {
                return control.EndInvoke(result);
            }

            for (var i = 0; i < 50; i++)
            {
                if (control.IsDisposed || control.Disposing || !control.IsHandleCreated)
                {
                    break;
                }

                result.AsyncWaitHandle.WaitOne(100);
                if (result.IsCompleted)
                {
                    return control.EndInvoke(result);
                }
            }

            return null;
        }

        public static void AppendDateTimeLine(this RichTextBox target, string text)
        {
            if (target.InvokeRequired)
            {
                target.SafeInvoke(new Action(delegate
                {
                    target.AppendDateTimeLine(text);
                }));
            }
            else
            {
                if (target.TextLength > 0)
                {
                    target.AppendText($"\n[{DateTime.Now.ToLongTimeString()}] {text}");
                }
                else
                {
                    target.AppendText($"[{DateTime.Now.ToLongTimeString()}] {text}");
                }
                target.ScrollToCaret();
            }
        }
    }
}
