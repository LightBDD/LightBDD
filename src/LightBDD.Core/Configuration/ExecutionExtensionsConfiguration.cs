using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Core.Execution;
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

        /// <inheritdoc />
        [Obsolete("Use ScenarioDecorators instead", true)]
        public IEnumerable<IScenarioExecutionExtension> ScenarioExecutionExtensions
            => throw new NotSupportedException();

        /// <inheritdoc />
        [Obsolete("Use StepDecorators instead", true)]
        public IEnumerable<IStepExecutionExtension> StepExecutionExtensions
            => throw new NotSupportedException();

        /// <summary>
        /// Enables scenario decorator of specified type.
        /// If decorator is already enabled the method does nothing.
        /// </summary>
        /// <typeparam name="TScenarioDecorator">Decorator type to enable.</typeparam>
        /// <returns>Self.</returns>
        public ExecutionExtensionsConfiguration EnableScenarioDecorator<TScenarioDecorator>()
            where TScenarioDecorator : IScenarioDecorator, new()
            => EnableScenarioDecorator(() => new TScenarioDecorator());

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
           => EnableStepDecorator(() => new TStepDecorator());

        /// <summary>
        /// Enables scenario execution extension of specified type.
        /// If extension is already enabled the method does nothing.
        /// </summary>
        /// <typeparam name="TScenarioExecutionExtension">Extension type to enable.</typeparam>
        /// <returns>Self.</returns>
        [Obsolete("Use EnableScenarioDecorator instead", true)]
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
        [Obsolete("Use EnableScenarioDecorator instead", true)]
        public ExecutionExtensionsConfiguration EnableScenarioExtension<TScenarioExecutionExtension>(
            Func<TScenarioExecutionExtension> factory)
            where TScenarioExecutionExtension : IScenarioExecutionExtension
            => EnableScenarioDecorator(() => new ScenarioExtensionToDecoratorConverter(factory()));

        /// <summary>
        /// Enables step execution extension of specified type.
        /// If extension is already enabled the method does nothing.
        /// If extension is not enabled yet, the given <c>factory</c> parameter would be used to instantiate extension.
        /// </summary>
        /// <typeparam name="TStepExecutionExtension">Extension type to enable.</typeparam>
        /// <param name="factory">Factory used to instantiate the extension.</param>
        /// <returns>Self.</returns>
        [Obsolete("Use EnableStepDecorator instead", true)]
        public ExecutionExtensionsConfiguration EnableStepExtension<TStepExecutionExtension>(
            Func<TStepExecutionExtension> factory)
            where TStepExecutionExtension : IStepExecutionExtension
            => EnableStepDecorator(factory);

        /// <summary>
        /// Enables scenario execution extension of specified type.
        /// If extension is already enabled the method does nothing.
        /// </summary>
        /// <typeparam name="TStepExecutionExtension">Extension type to enable.</typeparam>
        /// <returns>Self.</returns>
        [Obsolete("Use EnableStepDecorator instead", true)]
        public ExecutionExtensionsConfiguration EnableStepExtension<TStepExecutionExtension>()
           where TStepExecutionExtension : IStepExecutionExtension, new()
           => EnableStepExtension(() => new TStepExecutionExtension());

        [DebuggerStepThrough]
        [Obsolete]
        private class ScenarioExtensionToDecoratorConverter : IScenarioDecorator
        {
            private readonly IScenarioExecutionExtension _extension;

            public ScenarioExtensionToDecoratorConverter(IScenarioExecutionExtension extension)
            {
                _extension = extension;
            }

            public async Task ExecuteAsync(IScenario scenario, Func<Task> scenarioInvocation)
            {
                await _extension.ExecuteAsync(scenario.Info, scenarioInvocation);
            }
        }
    }


}