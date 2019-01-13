using System;
using System.Linq;
using System.Text;

namespace LightBDD.Core.Formatting
{
    /// <summary>
    /// Extension methods for formatting TimeSpan values.
    /// </summary>
    public static class TimeFormatter
    {
        private static readonly Tuple<string, int, Func<TimeSpan, int>>[] Formatters = {
            new Tuple<string, int, Func<TimeSpan, int>>("d", 1, GetDays),
            new Tuple<string, int, Func<TimeSpan, int>>("h", 2, GetHours),
            new Tuple<string, int, Func<TimeSpan, int>>("m", 2, GetMinutes),
            new Tuple<string, int, Func<TimeSpan, int>>("s", 2, GetSeconds),
            new Tuple<string, int, Func<TimeSpan, int>>("ms", 3, GetMilliseconds)
        };

        /// <summary>
        /// This method returns up to 2 most meaningful time components of given time, to make it most readable.
        /// Maximal supported time component is 'day', while the minimal is 'millisecond'.
        /// TimeSpan value being less than 1ms but larger than 0 would be presented as &lt;1ms. 
        /// 
        /// Example values:
        /// <list type="bullet">
        /// <item><description>1d 12h</description></item>
        /// <item><description>5h</description></item>
        /// <item><description>5m 02s</description></item>
        /// <item><description>2s 527ms</description></item>
        /// <item><description>&lt;1ms</description></item>
        /// <item><description>0ms</description></item>
        /// </list>
        /// </summary>
        public static string FormatPretty(this TimeSpan ts)
        {
            if (ts.Ticks == 0)
                return "0ms";

            var sb = new StringBuilder();
            var position = FindFirst(ts);

            foreach (var fmt in Formatters.Skip(position).Take(2).Where(f => f.Item3(ts) > 0))
                FormatPretty(sb, ts, fmt);

            if (sb.Length == 0)
                sb.Append("<1ms");

            return sb.ToString();
        }

        private static int FindFirst(TimeSpan ts)
        {
            int i;
            for (i = 0; i < Formatters.Length; i++)
            {
                if (Formatters[i].Item3(ts) > 0)
                    break;
            }
            return i;
        }

        private static void FormatPretty(StringBuilder sb, TimeSpan ts, Tuple<string, int, Func<TimeSpan, int>> formatter)
        {
            var padding = formatter.Item2;
            if (sb.Length == 0)
                padding = 1;
            else
                sb.Append(" ");
            sb.Append(formatter.Item3(ts).ToString("D" + padding)).Append(formatter.Item1);
        }

        private static int GetMilliseconds(TimeSpan ts)
        {
            return ts.Milliseconds;
        }

        private static int GetSeconds(TimeSpan ts)
        {
            return ts.Seconds;
        }

        private static int GetMinutes(TimeSpan ts)
        {
            return ts.Minutes;
        }

        private static int GetHours(TimeSpan ts)
        {
            return ts.Hours;
        }

        private static int GetDays(TimeSpan ts)
        {
            return ts.Days;
        }
    }
}
