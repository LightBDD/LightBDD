namespace LightBDD.Core.Results.Parameters
{
    /// <summary>
    /// Interface representing inline parameter result.
    /// </summary>
    public interface IInlineParameterResult : IParameterResult
    {
        /// <summary>
        /// Returns parameter value.
        /// </summary>
        IValueResult Value { get; }
    }
}