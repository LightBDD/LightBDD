using LightBDD.Extensions.ContextualAsyncExecution;
using LightBDD.Implementation;

namespace LightBDD
{
    public static class StepExecutionExtensions
    {
        public static void Comment(this StepExecution execution, string comment)
        {
            if (!string.IsNullOrWhiteSpace(comment))
                ScenarioExecutionContext.Current.Get<CurrentStepProperty>().Step.Comment(comment);
        }
    }
}
