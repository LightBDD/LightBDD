using LightBDD.Core.Notification;
using LightBDD.Notification;
using NUnit.Framework;

namespace LightBDD.Integration.NUnit3
{
    internal class NUnit3ProgressNotifier
    {
        public static IFeatureProgressNotifier CreateFeatureProgressNotifier()
        {
            return ParallelProgressNotifierProvider.Default.CreateFeatureProgressNotifier(TestContext.WriteLine);
        }

        public static IScenarioProgressNotifier CreateScenarioProgressNotifier()
        {
            return ParallelProgressNotifierProvider.Default.CreateScenarioProgressNotifier(text => TestContextProvider.Current.TestOutWriter.WriteLine(text));
        }
    }
}
