using Xunit;
using Xunit.Sdk;

namespace LightBDD.Integration.XUnit2.UnitTests
{
    public class FeatureFixtureTests
    {
        class TestableFeatureFixture : FeatureFixture
        {
            public IBddRunner GetRunner() => Runner;

            public TestableFeatureFixture() : base(new TestOutputHelper()) { }
        }

        [Fact]
        public void Runner_should_be_initialized()
        {
            Assert.NotNull(new TestableFeatureFixture().GetRunner());
        }
    }
}