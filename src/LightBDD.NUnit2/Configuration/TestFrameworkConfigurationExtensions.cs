using LightBDD.Core.Formatting.ExceptionFormatting;
using LightBDD.Framework.Notification.Configuration;
using LightBDD.NUnit2.Implementation;

namespace LightBDD.NUnit2.Configuration
{
    /// <summary>
    /// Extensions allowing to apply test framework default configuration.
    /// </summary>
    public static class TestFrameworkConfigurationExtensions
    {
        /// <summary>
        /// Adds NUnit specific stack trace member exclusions.
        /// </summary>
        /// <param name="formatter">Formatter.</param>
        /// <returns>Formatter.</returns>
        public static DefaultExceptionFormatter WithTestFrameworkDefaults(this DefaultExceptionFormatter formatter)
        {
            return formatter.WithMembersExcludedFromStackTrace("NUnit\\..*");
        }

        /// <summary>
        /// Appends LightBDD.NUnit2 default scenario progress notifiers.
        /// </summary>
        public static ScenarioProgressNotifierConfiguration AppendFrameworkDefaultProgressNotifiers(this ScenarioProgressNotifierConfiguration configuration)
        {
            return configuration.AppendNotifierProviders(NUnit2ProgressNotifier.CreateScenarioProgressNotifier);
        }

        /// <summary>
        /// Appends LightBDD.NUnit2 default feature progress notifiers.
        /// </summary>
        public static FeatureProgressNotifierConfiguration AppendFrameworkDefaultProgressNotifiers(this FeatureProgressNotifierConfiguration configuration)
        {
            return configuration.AppendNotifiers(NUnit2ProgressNotifier.CreateFeatureProgressNotifier());
        }
    }
}
