using LightBDD.Core.Configuration;
using LightBDD.Core.Dependencies;
using LightBDD.Framework.Commenting.Configuration;
using LightBDD.Framework.Commenting.Implementation;
using LightBDD.Framework.Configuration;
using LightBDD.Framework.ExecutionContext;

namespace LightBDD.Framework.Commenting
{
    /// <summary>
    /// Extension class for <see cref="StepExecution"/> allowing to access dependency resolver used by this step.
    /// </summary>
    public static class StepDependencyResolverExtensions
    {
        /// <summary>
        /// Retrieves <see cref="IDependencyResolver"/> for currently executed step.
        /// <para>This feature has to be enabled in <see cref="LightBddConfiguration"/> via <see cref="FrameworkConfigurationExtensions.EnableCurrentStepManagement"/>() prior to usage.</para>
        /// </summary>
        /// <param name="execution">Current step execution instance.</param>
        public static IDependencyResolver GetDependencyResolver(this StepExecution execution)
        {
            return ScenarioExecutionContext.Current.Get<CurrentStepProperty>().Step.DependencyResolver;
        }
    }
}
