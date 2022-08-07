using System;
using LightBDD.Core.Formatting.ExceptionFormatting;
using LightBDD.Framework.Configuration;
using LightBDD.XUnit2.Implementation;

namespace LightBDD.XUnit2.Configuration
{
    /// <summary>
    /// Extensions allowing to apply test framework default configuration.
    /// </summary>
    public static class TestFrameworkConfigurationExtensions
    {
        /// <summary>
        /// Adds xunit specific stack trace member exclusions.
        /// </summary>
        /// <param name="formatter">Formatter.</param>
        /// <returns>Formatter.</returns>
        public static DefaultExceptionFormatter WithTestFrameworkDefaults(this DefaultExceptionFormatter formatter)
        {
            return formatter
                .WithMembersExcludedFromStackTrace("Xunit\\..*");
        }

        /// <summary>
        /// Appends LightBDD.XUnit2 default scenario progress notifiers.
        /// </summary>
        [Obsolete("Use " + nameof(ProgressNotifierConfiguration) + " instead", true)]
        public static ScenarioProgressNotifierConfiguration AppendFrameworkDefaultProgressNotifiers(this ScenarioProgressNotifierConfiguration configuration)
        {
            return configuration
                .AppendNotifierProviders(XUnit2ProgressNotifier.CreateImmediateScenarioProgressNotifier)
                .AppendNotifierProviders<ITestOutputProvider>(XUnit2ProgressNotifier.CreateSummarizingScenarioProgressNotifier);
        }

        /// <summary>
        /// Appends LightBDD.XUnit2 default feature progress notifiers.
        /// </summary>
        [Obsolete("Use " + nameof(ProgressNotifierConfiguration) + " instead", true)]
        public static FeatureProgressNotifierConfiguration AppendFrameworkDefaultProgressNotifiers(this FeatureProgressNotifierConfiguration configuration)
        {
            return configuration.AppendNotifiers(XUnit2ProgressNotifier.CreateFeatureProgressNotifier());
        }

        /// <summary>
        /// Appends LightBDD.XUnit2 default feature progress notifiers.
        /// </summary>
        public static ProgressNotifierConfiguration AppendFrameworkDefaultProgressNotifiers(this ProgressNotifierConfiguration configuration)
        {
            return configuration.Append(XUnit2ProgressNotifier.CreateProgressNotifiers());
        }
    }
}
