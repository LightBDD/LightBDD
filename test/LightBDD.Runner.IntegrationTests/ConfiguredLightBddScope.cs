using LightBDD.Core.Configuration;
using LightBDD.Runner.IntegrationTests;
using LightBDD.Runner.IntegrationTests.Helpers;
using Xunit;

[assembly: ConfiguredLightBddScope]
[assembly: TestCollectionOrderer($"LightBDD.Runner.IntegrationTests.Helpers.{nameof(LightBddFirstOrderer)}", "LightBDD.Runner.IntegrationTests")]
[assembly: CollectionBehavior(DisableTestParallelization = true)]
namespace LightBDD.Runner.IntegrationTests
{
    internal class ConfiguredLightBddScope : LightBddScopeAttribute
    {
        public static LightBddConfiguration? CapturedConfiguration { get; private set; }

        protected override void OnConfigure(LightBddConfiguration configuration)
        {
            configuration.ReportConfiguration()
                .Clear();

            configuration.ProgressNotifierConfiguration()
                .Clear()
                .Append(new ProgressCapture());

            CapturedConfiguration = configuration;
        }
    }
}