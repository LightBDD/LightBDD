using System;
using LightBDD.Core.Notification;
using Xunit.Abstractions;

namespace LightBDD.Integration.XUnit2
{
    public class XUnitProgressNotifier : ParallelProgressNotifier
    {
        private readonly ITestOutputHelper _outputHelper;
        private static readonly ProgressManager SharedProgress = new ProgressManager();

        public XUnitProgressNotifier(ITestOutputHelper outputHelper) : base(SharedProgress)
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
