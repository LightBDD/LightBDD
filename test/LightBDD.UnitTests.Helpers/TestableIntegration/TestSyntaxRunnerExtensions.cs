using LightBDD.Framework;
using LightBDD.ScenarioHelpers;

namespace LightBDD.UnitTests.Helpers.TestableIntegration
{
    public static class TestSyntaxRunnerExtensions
    {
        public static TestScenarioBuilder Test(this IBddRunner runner)
        {
            return new TestScenarioBuilder(runner.Integrate().Core);
        }
    }
}