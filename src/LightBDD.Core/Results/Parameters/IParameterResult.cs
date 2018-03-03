using System.Collections.Generic;

namespace LightBDD.Core.Results.Parameters
{
    /// <summary>
    /// TODO
    /// </summary>
    public interface IParameterResult
    {
        /// <summary>
        /// TODO
        /// </summary>
        string Name { get; }
    }

    public interface ITabularParameterResult : IParameterResult
    {
        IEnumerable<ITableColumn> Columns { get; }
        IEnumerable<ITableRow> Rows { get; }
    }

    public interface ITableColumn
    {
        string Name { get; }
        bool IsKey { get; }
    }

    public interface ITableRow
    {
        TableRowType Type { get; }
        IEnumerable<IValueResult> Values { get; }
    }

    public enum TableRowType
    {
        Matching,
        Surplus,
        Missing
    }
}