namespace LightBDD.Core.Formatting
{
    /// <summary>
    /// Interface allowing to format name into readable text.
    /// </summary>
    public interface INameFormatter
    {
        /// <summary>
        /// Formats provided name into readable text.
        /// </summary>
        /// <param name="name">Name to format.</param>
        /// <returns>Formatted name.</returns>
        string FormatName(string name);
    }
}