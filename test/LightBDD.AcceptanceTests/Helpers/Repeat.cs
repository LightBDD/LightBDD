using System;
using System.Diagnostics;
using System.Threading;

namespace LightBDD.AcceptanceTests.Helpers
{
    internal static class Repeat
    {
        private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(3);

        public static void Until(Func<bool> predicate, string errorMessage)
        {
            Until(predicate, () => errorMessage);
        }

        public static void Until(Func<bool> predicate, Func<string> errorMessageFn)
        {
            var watch = Stopwatch.StartNew();
            do
            {
                if (predicate())
                    return;
                Thread.Sleep(50);
            } while (watch.Elapsed < Timeout);
            throw new TimeoutException(errorMessageFn());
        }
    }
}