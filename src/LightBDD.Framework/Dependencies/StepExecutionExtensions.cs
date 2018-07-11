using LightBDD.Core.Configuration;
using LightBDD.Core.Dependencies;
using LightBDD.Framework.Configuration;
using LightBDD.Framework.ExecutionContext;
using LightBDD.Framework.Extensibility.Implementation;

namespace LightBDD.Framework.Dependencies
{
    /// <summary>
    /// Extension class for <see cref="StepExecution"/> allowing to access dependency resolver used by this step.
    /// </summary>
    public static class StepDependencyResolverExtensions
    {
        /// <summary>
        /// Retrieves <see cref="IDependencyResolver"/> for currently executed scenario.
        /// Please note that for contextual scenarios or composite steps, it is better to specify <see cref="IDependencyResolver"/> in context constructor.
        /// <para>This feature has to be enabled in <see cref="LightBddConfiguration"/> via <see cref="FrameworkConfigurationExtensions.EnableCurrentScenarioTracking"/>() prior to usage.</para>
        /// </summary>
        /// <param name="execution">Current step execution instance.</param>
        public static IDependencyResolver GetScenarioDependencyResolver(this StepExecution execution)
        {
            return ScenarioExecutionContext.Current.Get<CurrentScenarioProperty>().Scenario.DependencyResolver;
        }
    }
}
