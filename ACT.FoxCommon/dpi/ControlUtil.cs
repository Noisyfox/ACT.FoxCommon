using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ACT.FoxCommon.dpi
{
    /// <summary>
    /// https://github.com/gitextensions/gitextensions/blob/4695bb371563922d43cccfbbc3b79d6b704096d2/GitExtUtils/GitUI/ControlUtil.cs
    /// </summary>
    public static class ControlUtil
    {
        /// <summary>
        /// Enumerates all descendant controls.
        /// </summary>
        public static IEnumerable<Control> FindDescendants(this Control control,
            Func<Control, bool> skip = null)
        {
            var queue = new Queue<Control>();

            foreach (Control child in control.Controls)
            {
                if (skip?.Invoke(control) == true)
                {
                    continue;
                }

                queue.Enqueue(child);
            }

            while (queue.Count != 0)
            {
                var c = queue.Dequeue();

                yield return c;

                foreach (Control child in c.Controls)
                {
                    if (skip?.Invoke(child) == true)
                    {
                        continue;
                    }

                    queue.Enqueue(child);
                }
            }
        }
    }
}
