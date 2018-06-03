namespace LightBDD.Core.Results.Parameters.Tabular
{
    /// <summary>
    /// Interface representing tabular parameter result table column.
    /// </summary>
    public interface ITabularParameterColumn
    {
        /// <summary>
        /// Returns column name.
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Returns <c>true</c> for key column.
        /// </summary>
        bool IsKey { get; }
    }
}