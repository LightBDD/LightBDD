using System;
using System.Threading.Tasks;

namespace Example.Domain.Helpers
{
    public static class LongRunningOperationSimulator
    {
        private static readonly Random Rand = new Random();

        public static void Simulate()
        {
            SimulateAsync().Wait();
        }

        public static Task SimulateAsync()
        {
            return Task.Delay(TimeSpan.FromMilliseconds(Rand.Next(1000, 2000)));
        }
    }
}