#nullable enable
using LightBDD.Core.Dependencies;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Metadata;

namespace LightBDD.Core.Execution
{
    /// <summary>
    /// Interface describing current scenario.
    /// </summary>
    public interface IScenario : IExecutable
    {
        /// <summary>
        /// Scenario information details.
        /// </summary>
        IScenarioInfo Info { get; }
        /// <summary>
        /// Returns the dependency resolver used by this scenario.
        /// </summary>
        IDependencyResolver DependencyResolver { get; }
        /// <summary>
        /// Returns the context used by this scenario (or null if none were provided).
        /// </summary>
        object? Context { get; }
        /// <summary>
        /// Returns feature fixture on which the scenario is executed.
        /// </summary>
        object Fixture { get; }

        /// <summary>
        /// Returns scenario descriptor used to instantiate this scenario.
        /// </summary>
        ScenarioDescriptor? Descriptor { get; }
    }
}