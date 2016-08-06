using LightBDD.Core.Notification;
using NUnit.Framework;

namespace LightBDD.Integration.NUnit3
{
    public class NUnit3ProgressNotifier
    {
        public static IFeatureProgressNotifier CreateFeatureProgressNotifier()
        {
            return ParallelProgressNotifierProvider.Default.CreateFeatureProgressNotifier(TestContext.WriteLine);
        }

        public static IScenarioProgressNotifier CreateScenarioProgressNotifier()
        {
            return ParallelProgressNotifierProvider.Default.CreateScenarioProgressNotifier(TestContext.WriteLine);
        }
    }
}
