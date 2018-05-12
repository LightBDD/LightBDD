using LightBDD.Core.Formatting.ExceptionFormatting;
using LightBDD.Fixie2.Implementation;
using LightBDD.Framework.Notification.Configuration;

namespace LightBDD.Fixie2.Configuration
{
    /// <summary>
    /// Extensions allowing to apply test framework default configuration.
    /// </summary>
    public static class TestFrameworkConfigurationExtensions
    {
        /// <summary>
        /// Adds Fixie specific stack trace member exclusions.
        /// </summary>
        /// <param name="formatter">Formatter.</param>
        /// <returns>Formatter.</returns>
        public static DefaultExceptionFormatter WithTestFrameworkDefaults(this DefaultExceptionFormatter formatter)
        {
            return formatter;
        }

        /// <summary>
        /// Appends LightBDD.Fixie2 default scenario progress notifiers.
        /// </summary>
        public static ScenarioProgressNotifierConfiguration AppendFrameworkDefaultProgressNotifiers(this ScenarioProgressNotifierConfiguration configuration)
        {
            return configuration
                .AppendNotifierProviders(FixieProgressNotifier.CreateImmediateScenarioProgressNotifier);
        }

        /// <summary>
        /// Appends LightBDD.Fixie2 default feature progress notifiers.
        /// </summary>
        public static FeatureProgressNotifierConfiguration AppendFrameworkDefaultProgressNotifiers(this FeatureProgressNotifierConfiguration configuration)
        {
            return configuration.AppendNotifiers(FixieProgressNotifier.CreateFeatureProgressNotifier());
        }
    }
}
