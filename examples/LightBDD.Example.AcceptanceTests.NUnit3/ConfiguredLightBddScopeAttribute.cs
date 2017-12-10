using LightBDD.Core.Configuration;
using LightBDD.Example.AcceptanceTests.NUnit3;
using LightBDD.Framework.Notification;
using LightBDD.Framework.Notification.Configuration;
using LightBDD.Framework.Reporting.Configuration;
using LightBDD.Framework.Reporting.Formatters;
using LightBDD.NUnit3;

[assembly: ConfiguredLightBddScope]
namespace LightBDD.Example.AcceptanceTests.NUnit3
{
    internal class ConfiguredLightBddScopeAttribute : LightBddScopeAttribute
    {
        protected override void OnConfigure(LightBddConfiguration configuration)
        {
            configuration
                .ReportWritersConfiguration()
                .AddFileWriter<PlainTextReportFormatter>("~\\Reports\\{TestDateTimeUtc:yyyy-MM-dd-HH_mm_ss}_FeaturesReport.txt");

            var notifier = new IncrementalJsonProgressNotifier("~\\Progress\\progress.json");
            configuration
                .FeatureProgressNotifierConfiguration()
                .UpdateNotifier(notifier);
            configuration
                .ScenarioProgressNotifierConfiguration()
                .UpdateNotifierProvider(()=>notifier);
            configuration
                .LightBddProgressNotifierConfiguration()
                .UpdateNotifier(notifier);
        }
    }
}