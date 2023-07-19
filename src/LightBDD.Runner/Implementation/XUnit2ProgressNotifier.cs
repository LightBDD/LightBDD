using System;
using LightBDD.Core.ExecutionContext;
using LightBDD.Core.Notification;
using LightBDD.Framework.Notification;

namespace LightBDD.XUnit2.Implementation
{
    internal class XUnit2ProgressNotifier
    {
        [Obsolete]
        public static IFeatureProgressNotifier CreateFeatureProgressNotifier()
        {
            return ParallelProgressNotifierProvider.Default.CreateFeatureProgressNotifier(Console.WriteLine);
        }

        [Obsolete]
        public static IScenarioProgressNotifier CreateImmediateScenarioProgressNotifier()
        {
            return ParallelProgressNotifierProvider.Default.CreateScenarioProgressNotifier(Console.WriteLine);
        }

        [Obsolete]
        public static IScenarioProgressNotifier CreateSummarizingScenarioProgressNotifier(ITestOutputProvider fixture)
        {
            return new DefaultProgressNotifier(fixture.TestOutput.WriteLine);
        }

        public static IProgressNotifier[] CreateProgressNotifiers()
        {
            return new[]
            {
                ParallelProgressNotifierProvider.Default.CreateProgressNotifier(Console.WriteLine),
                new DefaultProgressNotifier(WriteTestOutput)
            };
        }

        private static void WriteTestOutput(string message) => ScenarioExecutionContext.GetCurrentScenarioFixtureIfPresent<ITestOutputProvider>()?.TestOutput.WriteLine(message);
    }
}
