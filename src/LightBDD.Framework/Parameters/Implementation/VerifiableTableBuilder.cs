using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using LightBDD.Framework.Expectations;

namespace LightBDD.Framework.Parameters.Implementation
{
    internal class VerifiableTableBuilder<TRow> : AbstractTableBuilder<TRow, VerifiableTableColumn>, IVerifiableTableBuilder<TRow>
    {
        public VerifiableTable<TRow> Build(IEnumerable<TRow> items)
        {
            var rows = items.ToArray();
            return new VerifiableTable<TRow>(BuildColumns(rows), rows);
        }

        public IVerifiableTableBuilder<TRow> WithColumn<TValue>(Expression<Func<TRow, TValue>> columnExpression)
        {
            return Add(Reflector.GetMemberName(columnExpression), false, columnExpression.Compile(), x => Expect.To.Equal(x));
        }

        public IVerifiableTableBuilder<TRow> WithColumn<TValue>(Expression<Func<TRow, TValue>> columnExpression, Func<TValue, IExpectation<TValue>> expectationFn)
        {
            return Add(Reflector.GetMemberName(columnExpression), false, columnExpression.Compile(), expectationFn);
        }

        public IVerifiableTableBuilder<TRow> WithColumn<TValue>(string columnName, Func<TRow, TValue> columnExpression)
        {
            return Add(columnName, false, columnExpression, x => Expect.To.Equal(x));
        }

        public IVerifiableTableBuilder<TRow> WithKey<TValue>(Expression<Func<TRow, TValue>> columnExpression)
        {
            return Add(Reflector.GetMemberName(columnExpression), true, columnExpression.Compile(), x => Expect.To.Equal(x));
        }

        public IVerifiableTableBuilder<TRow> WithKey<TValue>(Expression<Func<TRow, TValue>> columnExpression, Func<TValue, IExpectation<TValue>> expectationFn)
        {
            return Add(Reflector.GetMemberName(columnExpression), true, columnExpression.Compile(), expectationFn);
        }

        public IVerifiableTableBuilder<TRow> WithKey<TValue>(string columnName, Func<TRow, TValue> columnExpression, Func<TValue, IExpectation<TValue>> expectationFn)
        {
            return Add(columnName, true, columnExpression, expectationFn);
        }

        public IVerifiableTableBuilder<TRow> WithKey<TValue>(string columnName, Func<TRow, TValue> columnExpression)
        {
            return Add(columnName, true, columnExpression, x => Expect.To.Equal(x));
        }

        public IVerifiableTableBuilder<TRow> WithInferredColumns()
        {
            InferColumns = true;
            return this;
        }

        public IVerifiableTableBuilder<TRow> WithColumn<TValue>(string columnName, Func<TRow, TValue> columnExpression, Func<TValue, IExpectation<TValue>> expectationFn)
        {
            return Add(columnName, false, columnExpression, expectationFn);
        }

        private IVerifiableTableBuilder<TRow> Add<TValue>(string columnName, bool isKey, Func<TRow, TValue> columnExpression, Func<TValue, IExpectation<TValue>> expectationFn)
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
    }
}