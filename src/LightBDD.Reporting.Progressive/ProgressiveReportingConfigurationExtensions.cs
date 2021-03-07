using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using LightBDD.Core.Configuration;
using LightBDD.Core.Reporting;
using LightBDD.Core.Results;
using LightBDD.Framework.Configuration;

namespace LightBDD.Reporting.Progressive
{
    public static class ProgressiveReportingConfigurationExtensions
    {
        public static LightBddConfiguration EnableProgressiveReporting(this LightBddConfiguration cfg)
        {
            var reportWriter = new ProgressiveReportWriter();
            cfg.ProgressNotifierConfiguration().Append(reportWriter.Notifier);
            cfg.ReportWritersConfiguration().Add(reportWriter);
            return cfg;
        }
    }
}
