using System;
using System.Threading.Tasks;

namespace LightBDD.Example.Helpers
{
    public static class LongRunningOperationSimulator
    {
        private static readonly Random Rand = new Random();

        public static void Simulate()
        {
            Task.Delay(TimeSpan.FromMilliseconds(Rand.Next(100, 200))).Wait();
        }
    }
}