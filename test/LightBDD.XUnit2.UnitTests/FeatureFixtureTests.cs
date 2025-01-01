using LightBDD.Framework;
using Xunit;

namespace LightBDD.XUnit2.UnitTests
{
    public class FeatureFixtureTests
    {
        private class TestableFeatureFixture : FeatureFixture
        {
            public IBddRunner GetRunner()
            {
                return Runner;
            }
        }

        [Fact]
        public void Runner_should_be_initialized()
        {
            Assert.NotNull(new TestableFeatureFixture().GetRunner());
        }
    }
}