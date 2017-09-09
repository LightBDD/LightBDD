using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        /// <returns>Self.</returns>
        /// <exception cref="ArgumentNullException">Throws when <paramref name="contextProvider"/> is <c>null</c>.</exception>
        IScenarioRunner WithContext(Func<object> contextProvider);
        /// <summary>
        /// Configures scenario to be executed with additional execution extensions provided by <paramref name="scenarioExecutionExtensions"/>.
        /// </summary>
        /// <param name="scenarioExecutionExtensions">Execution extensions to use.</param>
        /// <returns>Self.</returns>
        IScenarioRunner WithScenarioExecutionExtensions(IEnumerable<IScenarioExtension> scenarioExecutionExtensions);
        /// <summary>
        /// Runs scenario asynchronously and returns task representing it.
        /// Before scenario is run, a validation is done if scenario is properly configured (i.e. name is defined and there is defined at least one step to execute).
        /// </summary>
        /// <returns>Scenario task.</returns>
        /// <exception cref="InvalidOperationException">Throws when name or steps are not defined.</exception>
        Task RunAsynchronously();
        /// <summary>
        /// Runs scenario synchronously - guaranteeing that scenario and all steps will execute on the same, calling thread.
        /// Before scenario is run, a validation is done if scenario is properly configured (i.e. name is defined and there is defined at least one step to execute).
        /// It is expected that only such scenarios can run synchronously whose steps returns completed <see cref="Task"/>. If any step method returns pending task, an <see cref="InvalidOperationException"/> exception will be thrown.
        /// </summary>
        /// <exception cref="InvalidOperationException">Throws when name or steps are not defined before scenario run. Throws also if any step method returns pending task.</exception>
        void RunSynchronously();
    }
}