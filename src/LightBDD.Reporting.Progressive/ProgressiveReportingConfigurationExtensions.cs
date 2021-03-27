using LightBDD.Core.Configuration;
using LightBDD.Framework.Configuration;

namespace LightBDD.Reporting.Progressive
{
    /// <summary>
    /// Progressive reporting configuration extensions.
    /// </summary>
    public static class ProgressiveReportingConfigurationExtensions
    {
        /// <summary>
        /// Enables progressive reporting.
        /// </summary>
        /// <param name="cfg"></param>
        /// <returns></returns>
        public static LightBddConfiguration EnableProgressiveReporting(this LightBddConfiguration cfg)
        {
            var reportWriter = new ProgressiveReportWriter();
            cfg.ProgressNotifierConfiguration().Append(reportWriter.Notifier);
            cfg.ReportWritersConfiguration().Add(reportWriter);
            return cfg;
        }
    }
}
