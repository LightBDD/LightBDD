using LightBDD.Core.Formatting;

namespace LightBDD.Framework.Formatting
{
    /// <summary>
    /// Default implementation of <see cref="INameFormatter"/> interface.
    /// </summary>
    public class DefaultNameFormatter : INameFormatter
    {
        /// <summary>
        /// Formats name into readable text.
        /// This method applies following replacements:<br/>
        /// "__" -> ": "<br/>
        /// "_s_" -> "'s "<br/>
        /// "_" -> " "<br/>
        /// </summary>
        /// <param name="name">Name to format.</param>
        /// <returns>Formatted text.</returns>
        public string FormatName(string name)
        {
            return name
                .Replace("__", ": ")
                .Replace("_s_", "'s ")
                .Replace("_", " ");
        }
    }
}