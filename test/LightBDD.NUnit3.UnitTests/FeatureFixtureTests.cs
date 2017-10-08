using LightBDD.Framework;
using NUnit.Framework;

namespace LightBDD.NUnit3.UnitTests
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