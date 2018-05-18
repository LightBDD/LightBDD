using System.Collections.Generic;

namespace LightBDD.Core.Results.Parameters
{
    /// <summary>
    /// Interface representing tabular parameter result table row
    /// </summary>
    public interface ITabularParameterRow : IVerificationResult
    {
        /// <summary>
        /// Returns row type.
        /// </summary>
        TableRowType Type { get; }
        /// <summary>
        /// Returns row values, where order corresponds to the column order specified in <see cref="ITabularParameterResult"/>.
        /// </summary>
        IReadOnlyList<IValueResult> Values { get; }
    }
}