using System;
using LightBDD.Core.Notification;
using LightBDD.Framework.Notification;

namespace LightBDD.Fixie2.Implementation
{
    internal class FixieProgressNotifier
    {
        [Obsolete]
        public static IFeatureProgressNotifier CreateFeatureProgressNotifier()
        {
            return ParallelProgressNotifierProvider.Default.CreateFeatureProgressNotifier(Console.WriteLine);
        }

        [Obsolete]
        public static IScenarioProgressNotifier CreateImmediateScenarioProgressNotifier()
        {
            return ParallelProgressNotifierProvider.Default.CreateScenarioProgressNotifier(Console.WriteLine);
        }

        public static IProgressNotifier CreateProgressNotifier()
        {
            return ParallelProgressNotifierProvider.Default.CreateProgressNotifier(Console.WriteLine);
        }
    }
}
