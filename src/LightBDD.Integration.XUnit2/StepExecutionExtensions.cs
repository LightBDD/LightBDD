using LightBDD.Integration.XUnit2.Customization;

namespace LightBDD
{
    public static class StepExecutionExtensions
    {
        public static void IgnoreScenario(this StepExecution execution, string reason)
        {
            throw new IgnoreException(reason);
        }
    }
}