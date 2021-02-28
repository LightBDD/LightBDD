using System;
using LightBDD.Core.Formatting.ExceptionFormatting;
using LightBDD.Framework.Configuration;
using LightBDD.NUnit3.Implementation;

namespace LightBDD.NUnit3.Configuration
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
            return formatter
                .WithMembersExcludedFromStackTrace("NUnit\\..*");
        }

        /// <summary>
        /// Appends LightBDD.NUnit3 default scenario progress notifiers.
        /// </summary>
        [Obsolete("Use " + nameof(ProgressNotifierConfiguration) + " instead", true)]
        public static ScenarioProgressNotifierConfiguration AppendFrameworkDefaultProgressNotifiers(this ScenarioProgressNotifierConfiguration configuration)
        {
            return configuration.AppendNotifierProviders(NUnit3ProgressNotifier.CreateScenarioProgressNotifier);
        }

        /// <summary>
        /// Appends LightBDD.NUnit3 default feature progress notifiers.
        /// </summary>
        [Obsolete("Use " + nameof(ProgressNotifierConfiguration) + " instead", true)]
        public static FeatureProgressNotifierConfiguration AppendFrameworkDefaultProgressNotifiers(this FeatureProgressNotifierConfiguration configuration)
        {
            return configuration.AppendNotifiers(NUnit3ProgressNotifier.CreateFeatureProgressNotifier());
        }

        /// <summary>
        /// Appends LightBDD.NUnit3 default progress notifiers.
        /// </summary>
        public static ProgressNotifierConfiguration AppendFrameworkDefaultProgressNotifiers(this ProgressNotifierConfiguration configuration)
        {
            return configuration.Append(NUnit3ProgressNotifier.CreateProgressNotifiers());
        }
    }
}
