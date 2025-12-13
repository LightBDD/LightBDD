using LightBDD.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LightBDD.MsTest3.UnitTests
{
    [TestClass]
    public class FeatureFixtureTests
    {
        public class TestableFeatureFixture : FeatureFixture
        {
            public IBddRunner GetRunner()
            {
                return Runner;
            }
        }

        [TestMethod]
        public void Runner_should_be_initialized()
        {
            Assert.IsNotNull(new TestableFeatureFixture().GetRunner());
        }
    }
}