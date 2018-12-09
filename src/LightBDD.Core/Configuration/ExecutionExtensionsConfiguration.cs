using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using LightBDD.Core.Extensibility.Execution;

namespace LightBDD.Core.Configuration
{
    /// <summary>
    /// Execution extensions configuration allowing to enable decorators for scenarios and steps.
    /// </summary>
    [DebuggerStepThrough]
    public class ExecutionExtensionsConfiguration : FeatureConfiguration, IExecutionExtensions
    {
        private readonly List<IScenarioDecorator> _scenarioExtensions = new List<IScenarioDecorator>();
        private readonly List<IStepDecorator> _stepExtensions = new List<IStepDecorator>();

        /// <summary>
        /// Collection of scenario execution extensions.
        /// </summary>
        public IEnumerable<IScenarioDecorator> ScenarioDecorators => _scenarioExtensions;
        /// <summary>
        /// Collection of step execution extensions.
        /// </summary>
        public IEnumerable<IStepDecorator> StepDecorators => _stepExtensions;

        /// <summary>
        /// Enables scenario decorator of specified type.
        /// If decorator is already enabled the method does nothing.
        /// </summary>
        /// <typeparam name="TScenarioDecorator">Decorator type to enable.</typeparam>
        /// <returns>Self.</returns>
        public ExecutionExtensionsConfiguration EnableScenarioDecorator<TScenarioDecorator>()
            where TScenarioDecorator : IScenarioDecorator, new()
        {
            return EnableScenarioDecorator(() => new TScenarioDecorator());
        }

        /// <summary>
        /// Enables scenario decorator of specified type.
        /// If decorator is already enabled the method does nothing.
        /// If decorator is not enabled yet, the given <c>factory</c> parameter would be used to instantiate it.
        /// </summary>
        /// <typeparam name="TScenarioDecorator">Decorator type to enable.</typeparam>
        /// <param name="factory">Factory used to instantiate decorator.</param>
        /// <returns>Self.</returns>
        public ExecutionExtensionsConfiguration EnableScenarioDecorator<TScenarioDecorator>(Func<TScenarioDecorator> factory)
            where TScenarioDecorator : IScenarioDecorator
        {
            ThrowIfSealed();
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));

            if (!_scenarioExtensions.Any(e => e is TScenarioDecorator))
                _scenarioExtensions.Add(factory());

            return this;
        }
        /// <summary>
        /// Enables step decorator of specified type.
        /// If decorator is already enabled the method does nothing.
        /// If decorator is not enabled yet, the given <c>factory</c> parameter would be used to instantiate it.
        /// </summary>
        /// <typeparam name="TStepDecorator">Decorator type to enable.</typeparam>
        /// <param name="factory">Factory used to instantiate decorator.</param>
        /// <returns>Self.</returns>
        public ExecutionExtensionsConfiguration EnableStepDecorator<TStepDecorator>(Func<TStepDecorator> factory)
            where TStepDecorator : IStepDecorator
        {
            ThrowIfSealed();
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));

            if (!_stepExtensions.Any(e => e is TStepDecorator))
                _stepExtensions.Add(factory());

            return this;
        }
        /// <summary>
        /// Enables scenario decorator of specified type.
        /// If decorator is already enabled the method does nothing.
        /// </summary>
        /// <typeparam name="TStepDecorator">Decorator type to enable.</typeparam>
        /// <returns>Self.</returns>
        public ExecutionExtensionsConfiguration EnableStepDecorator<TStepDecorator>()
           where TStepDecorator : IStepDecorator, new()
        {
            return EnableStepDecorator(() => new TStepDecorator());
        }
    }
}