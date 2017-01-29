using System;
using LightBDD.Core.Notification;
using LightBDD.Notification;
using Xunit.Abstractions;

namespace LightBDD.Integration.XUnit2
{
    internal class XUnit2ProgressNotifier
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
