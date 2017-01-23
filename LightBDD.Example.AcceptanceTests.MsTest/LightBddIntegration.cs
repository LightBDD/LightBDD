using LightBDD.Commenting.Configuration;
using LightBDD.Core.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LightBDD.Example.AcceptanceTests.MsTest
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
            configuration
                .ExecutionExtensionsConfiguration()
                .EnableStepCommenting();
        }
    }
}