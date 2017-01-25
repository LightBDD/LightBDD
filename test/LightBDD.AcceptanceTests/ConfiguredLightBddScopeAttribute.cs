using LightBDD.AcceptanceTests;
using LightBDD.Core.Configuration;
using LightBDD.Reporting;
using LightBDD.Reporting.Configuration;
using LightBDD.Reporting.Formatters;

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