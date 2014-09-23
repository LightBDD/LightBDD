using System;
using System.Linq;
using System.Text;

namespace LightBDD.Formatters
{
    internal static class TimeFormatter
    {
        private static readonly Tuple<string, int, Func<TimeSpan, int>>[] _formatters = new[]
        {
            new Tuple<string, int, Func<TimeSpan, int>>("d", 1, ts => ts.Days),
            new Tuple<string, int, Func<TimeSpan, int>>("h", 2, ts => ts.Hours),
            new Tuple<string, int, Func<TimeSpan, int>>("m", 2, ts => ts.Minutes),
            new Tuple<string, int, Func<TimeSpan, int>>("s", 2, ts => ts.Seconds),
            new Tuple<string, int, Func<TimeSpan, int>>("ms", 3, ts => ts.Milliseconds)
        };
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
