using System;
using System.Linq.Expressions;

namespace LightBDD.Execution
{
    /// <summary>
    /// Scenario builder interface that allows to build and execute scenario in fluent way.
    /// </summary>
    public interface IScenarioBuilder
    {
        /// <summary>
        /// Completes scenario build process and executes given steps in order.
        /// If any step throws, other are not executed and exception is propagated to calling method.
        /// Example usage:
        /// <code>
        /// [Test]
        /// public void Successful_login()
        /// {
        ///     _bddRunner.NewScenario()
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
        void RunSimpleSteps(params Action[] steps);

        void RunFormalizedSteps(params Expression<Action<StepType>>[] steps);
        IScenarioBuilder<TContext> WithContext<TContext>() where TContext : new();
        IScenarioBuilder<TContext> WithContext<TContext>(TContext instance);
    }

    public interface IScenarioBuilder<TContext>
    {
        /// <summary>
        /// Completes scenario build process and executes given steps in order, where all steps share context of <c>TContext</c> type instantiated with default constructor.
        /// If any step throws, other are not executed and exception is propagated to calling method.
        /// Example usage:
        /// <code>
        /// [Test]
        /// public void Successful_login()
        /// {
        ///     _bddRunner.NewScenario()
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
        void RunSimpleSteps(params Action<TContext>[] steps);
        void RunFormalizedSteps(params Expression<Action<StepType, TContext>>[] steps);
    }
}