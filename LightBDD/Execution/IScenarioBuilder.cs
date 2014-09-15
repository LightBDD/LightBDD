using System;

namespace LightBDD.Execution
{
    /// <summary>
    /// Scenario builder interface that allows to build and execute scenario in fluent way.
    /// </summary>
    public interface IScenarioBuilder
    {
        /// <summary>
        /// Associates label with scenario.
        /// 
        /// Example usage:
        /// <code>
        /// [Test]
        /// public void Successful_login()
        /// {
        ///     _bddRunner.NewScenario("My successful login")
        ///         .WithLabel("Ticket-1")
        ///         .Execute(
        ///             Given_user_is_about_to_login,
        ///             Given_user_entered_valid_login,
        ///             Given_user_entered_valid_password,
        ///             When_user_clicked_login_button,
        ///             Then_login_is_successful,
        ///             Then_welcome_message_is_returned_containing_user_name);
        /// }
        /// </code>
        /// </summary>
        /// <param name="label">Label to associate with scenario.</param>
        /// <returns>Scenario builder</returns>
        IScenarioBuilder WithLabel(string label);

        /// <summary>
        /// Completes scenario build process and executes given steps in order.
        /// If any step throws, other are not executed and exception is propagated to calling method.
        /// Example usage:
        /// <code>
        /// [Test]
        /// public void Successful_login()
        /// {
        ///     _bddRunner.NewScenario("My successful login")
        ///         .WithLabel("Ticket-1")
        ///         .Execute(
        ///             Given_user_is_about_to_login,
        ///             Given_user_entered_valid_login,
        ///             Given_user_entered_valid_password,
        ///             When_user_clicked_login_button,
        ///             Then_login_is_successful,
        ///             Then_welcome_message_is_returned_containing_user_name);
        /// }
        /// </code>
        /// </summary>
        /// <param name="steps">List of steps to execute in order.</param>
        void Execute(params Action[] steps);
        /// <summary>
        /// Completes scenario build process and executes given steps in order, where all steps share context of <c>TContext</c> type instantiated with default constructor.
        /// If any step throws, other are not executed and exception is propagated to calling method.
        /// Example usage:
        /// <code>
        /// [Test]
        /// public void Successful_login()
        /// {
        ///     _bddRunner.NewScenario("My successful login")
        ///         .WithLabel("Ticket-1")
        ///         .Execute(
        ///             Given_user_is_about_to_login,
        ///             Given_user_entered_valid_login,
        ///             Given_user_entered_valid_password,
        ///             When_user_clicked_login_button,
        ///             Then_login_is_successful,
        ///             Then_welcome_message_is_returned_containing_user_name);
        /// }
        /// </code>
        /// </summary>
        /// <typeparam name="TContext">Type of context that would be shared between all steps.</typeparam>
        /// <param name="steps">List of steps to execute in order.</param>
        void Execute<TContext>(params Action<TContext>[] steps) where TContext : new();
        /// <summary>
        /// Completes scenario build process and executes given steps in order, where all steps share given <c>context</c> instance of <c>TContext</c> type.
        /// If any step throws, other are not executed and exception is propagated to calling method.
        /// Example usage:
        /// <code>
        /// [Test]
        /// public void Successful_login()
        /// {
        ///     _bddRunner.NewScenario("My successful login")
        ///         .WithLabel("Ticket-1")
        ///         .Execute(context,
        ///             Given_user_is_about_to_login,
        ///             Given_user_entered_valid_login,
        ///             Given_user_entered_valid_password,
        ///             When_user_clicked_login_button,
        ///             Then_login_is_successful,
        ///             Then_welcome_message_is_returned_containing_user_name);
        /// }
        /// </code>
        /// </summary>
        /// <typeparam name="TContext">Type of context that would be shared between all steps.</typeparam>
        /// <param name="context">Context instance that would be shared between all steps.</param>
        /// <param name="steps">List of steps to execute in order.</param>
        void Execute<TContext>(TContext context, params Action<TContext>[] steps);
    }
}