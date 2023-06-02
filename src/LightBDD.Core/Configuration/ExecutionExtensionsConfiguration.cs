using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Core.Execution;
using LightBDD.Core.Execution.Implementation.GlobalInitialization;
using LightBDD.Core.Extensibility.Execution;

namespace LightBDD.Core.Configuration
{
    /// <summary>
    /// Execution extensions configuration allowing to enable decorators for scenarios and steps.
    /// </summary>
    public class ExecutionExtensionsConfiguration : FeatureConfiguration, IExecutionExtensions
    {
        internal GlobalSetUp GlobalSetUp { get; } = new();

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
        /// Registers <typeparam name="TDependency"/> to for global set up before any tests are run and clean up after all tests execution.<br/>
        /// The <seealso cref="IGlobalResourceSetUp.SetUpAsync"/> method will be executed once, before any tests are run. If multiple set up functions are registered, they will be executed in the registration order.<br/>
        /// The <seealso cref="IGlobalResourceSetUp.CleanUpAsync"/> it will be executed once after all tests are run, but only if <seealso cref="IGlobalResourceSetUp.SetUpAsync"/> has been successfully run. The clean up methods are executed in reverse registration order, i.e. last registered one will be executed as first.<br/>
        /// Please note that the <typeparam name="TDependency" /> is resolved independently by set up and clean up methods, thus needs to be registered as singleton or scoped if the same instance is expected.
        /// </summary>
        /// <typeparam name="TDependency">Dependency type, that is registered in the DI container.</typeparam>
        public ExecutionExtensionsConfiguration RegisterGlobalSetUp<TDependency>() where TDependency : IGlobalResourceSetUp
        {
            ThrowIfSealed();
            GlobalSetUp.RegisterResource<TDependency>();
            return this;
        }

        /// <summary>
        /// Registers global set up and optional global cleanup methods.<br/>
        /// The <paramref name="setUp"/> delegate will be executed once, before any tests are run. If multiple set up methods are registered, they will be executed in the registration order.<br/>
        /// If <paramref name="cleanUp"/> delegate is specified, it will be executed once after all tests are run, but only if <paramref cref="setUp"/> has been successfully run. The clean up methods are executed in reverse registration order.
        /// </summary>
        /// <param name="activityName">Name of the set up activity</param>
        /// <param name="setUp">Set up method</param>
        /// <param name="cleanUp">Clean up method</param>
        /// <returns></returns>
        public ExecutionExtensionsConfiguration RegisterGlobalSetUp(string activityName, Func<Task> setUp, Func<Task> cleanUp = null)
        {
            ThrowIfSealed();
            GlobalSetUp.RegisterActivity(activityName, setUp, cleanUp);
            return this;
        }

        /// <summary>
        /// Registers global set up and optional global cleanup methods.<br/>
        /// The <paramref name="setUp"/> delegate will be executed once, before any tests are run. If multiple set up methods are registered, they will be executed in the registration order.<br/>
        /// If <paramref name="cleanUp"/> delegate is specified, it will be executed once after all tests are run, but only if <paramref cref="setUp"/> has been successfully run. The clean up methods are executed in reverse registration order.
        /// </summary>
        /// <param name="activityName">Name of the set up activity</param>
        /// <param name="setUp">Set up method</param>
        /// <param name="cleanUp">Clean up method</param>
        /// <returns></returns>
        public ExecutionExtensionsConfiguration RegisterGlobalSetUp(string activityName, Action setUp, Action cleanUp = null)
        {
            ThrowIfSealed();
            if (setUp == null)
                throw new ArgumentNullException(nameof(setUp));
            GlobalSetUp.RegisterActivity(activityName,
                () =>
                {
                    setUp();
                    return Task.CompletedTask;
                },
                () =>
                {
                    cleanUp?.Invoke();
                    return Task.CompletedTask;
                });
            return this;
        }

        /// <summary>
        /// Registers global cleanup method.<br/>
        /// The <paramref name="cleanUp"/> delegate will be executed once after all tests are run. The clean up methods are executed in reverse registration order, i.e. last registered one will be executed as first.
        /// </summary>
        /// <param name="activityName">Name of the clean up activity</param>
        /// <param name="cleanUp">Clean up method</param>
        /// <returns></returns>
        public ExecutionExtensionsConfiguration RegisterGlobalCleanUp(string activityName, Func<Task> cleanUp)
        {
            ThrowIfSealed();
            if (cleanUp == null)
                throw new ArgumentNullException(nameof(cleanUp));
            GlobalSetUp.RegisterActivity(activityName, null, cleanUp);
            return this;
        }

        /// <summary>
        /// Registers global cleanup method.<br/>
        /// The <paramref name="cleanUp"/> delegate will be executed once after all tests are run. The clean up methods are executed in reverse registration order, i.e. last registered one will be executed as first.
        /// </summary>
        /// <param name="activityName">Name of the clean up activity</param>
        /// <param name="cleanUp">Clean up method</param>
        /// <returns></returns>
        public ExecutionExtensionsConfiguration RegisterGlobalCleanUp(string activityName, Action cleanUp)
        {
            ThrowIfSealed();
            if (cleanUp == null)
                throw new ArgumentNullException(nameof(cleanUp));
            GlobalSetUp.RegisterActivity(activityName,
                null,
                () =>
                {
                    cleanUp.Invoke();
                    return Task.CompletedTask;
                });
            return this;
        }
    }
}