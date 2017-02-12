using LightBDD.Core.Configuration;
using LightBDD.MsTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace $rootnamespace$
{
    [TestClass]
    public class LightBddIntegration
    {
        [AssemblyInitialize]
        public static void Setup(TestContext testContext) { LightBddScope.Initialize(OnConfigure); }
        [AssemblyCleanup]
        public static void Cleanup() { LightBddScope.Cleanup(); }

        private static void OnConfigure(LightBddConfiguration configuration)
        {
            // custom configuration
        }
    }
}