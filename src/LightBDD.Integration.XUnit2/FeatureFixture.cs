using Xunit.Abstractions;

namespace LightBDD
{
    [FeatureFixture]
    public class FeatureFixture : ITestOutputProvider
    {
        public ITestOutputHelper TestOutput { get; }
        protected IBddRunner Runner { get; }

        protected FeatureFixture(ITestOutputHelper output)
        {
            TestOutput = output;
            Runner = FeatureFactory.GetRunnerFor(GetType()).GetRunner(this);
        }
    }
}