using System.Collections.Generic;

namespace LightBDD.Core.Results.Parameters
{
    /// <summary>
    /// Interface representing tabular parameter result.
    /// </summary>
    public interface ITabularParameterResult : IParameterVerificationResult
    {
        /// <summary>
        /// Returns list of table columns.
        /// </summary>
        IReadOnlyList<ITabularParameterColumn> Columns { get; }
        /// <summary>
        /// Returns list of table rows.
        /// </summary>
        IReadOnlyList<ITabularParameterRow> Rows { get; }
    }
}