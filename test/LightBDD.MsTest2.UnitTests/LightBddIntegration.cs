using LightBDD.MsTest2;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LightBDD.Integration.MsTest.UnitTests2
{
    [TestClass]
    class LightBddIntegration
    {
        [AssemblyInitialize]
        public static void Setup(TestContext testContext)
        {
            LightBddScope.Initialize();
        }

        [AssemblyCleanup]
        public static void Cleanup() { LightBddScope.Cleanup(); }
    }
}