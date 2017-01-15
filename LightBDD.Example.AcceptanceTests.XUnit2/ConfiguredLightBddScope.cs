using LightBDD.Configuration;
using LightBDD.Example.AcceptanceTests.XUnit2;
using LightBDD.Extensions.ContextualAsyncExecution;

[assembly: ConfiguredLightBddScope]
namespace LightBDD.Example.AcceptanceTests.XUnit2
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