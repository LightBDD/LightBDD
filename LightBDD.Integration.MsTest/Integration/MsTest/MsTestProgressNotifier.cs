using System;
using LightBDD.Core.Notification;

namespace LightBDD.Integration.MsTest
{
    public class MsTestProgressNotifier : ParallelProgressNotifier
    {
        protected override void Notify(string message)
        {
            Console.WriteLine(message);
        }

        public MsTestProgressNotifier(ProgressManager progressManager)
            : base(progressManager)
        {
        }
    }
}
