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
        ///     Runner.NewScenario("My successful login")
        ///         .WithLabel("Ticket-1")
        ///         .Run(
        ///             Given_the_user_is_about_to_login,
        ///             Given_the_user_entered_valid_login,
        ///             Given_the_user_entered_valid_password,
        ///             When_the_user_clicks_login_button,
        ///             Then_the_login_operation_should_be_successful,
        ///             Then_a_welcome_message_containing_user_name_should_be_returned);
        /// }
        /// </code>
        /// </summary>
        /// <param name="label">Label to associate with scenario.</param>
        /// <returns>Scenario builder</returns>
        ICustomizedScenarioBuilder WithLabel(string label);

        /// <summary>
        /// Associates list of categories with scenario.
        /// </summary>
        /// <param name="categories">Categories to associate.</param>
        /// <returns>Scenario builder</returns>
        ICustomizedScenarioBuilder WithCategories(params string[] categories);
    }
}