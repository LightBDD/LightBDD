using LightBDD.Core.Configuration;
using LightBDD.Framework.Commenting.Configuration;
using LightBDD.Framework.Commenting.Implementation;
using LightBDD.Framework.ExecutionContext;

namespace LightBDD.Framework.Commenting
{
    //TODO: final view on namespaces (Configuration, Formatting)
    /// <summary>
    /// Extension class for <see cref="StepExecution"/> allowing to comment currently running steps.
    /// </summary>
    public static class StepExecutionExtensions
    {
        /// <summary>
        /// Comments currently executed step with a <paramref name="comment"/> text.
        /// The comment would be included in progress notification, as well as in execution reports.
        /// <para>This feature has to be enabled in <see cref="LightBddConfiguration"/> via <see cref="CommentingConfigurationExtensions.EnableStepCommenting"/>() prior to usage.</para>
        /// </summary>
        /// <param name="execution">Current step execution instance.</param>
        /// <param name="comment">Comment to add. If comment is <c>null</c> or empty, it will not be added.</param>
        public static void Comment(this StepExecution execution, string comment)
        {
            if (!string.IsNullOrWhiteSpace(comment))
                ScenarioExecutionContext.Current.Get<CurrentStepProperty>().Step.Comment(comment);
        }
    }
}
