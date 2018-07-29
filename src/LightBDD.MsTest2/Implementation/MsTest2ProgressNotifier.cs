using System;
using System.Diagnostics;
using LightBDD.Core.Notification;
using LightBDD.Framework.Notification;

namespace LightBDD.MsTest2.Implementation
{
    [DebuggerStepThrough]
    internal class MsTest2ProgressNotifier
    {
        public static IFeatureProgressNotifier CreateFeatureProgressNotifier()
        {
            return NoProgressNotifier.Default;
        }

        public static IScenarioProgressNotifier CreateScenarioProgressNotifier(ITestContextProvider fixture)
        {
            if (fixture.TestContext == null)
                throw new InvalidOperationException($"The {nameof(ITestContextProvider.TestContext)} property value is not set.\nPlease ensure that {fixture.GetType()} does not hide {nameof(FeatureFixture)}.{nameof(FeatureFixture.TestContext)} property and if not, report issue for LightBDD.");
            return new DefaultProgressNotifier(fixture.TestContext.WriteLine);
        }
    }
}
