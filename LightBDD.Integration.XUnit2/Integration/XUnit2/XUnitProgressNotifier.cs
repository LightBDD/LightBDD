using System;
using LightBDD.Core.Notification;
using Xunit.Abstractions;

namespace LightBDD.Integration.XUnit2
{
    public class XUnitProgressNotifier : ParallelProgressNotifier
    {
        private readonly ITestOutputHelper _outputHelper;

        public XUnitProgressNotifier(ITestOutputHelper outputHelper, ProgressManager progressManager)
            : base(progressManager)
        {
            _outputHelper = outputHelper;
        }

        protected override void Notify(string message)
        {
            _outputHelper.WriteLine(message);
            Console.WriteLine(message);
        }
    }
}
