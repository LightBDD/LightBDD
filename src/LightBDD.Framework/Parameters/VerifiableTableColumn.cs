using System;
using System.Diagnostics;
using LightBDD.Framework.Expectations;
using LightBDD.Framework.Parameters.Implementation;

namespace LightBDD.Framework.Parameters
{
    /// <summary>
    /// Type representing <see cref="VerifiableTable{TRow}"/> column.
    /// </summary>
    [DebuggerStepThrough]
    public class VerifiableTableColumn : InputTableColumn
    {
        /// <summary>
        /// Returns true if column is a key column.
        /// </summary>
        public bool IsKey { get; }
        /// <summary>
        /// Returns function providing an <see cref="IExpectation{T}"/> expectation for provided column value.
        /// </summary>
        public Func<object, IExpectation<object>> Expectation { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">Column name.</param>
        /// <param name="isKey">Argument specifying if column is a key column.</param>
        /// <param name="getValue">Function providing column value for specified row object.</param>
        /// <param name="expectation">Function providing an <see cref="IExpectation{T}"/> expectation for provided column value.</param>
        public VerifiableTableColumn(string name, bool isKey, Func<object, ColumnValue> getValue, Func<object, IExpectation<object>> expectation) : base(name, getValue)
        {
            Expectation = expectation;
            IsKey = isKey;
        }

        internal new static VerifiableTableColumn FromColumnInfo(ColumnInfo columnInfo)
        {
            return new VerifiableTableColumn(columnInfo.Name, false, columnInfo.GetValue, expectedColumnValue => Expect.To.Equal(expectedColumnValue));
        }
    }
}