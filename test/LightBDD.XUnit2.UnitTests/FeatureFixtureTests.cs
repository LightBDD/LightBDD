using LightBDD.Framework;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace LightBDD.XUnit2.UnitTests
{
    public class FeatureFixtureTests
    {
        private class TestableFeatureFixture : FeatureFixture
        {
            public TestableFeatureFixture(ITestOutputHelper output) : base(output)
            {
            }

            public IBddRunner GetRunner()
            {
                return Runner;
            }
        }

        [Fact]
        public void Runner_should_be_initialized()
        {
            Assert.NotNull(new TestableFeatureFixture(null).GetRunner());
        }

        [Fact]
        public void TestRun_should_be_started()
        {
            Assert.Contains("TEST RUN STARTING: LightBDD.XUnit2.UnitTests", ConfiguredLightBddScope.CapturedNotifications);
        }
    }
}