using System;
using LightBDD.Core.Configuration;
using LightBDD.Core.Formatting.ExceptionFormatting;
using LightBDD.Fixie3.Implementation;
using LightBDD.Framework.Configuration;

namespace LightBDD.Fixie3.Configuration
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
        /// Appends LightBDD.Fixie3 default scenario progress notifiers.
        /// </summary>
        [Obsolete("Use " + nameof(ProgressNotifierConfiguration) + " instead", true)]
        public static ScenarioProgressNotifierConfiguration AppendFrameworkDefaultProgressNotifiers(this ScenarioProgressNotifierConfiguration configuration)
        {
            return configuration
                .AppendNotifierProviders(FixieProgressNotifier.CreateImmediateScenarioProgressNotifier);
        }

        /// <summary>
        /// Appends LightBDD.Fixie3 default feature progress notifiers.
        /// </summary>
        [Obsolete("Use " + nameof(ProgressNotifierConfiguration) + " instead", true)]
        public static FeatureProgressNotifierConfiguration AppendFrameworkDefaultProgressNotifiers(this FeatureProgressNotifierConfiguration configuration)
        {
            return configuration.AppendNotifiers(FixieProgressNotifier.CreateFeatureProgressNotifier());
        }

        /// <summary>
        /// Appends LightBDD.Fixie3 default progress notifiers.
        /// </summary>
        public static ProgressNotifierConfiguration AppendFrameworkDefaultProgressNotifiers(this ProgressNotifierConfiguration configuration)
        {
            return configuration.Append(FixieProgressNotifier.CreateProgressNotifier());
        }
    }
}
