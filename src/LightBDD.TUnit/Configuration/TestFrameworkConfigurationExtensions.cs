using System;
using LightBDD.Core.Formatting.ExceptionFormatting;
using LightBDD.Framework.Configuration;
using LightBDD.TUnit.Implementation;

namespace LightBDD.TUnit.Configuration
{
    /// <summary>
    /// Extensions allowing to apply test framework default configuration.
    /// </summary>
    public static class TestFrameworkConfigurationExtensions
    {
        /// <summary>
        /// Adds TUnit specific stack trace member exclusions.
        /// </summary>
        /// <param name="formatter">Formatter.</param>
        /// <returns>Formatter.</returns>
        public static DefaultExceptionFormatter WithTestFrameworkDefaults(this DefaultExceptionFormatter formatter)
        {
            return formatter
                .WithMembersExcludedFromStackTrace("TUnit\\..*");
        }

        /// <summary>
        /// Appends LightBDD.TUnit default scenario progress notifiers.
        /// </summary>
        [Obsolete("Use " + nameof(ProgressNotifierConfiguration) + " instead", true)]
        public static ScenarioProgressNotifierConfiguration AppendFrameworkDefaultProgressNotifiers(this ScenarioProgressNotifierConfiguration configuration)
        {
            return configuration.AppendNotifierProviders(TUnitProgressNotifier.CreateScenarioProgressNotifier);
        }

        /// <summary>
        /// Appends LightBDD.TUnit default feature progress notifiers.
        /// </summary>
        [Obsolete("Use " + nameof(ProgressNotifierConfiguration) + " instead", true)]
        public static FeatureProgressNotifierConfiguration AppendFrameworkDefaultProgressNotifiers(this FeatureProgressNotifierConfiguration configuration)
        {
            return configuration.AppendNotifiers(TUnitProgressNotifier.CreateFeatureProgressNotifier());
        }

        /// <summary>
        /// Appends LightBDD.TUnit default progress notifiers.
        /// </summary>
        public static ProgressNotifierConfiguration AppendFrameworkDefaultProgressNotifiers(this ProgressNotifierConfiguration configuration)
        {
            return configuration.Append(TUnitProgressNotifier.CreateProgressNotifiers());
        }
    }
}
