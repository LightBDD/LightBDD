using LightBDD.MsTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LightBDD.Integration.MsTest.UnitTests
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