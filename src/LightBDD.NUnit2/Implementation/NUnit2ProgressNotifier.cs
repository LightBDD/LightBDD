using System;
using System.Diagnostics;
using LightBDD.Core.Notification;
using LightBDD.Framework.Notification;

namespace LightBDD.NUnit2.Implementation
{
    internal class NUnit2ProgressNotifier
    {
        private static readonly DefaultProgressNotifier Notifier = new DefaultProgressNotifier(Console.WriteLine);

        public static IFeatureProgressNotifier CreateFeatureProgressNotifier()
        {
            return Notifier;
        }

        public static IScenarioProgressNotifier CreateScenarioProgressNotifier()
        {
            return Notifier;
        }
    }
}
