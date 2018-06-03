using System.Collections.Generic;

namespace LightBDD.Core.Results.Parameters.Tabular
{
    /// <summary>
    /// Interface representing tabular parameter result.
    /// </summary>
    public interface ITabularParameterDetails : IParameterDetails
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