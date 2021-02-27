using System;
using LightBDD.Core.Notification;
using LightBDD.Framework.Notification;
using NUnit.Framework;

namespace LightBDD.NUnit3.Implementation
{
    internal class NUnit3ProgressNotifier
    {
        private static readonly DefaultProgressNotifier SummarizingProgressNotifier = new DefaultProgressNotifier(WriteOutput);

        [Obsolete]
        public static IFeatureProgressNotifier CreateFeatureProgressNotifier()
        {
            return new DelegatingFeatureProgressNotifier(ParallelProgressNotifierProvider.Default.CreateFeatureProgressNotifier(WriteImmediateProgress));
        }

        [Obsolete]
        public static IScenarioProgressNotifier CreateScenarioProgressNotifier()
        {
            return new DelegatingScenarioProgressNotifier(
                ParallelProgressNotifierProvider.Default.CreateScenarioProgressNotifier(WriteImmediateProgress),
                SummarizingProgressNotifier);
        }

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
                ParallelProgressNotifierProvider.Default.CreateProgressNotifier(WriteImmediateProgress),
                new DefaultProgressNotifier(WriteOutput)
            };
        }
    }
}
