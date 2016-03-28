using LightBDD.Core.Notification;
using NUnit.Framework;

namespace LightBDD.Integration.NUnit3
{
    public class NUnit3ProgressNotifier : ParallelProgressNotifier
    {
        private static readonly ProgressManager SharedProgress = new ProgressManager();
        protected override void Notify(string message)
        {
            TestContext.WriteLine(message);
        }

        public NUnit3ProgressNotifier() : base(SharedProgress) { }
    }
}
