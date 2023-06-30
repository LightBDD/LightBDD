﻿using System;
using LightBDD.Core.Formatting.ExceptionFormatting;
using LightBDD.Framework.Configuration;
using LightBDD.MsTest3.Implementation;

namespace LightBDD.MsTest3.Configuration
{
    /// <summary>
    /// Extensions allowing to apply test framework default configuration.
    /// </summary>
    public static class TestFrameworkConfigurationExtensions
    {
        /// <summary>
        /// Adds MsTest specific stack trace member exclusions.
        /// </summary>
        /// <param name="formatter">Formatter.</param>
        /// <returns>Formatter.</returns>
        public static DefaultExceptionFormatter WithTestFrameworkDefaults(this DefaultExceptionFormatter formatter)
        {
            return formatter
                .WithMembersExcludedFromStackTrace("Microsoft\\.VisualStudio\\.TestTools\\..*");
        }

        /// <summary>
        /// Appends LightBDD.MsTest2 default scenario progress notifiers.
        /// </summary>
        [Obsolete("Use " + nameof(ProgressNotifierConfiguration) + " instead", true)]
        public static ScenarioProgressNotifierConfiguration AppendFrameworkDefaultProgressNotifiers(this ScenarioProgressNotifierConfiguration configuration)
        {
            return configuration.AppendNotifierProviders<ITestContextProvider>(MsTest2ProgressNotifier.CreateScenarioProgressNotifier);
        }

        /// <summary>
        /// Appends LightBDD.MsTest2 default feature progress notifiers.
        /// </summary>
        [Obsolete("Use " + nameof(ProgressNotifierConfiguration) + " instead", true)]
        public static FeatureProgressNotifierConfiguration AppendFrameworkDefaultProgressNotifiers(this FeatureProgressNotifierConfiguration configuration)
        {
            return configuration.AppendNotifiers(MsTest2ProgressNotifier.CreateFeatureProgressNotifier());
        }

        /// <summary>
        /// Appends LightBDD.MsTest2 default progress notifiers.
        /// </summary>
        public static ProgressNotifierConfiguration AppendFrameworkDefaultProgressNotifiers(this ProgressNotifierConfiguration configuration)
        {
            return configuration.Append(MsTest2ProgressNotifier.CreateProgressNotifier());
        }
    }
}
