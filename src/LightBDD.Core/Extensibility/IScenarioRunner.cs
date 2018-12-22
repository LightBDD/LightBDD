using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LightBDD.Core.Dependencies;
using LightBDD.Core.Extensibility.Execution;

namespace LightBDD.Core.Extensibility
{
    /// <summary>
    /// <see cref="IScenarioRunner"/> integration interface allowing to construct scenario for execution.
    /// The interface is dedicated for projects extending LightBDD with user friendly API for running scenarios - it should not be used directly by regular LightBDD users.
    /// </summary>
    public interface IScenarioRunner
    {
        /// <summary>
        /// Configures steps to be executed with scenario.
        /// </summary>
        /// <param name="steps">Steps to execute.</param>
        /// <returns>Self.</returns>
        /// <exception cref="ArgumentNullException">Throws when <paramref name="steps"/> are <c>null</c>.</exception>
        IScenarioRunner WithSteps(IEnumerable<StepDescriptor> steps);
        /// <summary>
        /// Configures scenario details with values inferred by <see cref="IMetadataProvider"/>.
        /// </summary>
        /// <returns>Self.</returns>
        IScenarioRunner WithCapturedScenarioDetails();
        /// <summary>
        /// Configures scenario with labels.
        /// </summary>
        /// <param name="labels">Labels to set.</param>
        /// <returns>Self.</returns>
        /// <exception cref="ArgumentNullException">Throws when <paramref name="labels"/> are <c>null</c>.</exception>
        IScenarioRunner WithLabels(string[] labels);
        /// <summary>
        /// Configures scenario with categories.
        /// </summary>
        /// <param name="categories">Categories to set.</param>
        /// <returns>Self.</returns>
        /// <exception cref="ArgumentNullException">Throws when <paramref name="categories"/> are <c>null</c>.</exception>
        IScenarioRunner WithCategories(string[] categories);
        /// <summary>
        /// Configures scenario with name.
        /// </summary>
        /// <param name="name">Name to set.</param>
        /// <returns>Self.</returns>
        /// <exception cref="ArgumentException">Throws when <paramref name="name"/> is <c>null</c> or empty.</exception>
        IScenarioRunner WithName(string name);

        /// <summary>
        /// Configures scenario to be executed with context provided by <paramref name="contextProvider"/>.
        /// </summary>
        /// <param name="contextProvider">Context provider function.</param>
        /// <param name="takeOwnership">Specifies if scenario runner should take ownership of the context instance. If set to true and context instance implements <see cref="IDisposable"/>, it will be disposed after scenario finish.</param>
        /// <returns>Self.</returns>
        /// <exception cref="ArgumentNullException">Throws when <paramref name="contextProvider"/> is <c>null</c>.</exception>
        IScenarioRunner WithContext(Func<object> contextProvider, bool takeOwnership);

        /// <summary>
        /// Configures scenario to be executed with context provided by <paramref name="contextProvider"/>.
        /// </summary>
        /// <param name="contextProvider">Context provider function.</param>
        /// <param name="scopeConfigurator"></param>
        /// <returns>Self.</returns>
        /// <exception cref="ArgumentNullException">Throws when <paramref name="contextProvider"/> is <c>null</c>.</exception>
        IScenarioRunner WithContext(Func<IDependencyResolver, object> contextProvider, Action<ContainerConfigurator> scopeConfigurator = null);
        /// <summary>
        /// Configures scenario to be executed with additional decorators provided by <paramref name="scenarioDecorators"/>.
        /// </summary>
        /// <param name="scenarioDecorators">Decorators to use.</param>
        /// <returns>Self.</returns>
        IScenarioRunner WithScenarioDecorators(IEnumerable<IScenarioDecorator> scenarioDecorators);

        /// <summary>
        /// Runs scenario synchronously - guaranteeing that scenario and all steps will execute on the same, calling thread.
        /// Before scenario is run, a validation is done if scenario is properly configured (i.e. name is defined and there is defined at least one step to execute).
        /// It is expected that only such scenarios can run synchronously whose steps returns completed <see cref="Task"/>. If any step method returns pending task, an <see cref="InvalidOperationException"/> exception will be thrown.<br/>
        /// Any exceptions thrown in scenario steps will be wrapped in <see cref="ScenarioExecutionException"/>. Code calling this method can rethrow the original exception by calling <code>ex.GetOriginal().Throw()</code>
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when name or steps are not defined before scenario run. Throws also if any step method returns pending task.</exception>
        /// <exception cref="ScenarioExecutionException">Thrown when one of steps throws exception. The original exception is accessible with <see cref="Exception.InnerException"/> property and can be rethrown by calling <code>ex.GetOriginal().Throw()</code></exception>
        void RunScenario();
        /// <summary>
        /// Runs scenario asynchronously and returns task representing it.
        /// Before scenario is run, a validation is done if scenario is properly configured (i.e. name is defined and there is defined at least one step to execute).
        /// Any exceptions thrown in scenario steps will be wrapped in <see cref="ScenarioExecutionException"/>. Code calling this method can rethrow the original exception by calling <code>ex.GetOriginal().Throw()</code>
        /// </summary>
        /// <returns>Scenario task.</returns>
        /// <exception cref="InvalidOperationException">Throws when name or steps are not defined.</exception>
        /// <exception cref="ScenarioExecutionException">Thrown when one of steps throws exception. The original exception is accessible with <see cref="Exception.InnerException"/> property and can be rethrown by calling <code>ex.GetOriginal().Throw()</code></exception>
        Task RunScenarioAsync();
    }
}