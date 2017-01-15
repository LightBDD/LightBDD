using LightBDD.Configuration;
using LightBDD.Example.AcceptanceTests.NUnit3;
using LightBDD.Extensions.ContextualAsyncExecution;

[assembly: ConfiguredLightBddScope]
namespace LightBDD.Example.AcceptanceTests.NUnit3
{
    class ConfiguredLightBddScope : LightBddScopeAttribute
    {
        protected override void OnConfigure(LightBddConfiguration configuration)
        {
            configuration
                .ExecutionExtensionsConfiguration()
                .AddScenarioExtension(new ScenarioExecutionContextExtension())
                .AddStepExtension(new StepCommentingExtension());
        }
    }
}