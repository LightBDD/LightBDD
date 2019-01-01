using LightBDD.Framework;
using LightBDD.Framework.Extensibility;

namespace LightBDD.UnitTests.Helpers.TestableIntegration
{
    public static class TestSyntaxRunnerExtensions
    {
        public static TestSyntaxRunner Test(this IBddRunner runner)
        {
            return new TestSyntaxRunner(runner);
        }
    }
}