using System;
using LightBDD.Core.Notification;

namespace LightBDD.Configuration
{
    public class FeatureProgressNotifierConfiguration : IFeatureConfiguration
    {
        public IFeatureProgressNotifier Notifier { get; private set; } = NoProgressNotifier.Default;

        public FeatureProgressNotifierConfiguration UpdateNotifier(IFeatureProgressNotifier notifier)
        {
            if (notifier == null)
                throw new ArgumentNullException(nameof(notifier));
            Notifier = notifier;
            return this;
        }
    }
}