using System;
using LightBDD.Core.Notification;
using LightBDD.Framework.Notification;

namespace LightBDD.TUnit.Implementation
{
    internal class TUnitProgressNotifier
    {
        private static readonly DefaultProgressNotifier SummarizingProgressNotifier = new(WriteOutput);

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
            TestContext.Current?.OutputWriter.WriteLine(text);
        }

        private static void WriteImmediateProgress(string text)
        {
            TestContext.Current?.OutputWriter.WriteLine(text);
        }

        public static IProgressNotifier[] CreateProgressNotifiers()
        {
            return
            [
                ParallelProgressNotifierProvider.Default.CreateProgressNotifier(WriteImmediateProgress),
                new DefaultProgressNotifier(WriteOutput)
            ];
        }
    }
}
