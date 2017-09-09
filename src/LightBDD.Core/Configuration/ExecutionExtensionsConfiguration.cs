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
    //TODO: backward compat
    [DebuggerStepThrough]
    public class ExecutionExtensionsConfiguration : FeatureConfiguration, IExecutionExtensions
    {
        private readonly List<IScenarioExtension> _scenarioExtensions = new List<IScenarioExtension>();
        private readonly List<IStepExtension> _stepExtensions = new List<IStepExtension>();

        /// <summary>
        /// Collection of scenario execution extensions.
        /// </summary>
        public IEnumerable<IScenarioExtension> ScenarioExecutionExtensions => _scenarioExtensions;
        /// <summary>
        /// Collection of step execution extensions.
        /// </summary>
        public IEnumerable<IStepExtension> StepExecutionExtensions => _stepExtensions;

        /// <summary>
        /// Enables scenario execution extension of specified type.
        /// If extension is already enabled the method does nothing.
        /// </summary>
        /// <typeparam name="TScenarioExtension">Extension type to enable.</typeparam>
        /// <returns>Self.</returns>
        public ExecutionExtensionsConfiguration EnableScenarioExtension<TScenarioExtension>()
            where TScenarioExtension : IScenarioExtension, new()
            => EnableScenarioExtension(() => new TScenarioExtension());

        /// <summary>
        /// Enables scenario execution extension of specified type.
        /// If extension is already enabled the method does nothing.
        /// If extension is not enabled yet, the given <c>factory</c> parameter would be used to instantiate extension.
        /// </summary>
        /// <typeparam name="TScenarioExtension">Extension type to enable.</typeparam>
        /// <param name="factory">Factory used to instantiate the extension.</param>
        /// <returns>Self.</returns>
        public ExecutionExtensionsConfiguration EnableScenarioExtension<TScenarioExtension>(Func<TScenarioExtension> factory) where TScenarioExtension : IScenarioExtension
        {
            ThrowIfSealed();
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));

            if (!_scenarioExtensions.Any(e => e is TScenarioExtension))
                _scenarioExtensions.Add(factory());

            return this;
        }
        /// <summary>
        /// Enables step execution extension of specified type.
        /// If extension is already enabled the method does nothing.
        /// If extension is not enabled yet, the given <c>factory</c> parameter would be used to instantiate extension.
        /// </summary>
        /// <typeparam name="TStepExtension">Extension type to enable.</typeparam>
        /// <param name="factory">Factory used to instantiate the extension.</param>
        /// <returns>Self.</returns>
        public ExecutionExtensionsConfiguration EnableStepExtension<TStepExtension>(Func<TStepExtension> factory) where TStepExtension : IStepExtension
        {
            ThrowIfSealed();
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));

            if (!_stepExtensions.Any(e => e is TStepExtension))
                _stepExtensions.Add(factory());

            return this;
        }
        /// <summary>
        /// Enables scenario execution extension of specified type.
        /// If extension is already enabled the method does nothing.
        /// </summary>
        /// <typeparam name="TStepExtension">Extension type to enable.</typeparam>
        /// <returns>Self.</returns>
        public ExecutionExtensionsConfiguration EnableStepExtension<TStepExtension>()
           where TStepExtension : IStepExtension, new()
           => EnableStepExtension(() => new TStepExtension());
    }
}