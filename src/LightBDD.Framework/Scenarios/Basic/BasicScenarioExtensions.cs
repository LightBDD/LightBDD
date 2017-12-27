using System;
using System.Diagnostics;
using System.Threading.Tasks;
using LightBDD.Framework.Extensibility;
using LightBDD.Framework.Scenarios.Basic.Implementation;

namespace LightBDD.Framework.Scenarios.Basic
{
    /// <summary>
    /// Extensions class allowing to use basic scenario syntax for running LightBDD tests.
    /// </summary>
    [DebuggerStepThrough]
    public static class BasicScenarioExtensions
    {
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
            Basic(runner)
                .BuildScenario(steps)
                .RunSynchronously();
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
            await Basic(runner)
                .BuildAsyncScenario(steps)
                .RunAsynchronously();
        }

        /// <summary>
        /// Runs test scenario by executing given steps in specified order.<br/>
        /// If given step throws, other are not executed.<br/>
        /// Scenario name is determined from the method name in which <see cref="RunScenarioActionsAsync"/>() method is called.<br/>
        /// Scenario labels are determined from <see cref="LabelAttribute"/> attributes applied on scenario method.<br/>
        /// Step name is determined from corresponding action name.<br/>
        /// Example usage:
        /// <code>
        /// [Scenario]
        /// [Label("Ticket-1")]
        /// public Task Successful_login()
        /// {
        ///     return Runner.RunScenarioActionsAsync(
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
        /// async void Given_the_user_is_about_to_login() { /* ... */ }
        /// </code>
        /// or 
        /// <code>
        /// void Given_the_user_is_about_to_login() { /* ... */ }
        /// </code>
        /// </summary>
        /// <param name="runner">Runner.</param>
        /// <param name="steps">List of steps to execute in order.</param>
        public static async Task RunScenarioActionsAsync(this IBddRunner runner, params Action[] steps)
        {
            await Basic(runner)
                .BuildAsyncScenario(steps)
                .RunAsynchronously();
        }

        private static BasicScenarioRunnerFactory Basic(this IBddRunner runner)
        {
            return new BasicScenarioRunnerFactory(runner.Integrate());
        }
    }
}
