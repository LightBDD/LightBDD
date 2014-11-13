using System;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace LightBDD.Formatters
{
    /// <summary>
    /// Extension methods for formatting TimeSpan values.
    /// </summary>
    [DebuggerStepThrough]
    public static class TimeFormatter
    {
        private static readonly Tuple<string, int, Func<TimeSpan, int>>[] _formatters = new[]
        {
            new Tuple<string, int, Func<TimeSpan, int>>("d", 1, ts => ts.Days),
            new Tuple<string, int, Func<TimeSpan, int>>("h", 2, ts => ts.Hours),
            new Tuple<string, int, Func<TimeSpan, int>>("m", 2, ts => ts.Minutes),
            new Tuple<string, int, Func<TimeSpan, int>>("s", 2, ts => ts.Seconds),
            new Tuple<string, int, Func<TimeSpan, int>>("ms", 3, ts => ts.Milliseconds)
        };
        /// <summary>
        /// Formats given value if provided or returns empty string.
        /// 
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
        public static string FormatPretty(this TimeSpan? ts)
        {
            return ts.HasValue ? ts.Value.FormatPretty() : string.Empty;
        }
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

            foreach (var fmt in _formatters.Skip(position).Take(2).Where(f => f.Item3(ts) > 0))
                FormatPretty(sb, ts, fmt);

            if (sb.Length == 0)
                sb.Append("<1ms");

            return sb.ToString();
        }

        private static int FindFirst(TimeSpan ts)
        {
            int i;
            for (i = 0; i < _formatters.Length; i++)
            {
                if (_formatters[i].Item3(ts) > 0)
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
    }
}
