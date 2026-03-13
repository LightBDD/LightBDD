using System;
using LightBDD.Core.ExecutionContext;
using LightBDD.Core.Notification;
using LightBDD.Framework.Notification;

namespace LightBDD.XUnit3.Implementation
{
    internal class XUnit3ProgressNotifier
    {
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
