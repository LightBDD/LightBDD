using System.Diagnostics;
using LightBDD.Core.Notification;
using LightBDD.Framework.Notification;
using NUnit.Framework;

namespace LightBDD.NUnit3.Implementation
{
    [DebuggerStepThrough]
    internal class NUnit3ProgressNotifier
    {
        public static IFeatureProgressNotifier CreateFeatureProgressNotifier()
        {
            return ParallelProgressNotifierProvider.Default.CreateFeatureProgressNotifier(TestContext.WriteLine);
        }

        public static IScenarioProgressNotifier CreateScenarioProgressNotifier()
        {
            return ParallelProgressNotifierProvider.Default.CreateScenarioProgressNotifier(WriteScenarioProgress);
        }

        private static void WriteScenarioProgress(string text)
        {
            TestContextProvider.Current.TestOutWriter.WriteLine(text);
        }
    }
}
