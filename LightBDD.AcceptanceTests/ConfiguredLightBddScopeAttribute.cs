using LightBDD.AcceptanceTests;
using LightBDD.Configuration;
using LightBDD.SummaryGeneration;
using LightBDD.SummaryGeneration.Configuration;
using LightBDD.SummaryGeneration.Formatters;

[assembly: ConfiguredLightBddScope]
namespace LightBDD.AcceptanceTests
{
    public class ConfiguredLightBddScopeAttribute : LightBddScopeAttribute
    {
        protected override void OnConfigure(LightBddConfiguration configuration)
        {
            configuration.SummaryWritersConfiguration()
                .Add(new SummaryFileWriter(new HtmlResultFormatter(), "~\\Reports\\FeaturesSummary.html"))
                .Add(new SummaryFileWriter(new PlainTextResultFormatter(), "~\\Reports\\FeaturesSummary.txt"));
        }
    }
}