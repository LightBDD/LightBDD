using LightBDD.Framework;
using LightBDD.NUnit2;
using NUnit.Framework;

[assembly: LightBddScope]

namespace LightBDD.NUnit2.NUnitTests
{
    [TestFixture]
    public class FeatureFixtureTests
    {
        private class TestableFeatureFixture : FeatureFixture
        {
            public IBddRunner GetRunner()
            {
                return Runner;
            }
        }

        [Test]
        public void Runner_should_be_initialized()
        {
            Assert.IsNotNull(new TestableFeatureFixture().GetRunner());
        }
    }
}