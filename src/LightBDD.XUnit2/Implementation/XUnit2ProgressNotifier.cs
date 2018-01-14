using System;
using System.Diagnostics;
using LightBDD.Core.Notification;
using LightBDD.Framework.Notification;

namespace LightBDD.XUnit2.Implementation
{
    [DebuggerStepThrough]
    internal class XUnit2ProgressNotifier
    {
        public static IFeatureProgressNotifier CreateFeatureProgressNotifier()
        {
            return ParallelProgressNotifierProvider.Default.CreateFeatureProgressNotifier(Console.WriteLine);
        }

        public static IScenarioProgressNotifier CreateImmediateScenarioProgressNotifier()
        {
            return ParallelProgressNotifierProvider.Default.CreateScenarioProgressNotifier(Console.WriteLine);
        }

        public static IScenarioProgressNotifier CreateSummarizingScenarioProgressNotifier(ITestOutputProvider fixture)
        {
            return new DefaultProgressNotifier(fixture.TestOutput.WriteLine);
        }
    }
}
