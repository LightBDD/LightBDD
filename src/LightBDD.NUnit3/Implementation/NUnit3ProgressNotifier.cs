using System;
using LightBDD.Core.Notification;
using LightBDD.Framework.Notification;
using NUnit.Framework;

namespace LightBDD.NUnit3.Implementation
{
    internal class NUnit3ProgressNotifier
    {
        private static readonly IProgressNotifier SummarizingProgressNotifier = new DefaultProgressNotifier(WriteOutput);
        private static readonly IProgressNotifier ImmediateProgressNotifier = ParallelProgressNotifierProvider.Default.CreateProgressNotifier(WriteImmediateProgress);

        private static void WriteOutput(string text)
        {
            TestContext.Out.WriteLine(text);
        }

        private static void WriteImmediateProgress(string text)
        {
            TestContext.Progress.WriteLine(text);
        }

        public static IProgressNotifier[] CreateProgressNotifiers()
        {
            return new[]
            {
                ImmediateProgressNotifier, SummarizingProgressNotifier
            };
        }
    }
}
