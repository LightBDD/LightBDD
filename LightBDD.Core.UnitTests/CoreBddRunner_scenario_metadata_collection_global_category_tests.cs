using System.Linq;
using LightBDD.Core.Extensibility;
using LightBDD.Core.UnitTests.Helpers;
using LightBDD.Core.UnitTests.TestableIntegration;
using Xunit;

namespace LightBDD.Core.UnitTests
{
    [ScenarioCategory("global1")]
    [ScenarioCategory("global2")]
    public class CoreBddRunner_scenario_metadata_collection_global_category_tests : Steps
    {
        private readonly IBddRunner _runner;

        public CoreBddRunner_scenario_metadata_collection_global_category_tests()
        {
            _runner = new TestableBddRunner(GetType());
        }

        [Fact]
        public void It_should_capture_scenario_name_with_global_categories()
        {
            _runner.TestScenario(Some_step);
            var scenario = _runner.Integrate().GetFeatureResult().GetScenarios().Single();
            Assert.Equal(new[] { "global1", "global2" }, scenario.Info.Categories);
        }

        [Fact]
        [ScenarioCategory("local1")]
        [ScenarioCategory("local2")]
        public void It_should_capture_scenario_name_with_global_and_local_categories()
        {
            _runner.TestScenario(Some_step);
            var scenario = _runner.Integrate().GetFeatureResult().GetScenarios().Single();
            Assert.Equal(new[] { "global1", "global2", "local1", "local2" }, scenario.Info.Categories);
        }
    }
}