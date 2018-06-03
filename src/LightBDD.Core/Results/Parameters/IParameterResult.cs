namespace LightBDD.Core.Results.Parameters
{
    /// <summary>
    /// Interface representing parameter result.
    /// </summary>
    public interface IParameterResult
    {
        /// <summary>
        /// Parameter name.
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Parameter value.
        /// </summary>
        IParameterDetails Details { get; }
    }
}