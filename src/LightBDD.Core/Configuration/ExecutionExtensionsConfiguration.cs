using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using LightBDD.Core.Extensibility.Execution;

namespace LightBDD.Core.Configuration
{
    /// <summary>
    /// Execution extensions configuration allowing to enable LightBDD extensions.
    /// </summary>
    [DebuggerStepThrough]
    public class ExecutionExtensionsConfiguration : FeatureConfiguration, IExecutionExtensions
    {
        private readonly List<IScenarioExecutionExtension> _scenarioExtensions = new List<IScenarioExecutionExtension>();
        private readonly List<IStepExecutionExtension> _stepExtensions = new List<IStepExecutionExtension>();

        /// <summary>
        /// Collection of scenario execution extensions.
        /// </summary>
        public IEnumerable<IScenarioExecutionExtension> ScenarioExecutionExtensions => _scenarioExtensions;
        /// <summary>
        /// Collection of step execution extensions.
        /// </summary>
        public IEnumerable<IStepExecutionExtension> StepExecutionExtensions => _stepExtensions;

        /// <summary>
        /// Enables scenario execution extension of specified type.
        /// If extension is already enabled the method does nothing.
        /// </summary>
        /// <typeparam name="TScenarioExecutionExtension">Extension type to enable.</typeparam>
        /// <returns>Self.</returns>
        public ExecutionExtensionsConfiguration EnableScenarioExtension<TScenarioExecutionExtension>()
            where TScenarioExecutionExtension : IScenarioExecutionExtension, new()
            => EnableScenarioExtension(() => new TScenarioExecutionExtension());

        /// <summary>
        /// Enables scenario execution extension of specified type.
        /// If extension is already enabled the method does nothing.
        /// If extension is not enabled yet, the given <c>factory</c> parameter would be used to instantiate extension.
        /// </summary>
        /// <typeparam name="TScenarioExecutionExtension">Extension type to enable.</typeparam>
        /// <param name="factory">Factory used to instantiate the extension.</param>
        /// <returns>Self.</returns>
        public ExecutionExtensionsConfiguration EnableScenarioExtension<TScenarioExecutionExtension>(Func<TScenarioExecutionExtension> factory) where TScenarioExecutionExtension : IScenarioExecutionExtension
        {
            ThrowIfSealed();
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));

            if (!_scenarioExtensions.Any(e => e is TScenarioExecutionExtension))
                _scenarioExtensions.Add(factory());

            return this;
        }
        /// <summary>
        /// Enables step execution extension of specified type.
        /// If extension is already enabled the method does nothing.
        /// If extension is not enabled yet, the given <c>factory</c> parameter would be used to instantiate extension.
        /// </summary>
        /// <typeparam name="TStepExecutionExtension">Extension type to enable.</typeparam>
        /// <param name="factory">Factory used to instantiate the extension.</param>
        /// <returns>Self.</returns>
        public ExecutionExtensionsConfiguration EnableStepExtension<TStepExecutionExtension>(Func<TStepExecutionExtension> factory) where TStepExecutionExtension : IStepExecutionExtension
        {
            ThrowIfSealed();
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));

            if (!_stepExtensions.Any(e => e is TStepExecutionExtension))
                _stepExtensions.Add(factory());

            return this;
        }
        /// <summary>
        /// Enables scenario execution extension of specified type.
        /// If extension is already enabled the method does nothing.
        /// </summary>
        /// <typeparam name="TStepExecutionExtension">Extension type to enable.</typeparam>
        /// <returns>Self.</returns>
        public ExecutionExtensionsConfiguration EnableStepExtension<TStepExecutionExtension>()
           where TStepExecutionExtension : IStepExecutionExtension, new()
           => EnableStepExtension(() => new TStepExecutionExtension());
    }
}