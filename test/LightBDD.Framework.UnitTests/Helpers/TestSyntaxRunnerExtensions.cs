using LightBDD.ScenarioHelpers;

namespace LightBDD.Framework.UnitTests.Helpers
{
    public static class TestSyntaxRunnerExtensions
    {
        public static TestScenarioBuilder Test(this IBddRunner runner)
        {
            return new TestScenarioBuilder(runner.Integrate().Core);
        }
    }
}