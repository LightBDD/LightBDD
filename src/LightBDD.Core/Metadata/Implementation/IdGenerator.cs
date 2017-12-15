using System.Globalization;
using System.Threading;

namespace LightBDD.Core.Metadata.Implementation
{
    internal static class IdGenerator
    {
        private static int _last;

        public static string Generate()
        {
            return Interlocked.Increment(ref _last).ToString(CultureInfo.InvariantCulture);
        }
    }
}
