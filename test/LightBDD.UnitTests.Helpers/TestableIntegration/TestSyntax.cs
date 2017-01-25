using LightBDD.Core.Extensibility;

namespace LightBDD.UnitTests.Helpers.TestableIntegration
{
    public static class TestSyntax
    {
        public static TestSyntaxRunner Test(this IBddRunner runner)
        {
            return new TestSyntaxRunner(runner.Integrate());
        }
    }
}