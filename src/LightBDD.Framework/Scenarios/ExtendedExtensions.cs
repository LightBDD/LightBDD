using LightBDD.Core.Execution;
using LightBDD.Framework.Scenarios.Implementation;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using LightBDD.Framework.Implementation;

namespace LightBDD.Framework.Scenarios
{
    /// <summary>
    /// Extensions class allowing to use extended syntax for defining and running LightBDD tests.
    /// </summary>
    public static class ExtendedExtensions
    {
        /// <summary>
        /// Adds steps specified by <paramref name="steps"/> parameter to the scenario.<br/>
        /// The step name is determined from lambda parameter name reflecting action type keyword, corresponding action name and passed list of parameters to called method.<br/>
        /// If scenario is defined with context, the context instance is provided with lambda parameter.<br/>
        /// Example usage:
        /// <code>
        /// AddSteps(
        ///         _ => Given_numbers(5, 8),
        ///         _ => When_I_add_them(),
        ///         _ => I_should_receive_number(13))
        /// </code>
        /// Expected step signature: <code>void Given_numbers(params int[] numbers) { /* ... */ }</code>
        /// </summary>
        /// <param name="builder">Builder.</param>
        /// <param name="steps">Steps to add, like: <c>AddSteps(_ => Given_numbers(5, 8), _ => When_I_add_them(), _ => I_should_receive_number(13))</c></param>
        public static IScenarioRunner<TContext> AddSteps<TContext>(this IScenarioBuilder<TContext> builder, params Expression<Action<TContext>>[] steps)
        {
            var integration = builder.Integrate();
            var compiler = new ExtendedStepCompiler<TContext>(integration.Core.Configuration);
            integration.Core.AddSteps(steps.Select(compiler.ToStep));
            return integration;
        }

        /// <summary>
        /// Adds asynchronous steps specified by <paramref name="steps"/> parameter to the scenario.<br/>
        /// The step name is determined from lambda parameter name reflecting action type keyword, corresponding action name and passed list of parameters to called method.<br/>
        /// If scenario is defined with context, the context instance is provided with lambda parameter.<br/>
        /// Example usage:
        /// <code>
        /// AddAsyncSteps(
        ///         _ => Given_numbers(5, 8),
        ///         _ => When_I_add_them(),
        ///         _ => I_should_receive_number(13))
        /// </code>
        /// Expected step signature: <code>Task Given_numbers(params int[] numbers) { /* ... */ }</code>
        /// </summary>
        /// <param name="builder">Builder.</param>
        /// <param name="steps">Steps to add, like: <c>AddAsyncSteps(_ => Given_numbers(5, 8), _ => When_I_add_them(), _ => I_should_receive_number(13))</c></param>
        public static IScenarioRunner<TContext> AddAsyncSteps<TContext>(this IScenarioBuilder<TContext> builder, params Expression<Func<TContext, Task>>[] steps)
        {
            var integration = builder.Integrate();
            var compiler = new ExtendedStepCompiler<TContext>(integration.Core.Configuration);
            integration.Core.AddSteps(steps.Select(compiler.ToStep));
            return integration;
        }

        /// <summary>
        /// Adds steps specified by <paramref name="steps"/> parameter to the composite step.<br/>
        /// The step name is determined from lambda parameter name reflecting action type keyword, corresponding action name and passed list of parameters to called method.<br/>
        /// If scenario is defined with context, the context instance is provided with lambda parameter.<br/>
        /// Example usage:
        /// <code>
        /// AddSteps(
        ///         _ => Given_numbers(5, 8),
        ///         _ => When_I_add_them(),
        ///         _ => I_should_receive_number(13))
        /// </code>
        /// Expected step signature: <code>void Given_numbers(params int[] numbers) { /* ... */ }</code>
        /// </summary>
        /// <param name="builder">Builder.</param>
        /// <param name="steps">Steps to add, like: <c>AddSteps(_ => Given_numbers(5, 8), _ => When_I_add_them(), _ => I_should_receive_number(13))</c></param>
        public static ICompositeStepBuilder<TContext> AddSteps<TContext>(this ICompositeStepBuilder<TContext> builder, params Expression<Action<TContext>>[] steps)
        {
            var coreBuilder = builder.Integrate();
            var compiler = new ExtendedStepCompiler<TContext>(coreBuilder.Configuration);
            coreBuilder.AddSteps(steps.Select(compiler.ToStep));
            return builder;
        }

        /// <summary>
        /// Adds asynchronous steps specified by <paramref name="steps"/> parameter to the composite step.<br/>
        /// The step name is determined from lambda parameter name reflecting action type keyword, corresponding action name and passed list of parameters to called method.<br/>
        /// If scenario is defined with context, the context instance is provided with lambda parameter.<br/>
        /// Example usage:
        /// <code>
        /// AddAsyncSteps(
        ///         _ => Given_numbers(5, 8),
        ///         _ => When_I_add_them(),
        ///         _ => I_should_receive_number(13))
        /// </code>
        /// Expected step signature: <code>Task Given_numbers(params int[] numbers) { /* ... */ }</code>
        /// </summary>
        /// <param name="builder">Builder.</param>
        /// <param name="steps">Steps to add, like: <c>AddAsyncSteps(_ => Given_numbers(5, 8), _ => When_I_add_them(), _ => I_should_receive_number(13))</c></param>
        public static ICompositeStepBuilder<TContext> AddAsyncSteps<TContext>(this ICompositeStepBuilder<TContext> builder, params Expression<Func<TContext, Task>>[] steps)
        {
            var coreBuilder = builder.Integrate();
            var compiler = new ExtendedStepCompiler<TContext>(coreBuilder.Configuration);
            coreBuilder.AddSteps(steps.Select(compiler.ToStep));
            return builder;
        }

        /// <summary>
        /// Runs test scenario by executing given steps in specified order.<br/>
        /// If given step throws, other are not executed.<br/>
        /// The scenario name is determined from the current scenario test method.<br/>
        /// Scenario labels are determined from <see cref="LabelAttribute"/> attributes applied on scenario method.<br/>
        /// The step name is determined from lambda parameter name reflecting action type keyword, corresponding action name and passed list of parameters to called method.<br/>
        /// If scenario is executed with context, the context instance is provided with lambda parameter.<br/>
        /// Example usage:
        /// <code>
        /// Runner.RunScenario(
        ///         _ => Given_numbers(5, 8),
        ///         _ => When_I_add_them(),
        ///         _ => I_should_receive_number(13))
        /// </code>
        /// Expected step signature: <code>void Given_numbers(params int[] numbers) { /* ... */ }</code>
        /// </summary>
        /// <param name="runner">Runner.</param>
        /// <param name="steps">List of steps to execute in order.</param>
        public static void RunScenario<TContext>(this IBddRunner<TContext> runner, params Expression<Action<TContext>>[] steps)
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
        /// The step name is determined from lambda parameter name reflecting action type keyword, corresponding action name and passed list of parameters to called method.<br/>
        /// If scenario is executed with context, the context instance is provided with lambda parameter.<br/>
        /// Example usage:
        /// <code>
        /// Runner.RunScenario(
        ///         _ => Given_numbers(5, 8),
        ///         _ => When_I_add_them(),
        ///         _ => I_should_receive_number(13))
        /// </code>
        /// Expected step signature: <code>void Given_numbers(params int[] numbers) { /* ... */ }</code>
        /// </summary>
        /// <param name="runner">Runner.</param>
        /// <param name="steps">List of steps to execute in order.</param>
        public static async Task RunScenarioAsync<TContext>(this IBddRunner<TContext> runner, params Expression<Action<TContext>>[] steps)
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
        /// The step name is determined from lambda parameter name reflecting action type keyword, corresponding action name and passed list of parameters to called method.<br/>
        /// If scenario is executed with context, the context instance is provided with lambda parameter.<br/>
        /// Example usage:
        /// <code>
        /// await Runner.RunScenarioAsync(
        ///         _ => Given_numbers(5, 8),
        ///         _ => When_I_add_them(),
        ///         _ => I_should_receive_number(13))
        /// </code>
        /// Expected step signature: <code>Task Given_numbers(params int[] numbers) { /* ... */ }</code>
        /// </summary>
        /// <remarks>This is an asynchronous method and should be awaited.</remarks>
        /// <param name="runner">Runner.</param>
        /// <param name="steps">List of steps to execute in order.</param>
        public static async Task RunScenarioAsync<TContext>(this IBddRunner<TContext> runner, params Expression<Func<TContext, Task>>[] steps)
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
