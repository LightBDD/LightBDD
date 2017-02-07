using System;
using System.Diagnostics;
using LightBDD.Core.Notification;
using LightBDD.Framework.Notification;

namespace LightBDD.MsTest.Implementation
{
    [DebuggerStepThrough]
    internal class MsTestProgressNotifier
    {
        private static readonly DefaultProgressNotifier ProgressNotifier = new DefaultProgressNotifier(Console.WriteLine);

        public static IFeatureProgressNotifier CreateFeatureProgressNotifier()
        {
            return ProgressNotifier;
        }

        public static IScenarioProgressNotifier CreateScenarioProgressNotifier()
        {
            return ProgressNotifier;
        }
    }
}
