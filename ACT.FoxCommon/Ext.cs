using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Advanced_Combat_Tracker;

namespace ACT.FoxCommon
{
    public static class Ext
    {
        public static void AddControlSetting(this SettingsSerializer serializer, Control controlToSerialize)
        {
            serializer.AddControlSetting(controlToSerialize.Name, controlToSerialize);
        }

        public static string ToHexString(this Color c)
        {
            return "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
        }

        public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0) return min;
            else if (val.CompareTo(max) > 0) return max;
            else return val;
        }

        public static IEnumerable<R> SelectNotDefault<T, R>(this IEnumerable<T> source, Func<T, R> selector)
        {
            return source.Select(selector).Where(it => !EqualityComparer<R>.Default.Equals(it, default));
        }

        public static IEnumerable<R> SelectManyNotDefault<T, R>(this IEnumerable<T> source, Func<T, IEnumerable<R>> selector)
        {
            return source.SelectMany(selector).Where(it => !EqualityComparer<R>.Default.Equals(it, default));
        }
    }
}
