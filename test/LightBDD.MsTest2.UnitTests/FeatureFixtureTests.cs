using LightBDD.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LightBDD.MsTest2.UnitTests
{
    [TestClass]
    public class FeatureFixtureTests
    {
        class TestableFeatureFixture : FeatureFixture
        {
            public IBddRunner GetRunner() => Runner;
        }

        [TestMethod]
        public void Runner_should_be_initialized()
        {
            Assert.IsNotNull(new TestableFeatureFixture().GetRunner());
        }
    }
}