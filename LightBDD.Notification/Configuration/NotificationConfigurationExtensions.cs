using LightBDD.Core.Configuration;

namespace LightBDD.Notification.Configuration
{
    public static class NotificationConfigurationExtensions
    {

        public static FeatureProgressNotifierConfiguration FeatureProgressNotifierConfiguration(this LightBddConfiguration cfg)
        {
            return cfg.Get<FeatureProgressNotifierConfiguration>();
        }

        public static ScenarioProgressNotifierConfiguration ScenarioProgressNotifierConfiguration(this LightBddConfiguration cfg)
        {
            return cfg.Get<ScenarioProgressNotifierConfiguration>();
        }
    }
}