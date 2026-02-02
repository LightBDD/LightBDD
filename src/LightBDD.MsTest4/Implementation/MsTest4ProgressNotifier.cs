using System;
using LightBDD.Core.ExecutionContext;
using LightBDD.Core.Notification;
using LightBDD.Framework.Notification;

namespace LightBDD.MsTest4.Implementation
{
    internal class MsTest4ProgressNotifier
    {
        [Obsolete]
        public static IFeatureProgressNotifier CreateFeatureProgressNotifier()
        {
            return NoProgressNotifier.Default;
        }

        [Obsolete]
        public static IScenarioProgressNotifier CreateScenarioProgressNotifier(ITestContextProvider fixture)
        {
            if (fixture.TestContext == null)
                throw new InvalidOperationException($"The {nameof(ITestContextProvider.TestContext)} property value is not set.\nPlease ensure that {fixture.GetType()} does not hide {nameof(FeatureFixture)}.{nameof(FeatureFixture.TestContext)} property and if not, report issue for LightBDD.");
            return new DefaultProgressNotifier(fixture.TestContext.WriteLine);
        }

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
