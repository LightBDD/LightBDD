namespace LightBDD
{
    /// <summary>
    /// Scenario assert class, extending xUnit assertions with methods like <c>Ignore</c>().
    /// </summary>
    public static class ScenarioAssert
    {
        /// <summary>
        /// Aborts execution of current scenario, marking scenario ignored.
        /// The currently executed step as well as scenario status becomes 'Ignored' in LightBDD reports.
        /// The test itself would be marked as 'Skipped' in xUnit, but only if the test is annotated with [Scenario] attribute.
        /// This method is not supported by regular [Fact] / [Theory] attributes.
        /// </summary>
        /// <param name="message">Ignore message.</param>
        public static void Ignore(string message)
        {
            throw new IgnoreException(message);
        }
    }
}