using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using LightBDD.Framework.Expectations;

namespace LightBDD.Framework.Parameters.Implementation
{
    [DebuggerStepThrough]
    internal class VerifiableTableBuilder<TRow> : AbstractTableBuilder<TRow, VerifiableTableColumn>, IVerifiableTableBuilder<TRow>
    {
        public VerifiableTable<TRow> Build()
        {
            return new VerifiableTable<TRow>(BuildColumns(new TRow[0]));
        }

        protected override VerifiableTableColumn CreateColumn(ColumnInfo columnInfo)
        {
            return VerifiableTableColumn.FromColumnInfo(columnInfo);
        }

        public IVerifiableTableBuilder<TRow> WithColumn<TValue>(Expression<Func<TRow, TValue>> columnExpression, IExpectation<TValue> expectation)
        {
            return Add(Reflector.GetMemberName(columnExpression), false, columnExpression.Compile(), _ => expectation);
        }

        public IVerifiableTableBuilder<TRow> WithColumn<TValue>(string columnName, Func<TRow, TValue> columnExpression, IExpectation<TValue> expectation)
        {
            return Add(columnName, false, columnExpression, _ => expectation);
        }

        private IVerifiableTableBuilder<TRow> Add<TValue>(string columnName, bool isKey, Func<TRow, TValue> columnExpression, Func<TValue, IExpectation<TValue>> expectationFn)
        {
            AddCustomColumn(new VerifiableTableColumn(
                columnName,
                isKey,
                row => ColumnValue.From(columnExpression((TRow)row)),
                value => expectationFn(default(TValue)).CastFrom(Expect.Type<object>())));

            return this;
        }
    }
}