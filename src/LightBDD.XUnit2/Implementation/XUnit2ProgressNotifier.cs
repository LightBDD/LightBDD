using System;
using System.Diagnostics;
using LightBDD.Core.Notification;
using LightBDD.Framework.Notification;
using Xunit.Abstractions;

namespace LightBDD.XUnit2.Implementation
{
    [DebuggerStepThrough]
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
