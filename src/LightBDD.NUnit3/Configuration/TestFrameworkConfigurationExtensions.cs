using System;
using LightBDD.Core.Configuration;
using LightBDD.Core.Formatting.ExceptionFormatting;
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
        /// Appends LightBDD.NUnit3 default progress notifiers.
        /// </summary>
        public static ProgressNotifierConfiguration AppendFrameworkDefaultProgressNotifiers(this ProgressNotifierConfiguration configuration)
        {
            return configuration.Append(NUnit3ProgressNotifier.CreateProgressNotifiers());
        }
    }
}
