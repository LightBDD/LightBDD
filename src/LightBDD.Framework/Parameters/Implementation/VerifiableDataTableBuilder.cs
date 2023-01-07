using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using LightBDD.Framework.Expectations;
using LightBDD.Framework.Implementation;

namespace LightBDD.Framework.Parameters.Implementation
{
    internal class VerifiableDataTableBuilder<TRow> : AbstractTableBuilder<TRow, VerifiableTableColumn>, IVerifiableDataTableBuilder<TRow>
    {
        public VerifiableDataTable<TRow> Build(IEnumerable<TRow> items)
        {
            var rows = items.ToArray();
            return new VerifiableDataTable<TRow>(BuildColumns(rows), rows);
        }

        public IVerifiableDataTableBuilder<TRow> WithColumn<TValue>(Expression<Func<TRow, TValue>> columnExpression)
        {
            return Add(Reflector.GetMemberName(columnExpression), false, columnExpression.Compile(), x => Expect.To.Equal(x));
        }

        public IVerifiableDataTableBuilder<TRow> WithColumn<TValue>(Expression<Func<TRow, TValue>> columnExpression, Func<TValue, IExpectation<TValue>> expectationFn)
        {
            return Add(Reflector.GetMemberName(columnExpression), false, columnExpression.Compile(), expectationFn);
        }

        public IVerifiableDataTableBuilder<TRow> WithColumn<TValue>(string columnName, Func<TRow, TValue> columnExpression)
        {
            return Add(columnName, false, columnExpression, x => Expect.To.Equal(x));
        }

        public IVerifiableDataTableBuilder<TRow> WithKey<TValue>(Expression<Func<TRow, TValue>> columnExpression)
        {
            return Add(Reflector.GetMemberName(columnExpression), true, columnExpression.Compile(), x => Expect.To.Equal(x));
        }

        public IVerifiableDataTableBuilder<TRow> WithKey<TValue>(Expression<Func<TRow, TValue>> columnExpression, Func<TValue, IExpectation<TValue>> expectationFn)
        {
            return Add(Reflector.GetMemberName(columnExpression), true, columnExpression.Compile(), expectationFn);
        }

        public IVerifiableDataTableBuilder<TRow> WithKey<TValue>(string columnName, Func<TRow, TValue> columnExpression, Func<TValue, IExpectation<TValue>> expectationFn)
        {
            return Add(columnName, true, columnExpression, expectationFn);
        }

        public IVerifiableDataTableBuilder<TRow> WithKey<TValue>(string columnName, Func<TRow, TValue> columnExpression)
        {
            return Add(columnName, true, columnExpression, x => Expect.To.Equal(x));
        }

        public IVerifiableDataTableBuilder<TRow> WithInferredColumns() => WithInferredColumns(InferredColumnsOrder.Name);

        public IVerifiableDataTableBuilder<TRow> WithInferredColumns(InferredColumnsOrder columnsOrder)
        {
            InferColumns = true;
            InferredColumnsOrder = columnsOrder;
            return this;
        }

        public IVerifiableDataTableBuilder<TRow> WithColumn<TValue>(string columnName, Func<TRow, TValue> columnExpression, Func<TValue, IExpectation<TValue>> expectationFn)
        {
            return Add(columnName, false, columnExpression, expectationFn);
        }

        private IVerifiableDataTableBuilder<TRow> Add<TValue>(string columnName, bool isKey, Func<TRow, TValue> columnExpression, Func<TValue, IExpectation<TValue>> expectationFn)
        {
            AddCustomColumn(new VerifiableTableColumn(
                columnName,
                isKey,
                row => ColumnValue.From(columnExpression((TRow)row)),
                value => expectationFn((TValue)value).CastFrom(Expect.Type<object>())));

            return this;
        }

        protected override VerifiableTableColumn CreateColumn(ColumnInfo columnInfo)
        {
            return VerifiableTableColumn.FromColumnInfo(columnInfo);
        }

        public IVerifiableDataTableBuilder<TRow> WithKey<TValue>(Expression<Func<TRow, TValue>> columnExpression, IExpectation<TValue> expectation)
        {
            return Add(Reflector.GetMemberName(columnExpression), true, columnExpression.Compile(), _ => expectation);
        }

        public IVerifiableDataTableBuilder<TRow> WithKey<TValue>(string columnName, Func<TRow, TValue> columnExpression, IExpectation<TValue> expectation)
        {
            return Add(columnName, true, columnExpression, _ => expectation);
        }

        public IVerifiableDataTableBuilder<TRow> WithColumn<TValue>(Expression<Func<TRow, TValue>> columnExpression, IExpectation<TValue> expectation)
        {
            return Add(Reflector.GetMemberName(columnExpression), false, columnExpression.Compile(), _ => expectation);
        }

        public IVerifiableDataTableBuilder<TRow> WithColumn<TValue>(string columnName, Func<TRow, TValue> columnExpression, IExpectation<TValue> expectation)
        {
            return Add(columnName, false, columnExpression, _ => expectation);
        }
    }
}