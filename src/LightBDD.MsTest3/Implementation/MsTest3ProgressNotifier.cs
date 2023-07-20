using System;
using LightBDD.Core.ExecutionContext;
using LightBDD.Core.Notification;
using LightBDD.Framework.Notification;

namespace LightBDD.MsTest3.Implementation
{
    internal class MsTest3ProgressNotifier
    {
        public static IProgressNotifier CreateProgressNotifier()
        {
            return new DefaultProgressNotifier(WriteTestOutput);
        }

        public static void WriteTestOutput(string message)
        {
            ScenarioExecutionContext.GetCurrentScenarioFixtureIfPresent<ITestContextProvider>()?.TestContext?.WriteLine(message);
        }
    }
}
