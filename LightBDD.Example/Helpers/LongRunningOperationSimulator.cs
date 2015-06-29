using System;
using System.Threading;

namespace LightBDD.Example.Helpers
{
    public static class LongRunningOperationSimulator
    {
        private static readonly Random Rand = new Random();

        public static void Simulate()
        {
            Thread.Sleep(TimeSpan.FromMilliseconds(Rand.Next(100, 200)));
        }
    }
}