using LightBDD.Core.Notification;
using LightBDD.Framework.Notification;
using NUnit.Framework;

namespace LightBDD.NUnit3.Implementation
{
    internal class NUnit3ProgressNotifier
    {
        private static readonly DefaultProgressNotifier SummarizingProgressNotifier = new DefaultProgressNotifier(WriteOutput);

        public static IFeatureProgressNotifier CreateFeatureProgressNotifier()
        {
            return new DelegatingFeatureProgressNotifier(ParallelProgressNotifierProvider.Default.CreateFeatureProgressNotifier(WriteImmediateProgress));
        }

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
    }
}
