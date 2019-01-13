using System;
using System.Linq.Expressions;
using LightBDD.Framework.Expectations;

namespace LightBDD.Framework.Parameters.Implementation
{
    internal class TableValidatorBuilder<TRow> : AbstractTableBuilder<TRow, VerifiableTableColumn>, ITableValidatorBuilder<TRow>
    {
        public TableValidator<TRow> Build()
        {
            return new TableValidator<TRow>(BuildColumns(new TRow[0]));
        }

        protected override VerifiableTableColumn CreateColumn(ColumnInfo columnInfo)
        {
            return VerifiableTableColumn.FromColumnInfo(columnInfo);
        }

        public ITableValidatorBuilder<TRow> WithColumn<TValue>(Expression<Func<TRow, TValue>> columnExpression, IExpectation<TValue> expectation)
        {
            return Add(Reflector.GetMemberName(columnExpression), false, columnExpression.Compile(), _ => expectation);
        }

        public ITableValidatorBuilder<TRow> WithColumn<TValue>(string columnName, Func<TRow, TValue> columnExpression, IExpectation<TValue> expectation)
        {
            return Add(columnName, false, columnExpression, _ => expectation);
        }

        private ITableValidatorBuilder<TRow> Add<TValue>(string columnName, bool isKey, Func<TRow, TValue> columnExpression, Func<TValue, IExpectation<TValue>> expectationFn)
        {
            AddCustomColumn(new VerifiableTableColumn(
                columnName,
                isKey,
                row => ColumnValue.From(columnExpression((TRow)row)),
                value => expectationFn(default).CastFrom(Expect.Type<object>())));

            return this;
        }
    }
}