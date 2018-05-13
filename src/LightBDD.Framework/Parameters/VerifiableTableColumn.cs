using System;
using LightBDD.Framework.Expectations;

namespace LightBDD.Framework.Parameters
{
    public class VerifiableTableColumn : TableColumn
    {
        public Func<object,IExpectation<object>> Expectation { get; }

        public VerifiableTableColumn(string name, bool isKey, Func<object, ColumnValue> getValue, Func<object, IExpectation<object>> expectation) : base(name, isKey, getValue)
        {
            Expectation = expectation;
        }
    }
}