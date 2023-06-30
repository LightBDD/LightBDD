using System;
using System.Collections.Concurrent;
using System.Threading;
using LightBDD.Core.Execution;
using LightBDD.Core.ExecutionContext.Implementation;

namespace LightBDD.Core.ExecutionContext
{
    /// <summary>
    /// Scenario execution context class allowing to store and retrieve scenario properties that would be available for all steps and tasks executed within the scenario.
    /// </summary>
    public sealed class ScenarioExecutionContext
    {
        private static readonly AsyncLocal<ScenarioExecutionContext> CurrentContext = new();
        private readonly ConcurrentDictionary<Type, IContextProperty> _properties = new();

        /// <summary>
        /// Provides property value of <typeparamref name="TProperty"/> type that is stored in scenario context.
        /// If such property does not exists yet, a new instance will be registered in context and returned.
        /// </summary>
        /// <typeparam name="TProperty">Property type to retrieve.</typeparam>
        /// <returns>Property object.</returns>
        public TProperty Get<TProperty>() where TProperty : IContextProperty, new()
        {
            return (TProperty)_properties.GetOrAdd(typeof(TProperty), t => new TProperty());
        }

        /// <summary>
        /// Returns current scenario execution context.
        /// <exception cref="InvalidOperationException">Throws when <see cref="Current"/> property is accessed from outside of scenario method or if <see cref="ScenarioExecutionContext"/> feature is not enabled.</exception>
        /// </summary>
        public static ScenarioExecutionContext Current
        {
            get
            {
                var ctx = CurrentContext.Value;
                if (ctx != null)
                    return ctx;
                throw new InvalidOperationException($"Current task is not executing any scenarios. Ensure that operation accessing {nameof(ScenarioExecutionContext)} is called from task running scenario.");
            }
            set => CurrentContext.Value = value;
        }

        /// <summary>
        /// Returns currently executed step.
        /// <exception cref="InvalidOperationException">Thrown if no step is executed by current task.</exception>
        /// </summary>
        public static IStep CurrentStep => Current.Get<CurrentStepProperty>().Step;

        /// <summary>
        /// Returns currently executed scenario.
        /// <exception cref="InvalidOperationException">Thrown if no scenario is executed by current task or if scenario initialization is not complete.</exception>
        /// </summary>
        public static IScenario CurrentScenario => Current.Get<CurrentScenarioProperty>().Scenario ?? throw new InvalidOperationException("The current task does not run any initialized scenario. Ensure that feature is used within task running fully initialized scenario.");

        /// <summary>
        /// Returns currently executed scenario fixture object if present or <c>null</c> if no scenario is currently executed.<br/>
        /// <exception cref="InvalidOperationException">Thrown if fixture object is present but not assignable to <typeparam name="TFixture"></typeparam>.</exception>
        /// </summary>
        public static TFixture GetCurrentScenarioFixtureIfPresent<TFixture>() where TFixture : class
        {
            var fixture = CurrentContext.Value?.Get<CurrentScenarioProperty>().Fixture;

            if (fixture == null)
                return null;

            return fixture as TFixture
                   ?? throw new InvalidOperationException($"Expected fixture of type '{typeof(TFixture)}' while got '{fixture.GetType()}'.");
        }
    }
}