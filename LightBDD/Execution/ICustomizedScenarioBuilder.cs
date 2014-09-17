namespace LightBDD.Execution
{
    /// <summary>
    /// Customized scenario builder interface that allows to build and execute customized scenario in fluent way.
    /// </summary>
    public interface ICustomizedScenarioBuilder : IScenarioBuilder
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
        ICustomizedScenarioBuilder WithLabel(string label);
    }
}