using System;
using System.Diagnostics;
using LightBDD.Core.Notification;
using Xunit.Abstractions;

namespace LightBDD.Integration.XUnit2
{
    public class XUnit2ProgressNotifier : ParallelProgressNotifier
    {
        private readonly ITestOutputHelper _outputHelper;

        public XUnit2ProgressNotifier(ITestOutputHelper outputHelper, ProgressManager progressManager)
            : base(progressManager)
        {
            _outputHelper = outputHelper;
        }

        protected override void Notify(string message)
        {
            WriteToOutputHelper(message);
            Console.WriteLine(message);
        }

        private void WriteToOutputHelper(string message)
        {
            try
            {
                _outputHelper.WriteLine(message);
            }
            catch (Exception e)
            {
                Trace.TraceWarning($"{nameof(XUnit2ProgressNotifier)} is not able to log message.\nMessage: {message};\nReason: {e}");
            }
        }
    }
}
