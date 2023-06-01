using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Core.Execution.Implementation;
using LightBDD.Core.Extensibility.Execution;

namespace LightBDD.Core.Configuration
{
    /// <summary>
    /// Execution extensions configuration allowing to enable decorators for scenarios and steps.
    /// </summary>
    public class ExecutionExtensionsConfiguration : FeatureConfiguration, IExecutionExtensions
    {
        internal GlobalInitializer GlobalInitializer { get; } = new();

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

        /// <summary>
        /// Registers global set up and optional global cleanup methods for given dependency type configured on the DI.<br/>
        /// The <paramref name="setUp"/> method will be executed once, before any tests are run. If multiple set up methods are registered, they will be executed in the registration order.<br/>
        /// If <paramref name="cleanUp"/> method is specified, it will be executed once after all tests are run. The clean up methods are executed in reverse registration order, i.e. last registered one will be executed as first.
        /// </summary>
        /// <typeparam name="TDependency">Dependency type, that is registered in the DI container.</typeparam>
        /// <param name="setUp">Set up method</param>
        /// <param name="cleanUp">Clean up method</param>
        /// <returns></returns>
        public ExecutionExtensionsConfiguration RegisterGlobalSetUp<TDependency>(Func<TDependency, Task> setUp, Func<TDependency, Task> cleanUp = null)
        {
            ThrowIfSealed();
            GlobalInitializer.RegisterSetUp(setUp);
            if (cleanUp != null)
                GlobalInitializer.RegisterCleanUp(cleanUp);
            return this;
        }

        /// <summary>
        /// Registers global cleanup method for given dependency type configured on the DI.<br/>
        /// The <paramref name="cleanUp"/> method will be executed once after all tests are run. The clean up methods are executed in reverse registration order, i.e. last registered one will be executed as first.
        /// </summary>
        /// <typeparam name="TDependency">Dependency type, that is registered in the DI container.</typeparam>
        /// <param name="cleanUp">Clean up method</param>
        /// <returns></returns>
        public ExecutionExtensionsConfiguration RegisterGlobalCleanUp<TDependency>(Func<TDependency, Task> cleanUp)
        {
            ThrowIfSealed();
            GlobalInitializer.RegisterCleanUp(cleanUp);
            return this;
        }

        /// <summary>
        /// Registers global set up and optional global cleanup methods.<br/>
        /// The <paramref name="setUp"/> method will be executed once, before any tests are run. If multiple set up methods are registered, they will be executed in the registration order.<br/>
        /// If <paramref name="cleanUp"/> method is specified, it will be executed once after all tests are run. The clean up methods are executed in reverse registration order.
        /// </summary>
        /// <param name="setUp">Set up method</param>
        /// <param name="cleanUp">Clean up method</param>
        /// <returns></returns>
        public ExecutionExtensionsConfiguration RegisterGlobalSetUp(Func<Task> setUp, Func<Task> cleanUp = null)
        {
            ThrowIfSealed();
            GlobalInitializer.RegisterSetUp(setUp);
            return this;
        }

        /// <summary>
        /// Registers global cleanup method.<br/>
        /// The <paramref name="cleanUp"/> method will be executed once after all tests are run. The clean up methods are executed in reverse registration order, i.e. last registered one will be executed as first.
        /// </summary>
        /// <param name="cleanUp">Clean up method</param>
        /// <returns></returns>
        public ExecutionExtensionsConfiguration RegisterGlobalCleanUp(Func<Task> cleanUp)
        {
            ThrowIfSealed();
            GlobalInitializer.RegisterCleanUp(cleanUp);
            return this;
        }
    }
}