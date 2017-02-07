using System.Diagnostics;
using LightBDD.Core.Notification;
using LightBDD.Framework.Notification;

namespace LightBDD.NUnit3.Implementation
{
    [DebuggerStepThrough]
    internal class NUnit3ProgressNotifier
    {
        public static IFeatureProgressNotifier CreateFeatureProgressNotifier()
        {
            return NoProgressNotifier.Default;
        }

        public static IScenarioProgressNotifier CreateScenarioProgressNotifier()
        {
            return new DefaultProgressNotifier(WriteScenarioProgress);
        }

        private static void WriteScenarioProgress(string text)
        {
            TestContextProvider.Current.TestOutWriter.WriteLine(text);
        }
    }
}
