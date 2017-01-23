using System;
using LightBDD.Core.Notification;
using LightBDD.Notification.Implementation;

namespace LightBDD.Notification
{
    public class ParallelProgressNotifierProvider
    {
        private readonly ProgressManager _manager = new ProgressManager();
        public static ParallelProgressNotifierProvider Default { get; } = new ParallelProgressNotifierProvider();

        public IFeatureProgressNotifier CreateFeatureProgressNotifier(params Action<string>[] onNotify)
        {
            return new ParallelProgressNotifier(_manager, onNotify);
        }

        public IScenarioProgressNotifier CreateScenarioProgressNotifier(params Action<string>[] onNotify)
        {
            return new ParallelProgressNotifier(_manager, onNotify);
        }
    }
}