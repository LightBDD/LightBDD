using LightBDD.Core.Extensibility;

namespace LightBDD.Core.UnitTests.TestableIntegration
{
    static class TestSyntax
    {
        public static TestSyntaxRunner Test(this IBddRunner runner)
        {
            return new TestSyntaxRunner(runner.Integrate());
        }
    }
}