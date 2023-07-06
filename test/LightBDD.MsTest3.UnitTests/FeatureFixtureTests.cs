using LightBDD.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace LightBDD.MsTest3.UnitTests
{
    [TestClass]
    public class FeatureFixtureTests
    {
        private class TestableFeatureFixture : FeatureFixture
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

        [TestMethod]
        public void TestRun_should_be_started()
        {
            Assert.IsTrue(ConfiguredLightBddScope.CapturedNotifications.Contains("TEST RUN STARTING: LightBDD.MsTest3.UnitTests"));
        }
    }
}