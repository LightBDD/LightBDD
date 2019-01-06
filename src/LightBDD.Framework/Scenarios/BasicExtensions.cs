using System;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Core.Execution;
using LightBDD.Framework.Scenarios.Implementation;

namespace LightBDD.Framework.Scenarios
{
    /// <summary>
    /// Extensions class allowing to use basic scenario syntax for running LightBDD tests.
    /// </summary>
    public static class BasicExtensions
    {
        public static IScenarioRunner<NoContext> AddSteps(this IScenarioBuilder<NoContext> builder, params Action[] steps)
        {
            var integration = builder.Integrate();
            integration.Core.AddSteps(steps.Select(BasicStepCompiler.ToSynchronousStep));
            return integration;
        }

        public static IScenarioRunner<NoContext> AddAsyncSteps(this IScenarioBuilder<NoContext> builder, params Func<Task>[] steps)
        {
            var integration = builder.Integrate();
            integration.Core.AddSteps(steps.Select(BasicStepCompiler.ToAsynchronousStep));
            return integration;
        }

        public static ICompositeStepBuilder<NoContext> AddSteps(this ICompositeStepBuilder<NoContext> builder, params Action[] steps)
        {
            builder.Integrate().AddSteps(steps.Select(BasicStepCompiler.ToSynchronousStep));
            return builder;
        }

        public static ICompositeStepBuilder<NoContext> AddAsyncSteps(this ICompositeStepBuilder<NoContext> builder, params Func<Task>[] steps)
        {
            builder.Integrate().AddSteps(steps.Select(BasicStepCompiler.ToAsynchronousStep));
            return builder;
        }

        /// <summary>
        /// Runs test scenario by executing given steps in specified order.<br/>
        /// If given step throws, other are not executed.<br/>
        /// Scenario name is determined from the method name in which <see cref="RunScenario"/>() method is called.<br/>
        /// Scenario labels are determined from <see cref="LabelAttribute"/> attributes applied on scenario method.<br/>
        /// Step name is determined from corresponding action name.<br/>
        /// Example usage:
        /// <code>
        /// [Scenario]
        /// [Label("Ticket-1")]
        /// public void Successful_login()
        /// {
        ///     Runner.RunScenario(
        ///         Given_the_user_is_about_to_login,
        ///         Given_the_user_entered_valid_login,
        ///         Given_the_user_entered_valid_password,
        ///         When_the_user_clicks_login_button,
        ///         Then_the_login_operation_should_be_successful,
        ///         Then_a_welcome_message_containing_user_name_should_be_returned);
        /// }
        /// </code>
        /// Expected step signature:
        /// <code>
        /// void Given_the_user_is_about_to_login() { /* ... */ }
        /// </code>
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
                    .WithCapturedScenarioDetails()
                    .Build()
                    .ExecuteSync();
            }
            catch (ScenarioExecutionException e)
            {
                e.GetOriginal().Throw();
            }
        }

        /// <summary>
        /// Runs test scenario by executing given steps in specified order.<br/>
        /// If given step throws, other are not executed.<br/>
        /// Scenario name is determined from the method name in which <see cref="RunScenarioAsync"/>() method is called.<br/>
        /// Scenario labels are determined from <see cref="LabelAttribute"/> attributes applied on scenario method.<br/>
        /// Step name is determined from corresponding action name.<br/>
        /// Example usage:
        /// <code>
        /// [Scenario]
        /// [Label("Ticket-1")]
        /// public Task Successful_login()
        /// {
        ///     return Runner.RunScenarioAsync(
        ///         Given_the_user_is_about_to_login,
        ///         Given_the_user_entered_valid_login,
        ///         Given_the_user_entered_valid_password,
        ///         When_the_user_clicks_login_button,
        ///         Then_the_login_operation_should_be_successful,
        ///         Then_a_welcome_message_containing_user_name_should_be_returned);
        /// }
        /// </code>
        /// Expected step signature:
        /// <code>
        /// async Task Given_the_user_is_about_to_login() { /* ... */ }
        /// </code>
        /// </summary>
        /// <param name="runner">Runner.</param>
        /// <param name="steps">List of steps to execute in order.</param>
        public static async Task RunScenarioAsync(this IBddRunner runner, params Func<Task>[] steps)
        {
            try
            {
                await runner
                    .AddAsyncSteps(steps)
                    .Integrate().Core
                    .WithCapturedScenarioDetails()
                    .Build()
                    .ExecuteAsync();
            }
            catch (ScenarioExecutionException e)
            {
                e.GetOriginal().Throw();
            }
        }
    }
}
