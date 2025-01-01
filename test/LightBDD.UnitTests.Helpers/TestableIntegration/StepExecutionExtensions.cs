using LightBDD.Core.ExecutionContext;
using LightBDD.Framework;

namespace LightBDD.UnitTests.Helpers.TestableIntegration
{
    public static class StepExecutionExtensions
    {
        public static void IgnoreScenario(this StepExecution execution, string reason)
        {
            ScenarioExecutionContext.ValidateScenarioScope();
            throw new CustomIgnoreException(reason);
        }
    }
}