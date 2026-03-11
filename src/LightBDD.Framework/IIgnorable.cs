namespace LightBDD.Framework
{
    /// <summary>
    /// Interface for test fixtures that expose a settings object controlling conditional scenario ignore behavior.
    /// <para>
    /// Implement this on the test fixture class to enable framework-specific <c>IgnoreScenarioIfAttribute&lt;T&gt;</c> attributes.
    /// </para>
    /// </summary>
    /// <typeparam name="T">The type of the settings object whose boolean properties control conditional ignoring.</typeparam>
    public interface IIgnorable<out T> where T : class
    {
        /// <summary>
        /// Returns the settings object whose boolean properties determine whether scenarios should be ignored.
        /// </summary>
        T IgnoreSettings { get; }
    }
}

