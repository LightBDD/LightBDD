using System;
using LightBDD.Core.Notification;

namespace LightBDD.Integration.MsTest
{
    public class MsTestProgressNotifier : ParallelProgressNotifier
    {
        private static readonly ProgressManager SharedProgress = new ProgressManager();

        protected override void Notify(string message)
        {
            Console.WriteLine(message);
        }

        public MsTestProgressNotifier() : base(SharedProgress)
        {
        }
    }
}
