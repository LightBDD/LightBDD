using LightBDD.Core.Extensibility;
using LightBDD.Framework;

namespace LightBDD.UnitTests.Helpers.TestableIntegration
{
    public static class TestSyntaxRunnerExtensions
    {
        public static TestSyntaxRunner Test(this IBddRunner runner)
        {
            return new TestSyntaxRunner(runner);
        }

        public static TestSyntaxRunner Test(this ICoreScenarioBuilder scenarioBuilder)
        {
            return new TestSyntaxRunner(scenarioBuilder);
        }
    }
}