using LightBDD.Core.Metadata;

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
        /// Parameter info.
        /// </summary>
        IParameterInfo Info { get; }
        /// <summary>
        /// Parameter formatted value.
        /// </summary>
        string FormattedValue { get; }
        /// <summary>
        /// Parameter details.
        /// </summary>
        IParameterDetails Details { get; }
    }
}