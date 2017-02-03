using LightBDD.AcceptanceTests;
using LightBDD.Core.Configuration;
using LightBDD.Framework.Reporting;
using LightBDD.Framework.Reporting.Configuration;
using LightBDD.Framework.Reporting.Formatters;

[assembly: ConfiguredLightBddScope]
namespace LightBDD.AcceptanceTests
{
    public class ConfiguredLightBddScopeAttribute : LightBddScopeAttribute
    {
        protected override void OnConfigure(LightBddConfiguration configuration)
        {
            configuration.ReportWritersConfiguration()
                .Add(new ReportFileWriter(new PlainTextReportFormatter(), "~\\Reports\\FeaturesReport.txt"));
        }
    }
}