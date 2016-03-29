using LightBDD.Core.Notification;
using NUnit.Framework;

namespace LightBDD.Integration.NUnit3
{
    public class NUnit3ProgressNotifier : ParallelProgressNotifier
    {
        protected override void Notify(string message)
        {
            TestContext.WriteLine(message);
        }

        public NUnit3ProgressNotifier(ProgressManager progressManager)
            : base(progressManager)
        {
        }
    }
}
