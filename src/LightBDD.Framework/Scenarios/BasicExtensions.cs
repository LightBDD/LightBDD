using LightBDD.Core.Execution;
using LightBDD.Framework.Scenarios.Implementation;
using System;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Framework.Implementation;

namespace LightBDD.Framework.Scenarios
{
    /// <summary>
    /// Extensions class allowing to use basic syntax for defining and running LightBDD tests.
    /// </summary>
    public static class BasicExtensions
    {
        /// <summary>
        /// Adds steps specified by <paramref name="steps"/> to the scenario, where steps names are inferred directly from provided action names.<br/>
        /// Example usage:
        /// <code>
        /// AddSteps(
        ///         Given_numbers_5_and_8,
        ///         When_I_add_them,
        ///         I_should_receive_number_13)
        /// </code>
        /// Expected step signature: <code>void Given_numbers_5_and_8() { /* ... */ }</code>
        /// </summary>
        /// <param name="builder">Builder.</param>
        /// <param name="steps">Steps to add, like: <c>AddSteps(Given_numbers_5_and_8, When_I_add_them, I_should_receive_number_13)</c></param>
        public static IScenarioRunner<NoContext> AddSteps(this IScenarioBuilder<NoContext> builder, params Action[] steps)
        {
            var integration = builder.Integrate();
            integration.Core.AddSteps(steps.Select(BasicStepCompiler.ToSynchronousStep));
            return integration;
        }

        /// <summary>
        /// Adds asynchronous steps specified by <paramref name="steps"/> to the scenario, where steps names are inferred directly from provided action names.<br/>
        /// Example usage:
        /// <code>
        /// AddAsyncSteps(
        ///         Given_numbers_5_and_8,
        ///         When_I_add_them,
        ///         I_should_receive_number_13)
        /// </code>
        /// Expected step signature: <code>Task Given_numbers_5_and_8() { /* ... */ }</code>
        /// </summary>
        /// <param name="builder">Builder.</param>
        /// <param name="steps">Steps to add, like: <c>AddAsyncSteps(Given_numbers_5_and_8, When_I_add_them, I_should_receive_number_13)</c></param>
        public static IScenarioRunner<NoContext> AddAsyncSteps(this IScenarioBuilder<NoContext> builder, params Func<Task>[] steps)
        {
            var integration = builder.Integrate();
            integration.Core.AddSteps(steps.Select(BasicStepCompiler.ToAsynchronousStep));
            return integration;
        }

        /// <summary>
        /// Adds steps specified by <paramref name="steps"/> to the composite step, where steps names are inferred directly from provided action names.<br/>
        /// Example usage:
        /// <code>
        /// AddSteps(
        ///         Given_numbers_5_and_8,
        ///         When_I_add_them,
        ///         I_should_receive_number_13)
        /// </code>
        /// Expected step signature: <code>void Given_numbers_5_and_8() { /* ... */ }</code>
        /// </summary>
        /// <param name="builder">Builder.</param>
        /// <param name="steps">Steps to add, like: <c>AddSteps(Given_numbers_5_and_8, When_I_add_them, I_should_receive_number_13)</c></param>
        public static ICompositeStepBuilder<NoContext> AddSteps(this ICompositeStepBuilder<NoContext> builder, params Action[] steps)
        {
            builder.Integrate().AddSteps(steps.Select(BasicStepCompiler.ToSynchronousStep));
            return builder;
        }

        /// <summary>
        /// Adds asynchronous steps specified by <paramref name="steps"/> to the composite step, where steps names are inferred directly from provided action names.<br/>
        /// Example usage:
        /// <code>
        /// AddAsyncSteps(
        ///         Given_numbers_5_and_8,
        ///         When_I_add_them,
        ///         I_should_receive_number_13)
        /// </code>
        /// Expected step signature: <code>Task Given_numbers_5_and_8() { /* ... */ }</code>
        /// </summary>
        /// <param name="builder">Builder.</param>
        /// <param name="steps">Steps to add, like: <c>AddAsyncSteps(Given_numbers_5_and_8, When_I_add_them, I_should_receive_number_13)</c></param>
        public static ICompositeStepBuilder<NoContext> AddAsyncSteps(this ICompositeStepBuilder<NoContext> builder, params Func<Task>[] steps)
        {
            builder.Integrate().AddSteps(steps.Select(BasicStepCompiler.ToAsynchronousStep));
            return builder;
        }

        /// <summary>
        /// Runs test scenario by executing given steps in specified order.<br/>
        /// If given step throws, other are not executed.<br/>
        /// The scenario name is determined from the current scenario test method.<br/>
        /// Scenario labels are determined from <see cref="LabelAttribute"/> attributes applied on scenario method.<br/>
        /// The step name is determined from corresponding action name.<br/>
        /// Example usage:
        /// <code>
        /// Runner.RunScenario(
        ///         Given_numbers_5_and_8,
        ///         When_I_add_them,
        ///         I_should_receive_number_13);
        /// </code>
        /// Expected step signature: <code>void Given_numbers_5_and_8() { /* ... */ }</code>
        /// </summary>
        /// <param name="runner">Runner.</param>
        /// <param name="steps">List of steps to execute in order.</param>
        public static void RunScenario(this IBddRunner runner, params Action[] steps)
        {
            try
            {
                runner
                    .AddSteps(steps)
                    .Integrate().Core
                    .RunSync();
            }
            catch (ScenarioExecutionException e)
            {
                e.GetOriginal().Throw();
            }
        }

        /// <summary>
        /// Runs test scenario by executing given steps in specified order.<br/>
        /// If given step throws, other are not executed.<br/>
        /// The scenario name is determined from the current scenario test method.<br/>
        /// Scenario labels are determined from <see cref="LabelAttribute"/> attributes applied on scenario method.<br/>
        /// The step name is determined from corresponding action name.<br/>
        /// Example usage:
        /// <code>
        /// Runner.RunScenario(
        ///         Given_numbers_5_and_8,
        ///         When_I_add_them,
        ///         I_should_receive_number_13);
        /// </code>
        /// Expected step signature: <code>void Given_numbers_5_and_8() { /* ... */ }</code>
        /// </summary>
        /// <param name="runner">Runner.</param>
        /// <param name="steps">List of steps to execute in order.</param>
        public static async Task RunScenarioAsync(this IBddRunner runner, params Action[] steps)
        {
            try
            {
                await runner
                    .AddSteps(steps)
                    .Integrate().Core
                    .RunAsync();
            }
            catch (ScenarioExecutionException e)
            {
                e.GetOriginal().Throw();
            }
        }

        /// <summary>
        /// Runs asynchronous test scenario by executing given steps in specified order.<br/>
        /// If given step throws, other are not executed.<br/>
        /// The scenario name is determined from the current scenario test method.<br/>
        /// Scenario labels are determined from <see cref="LabelAttribute"/> attributes applied on scenario method.<br/>
        /// The step name is determined from corresponding action name.<br/>
        /// Example usage:
        /// <code>
        /// await Runner.RunScenarioAsync(
        ///         Given_numbers_5_and_8,
        ///         When_I_add_them,
        ///         I_should_receive_number_13);
        /// </code>
        /// Expected step signature:
        /// <code>
        /// async Task Given_numbers_5_and_8() { /* ... */ }
        /// </code>
        /// </summary>
        /// <remarks>This is an asynchronous method and should be awaited.</remarks>
        /// <param name="runner">Runner.</param>
        /// <param name="steps">List of steps to execute in order.</param>
        public static async Task RunScenarioAsync(this IBddRunner runner, params Func<Task>[] steps)
        {
            try
            {
                await runner
                    .AddAsyncSteps(steps)
                    .Integrate().Core
                    .RunAsync();
            }
            catch (ScenarioExecutionException e)
            {
                e.GetOriginal().Throw();
            }
        }
    }
}
