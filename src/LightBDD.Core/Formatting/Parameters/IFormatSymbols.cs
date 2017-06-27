namespace LightBDD.Core.Formatting.Parameters
{
    /// <summary>
    /// Interface providing format symbols that should be used in parameter formatting.
    /// </summary>
    public interface IFormatSymbols
    {
        /// <summary>
        /// Null value text representation.
        /// </summary>
        string NullValue { get; }
    }
}