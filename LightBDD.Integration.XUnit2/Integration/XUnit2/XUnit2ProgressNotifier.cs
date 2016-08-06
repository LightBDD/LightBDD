using System;
using LightBDD.Core.Notification;
using Xunit.Abstractions;

namespace LightBDD.Integration.XUnit2
{
    public class XUnit2ProgressNotifier
    {
        public static IFeatureProgressNotifier CreateFeatureProgressNotifier()
        {
            return ParallelProgressNotifierProvider.Default.CreateFeatureProgressNotifier(Console.WriteLine);
        }

        public static IScenarioProgressNotifier CreateScenarioProgressNotifier(ITestOutputHelper helper)
        {
            return ParallelProgressNotifierProvider.Default.CreateScenarioProgressNotifier(Console.WriteLine, helper.WriteLine);
        }
    }
}
