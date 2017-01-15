using LightBDD.Configuration;
using LightBDD.Extensions.ContextualAsyncExecution;
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
                .AddScenarioExtension(new ScenarioExecutionContextExtension())
                .AddStepExtension(new StepCommentingExtension());
        }
    }
}