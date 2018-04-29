using System.Collections.Generic;

namespace LightBDD.Core.Results.Parameters
{
    /// <summary>
    /// Interface representing tabular parameter result.
    /// </summary>
    public interface ITabularParameterResult : IParameterResult
    {
        /// <summary>
        /// Returns list of table columns.
        /// </summary>
        IEnumerable<ITableColumn> Columns { get; }
        /// <summary>
        /// Returns list of table rows.
        /// </summary>
        IEnumerable<ITableRow> Rows { get; }
    }
}