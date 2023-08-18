﻿using LightBDD.Core.Configuration;
using LightBDD.Core.Formatting.ExceptionFormatting;
using LightBDD.Core.Notification;
using LightBDD.Runner.Implementation;

namespace LightBDD.Runner.Configuration
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
        /// Appends LightBDD.XUnit2 default feature progress notifiers.
        /// </summary>
        public static ProgressNotifierRegistrator AddFrameworkDefaultProgressNotifiers(this ProgressNotifierRegistrator configuration)
        {
            foreach (var notifier in XUnit2ProgressNotifier.CreateProgressNotifiers())
                configuration.Add(notifier);
            return configuration;
        }
    }
}
