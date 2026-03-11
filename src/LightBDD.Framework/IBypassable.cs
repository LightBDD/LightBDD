namespace LightBDD.Framework
{
    /// <summary>
    /// Interface for test fixtures that expose a settings object controlling conditional step bypass behavior.
    /// <para>
    /// Implement this on the test fixture class to enable <see cref="BypassStepIfAttribute{T}"/>.
    /// </para>
    /// </summary>
    /// <typeparam name="T">The type of the settings object whose boolean properties control conditional bypassing.</typeparam>
    public interface IBypassable<out T> where T : class
    {
        /// <summary>
        /// Returns the settings object whose boolean properties determine whether steps should be bypassed.
        /// </summary>
        T BypassSettings { get; }
    }
}
