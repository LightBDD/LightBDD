using System;
using LightBDD.Framework.Expectations;
using LightBDD.Framework.Parameters.Implementation;

namespace LightBDD.Framework.Parameters
{
    public class VerifiableTableColumn : TableColumn
    {
        public bool IsKey { get; }
        public Func<object, IExpectation<object>> Expectation { get; }

        public VerifiableTableColumn(string name, bool isKey, Func<object, ColumnValue> getValue, Func<object, IExpectation<object>> expectation) : base(name, getValue)
        {
            Expectation = expectation;
            IsKey = isKey;
        }

        internal static VerifiableTableColumn FromColumnInfo(ColumnInfo columnInfo)
        {
            return new VerifiableTableColumn(columnInfo.Name, false, columnInfo.GetValue, expectedColumnValue => Expect.To.Equal(expectedColumnValue));
        }
    }
}