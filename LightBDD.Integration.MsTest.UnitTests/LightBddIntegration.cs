using LightBDD.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LightBDD.Integration.MsTest.UnitTests
{
    [TestClass]
    class LightBddIntegration
    {
        [AssemblyInitialize]
        public static void Setup(TestContext testContext)
        {
            var configuration = new LightBddConfiguration();
            LightBddScope.Initialize(configuration);
        }

        [AssemblyCleanup]
        public static void Cleanup() { LightBddScope.Cleanup(); }
    }
}