namespace LightBDD.Core.UnitTests.TestableIntegration
{
    public static class StepExecutionExtensions
    {
        public static void IgnoreScenario(this StepExecution execution, string reason)
        {
            throw new CustomIgnoreException(reason);
        }
    }
}