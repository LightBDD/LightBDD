using LightBDD.Core.Configuration;
using LightBDD.Runner.Tests;
using LightBDD.Runner.Tests.Helpers;
using Xunit;

[assembly: ConfiguredLightBddScope]
[assembly: TestCollectionOrderer($"LightBDD.Runner.Tests.Helpers.{nameof(LightBddFirstOrderer)}", "LightBDD.Runner.Tests")]
[assembly: CollectionBehavior(DisableTestParallelization = true)]
namespace LightBDD.Runner.Tests
{
    internal class ConfiguredLightBddScope : LightBddScopeAttribute
    {
        protected override void OnConfigure(LightBddConfiguration configuration)
        {
            configuration.ReportConfiguration()
                .Clear();

            configuration.ProgressNotifierConfiguration()
                .Clear()
                .Append(new ProgressCapture());
        }
    }
}