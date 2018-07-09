using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using LightBDD.Framework.Expectations;

namespace LightBDD.Framework.Parameters.Implementation
{
    [DebuggerStepThrough]
    internal class ExpectedTableBuilder<TRow> : AbstractTableBuilder<TRow, VerifiableTableColumn>, IExpectedTableBuilder<TRow>
    {
        public ExpectedTable<TRow> Build(IEnumerable<TRow> items)
        {
            var rows = items.ToArray();
            return new ExpectedTable<TRow>(BuildColumns(rows), rows);
        }

        public IExpectedTableBuilder<TRow> WithColumn<TValue>(Expression<Func<TRow, TValue>> columnExpression)
        {
            return Add(Reflector.GetMemberName(columnExpression), false, columnExpression.Compile(), x => Expect.To.Equal(x));
        }

        public IExpectedTableBuilder<TRow> WithColumn<TValue>(Expression<Func<TRow, TValue>> columnExpression, Func<TValue, IExpectation<TValue>> expectationFn)
        {
            return Add(Reflector.GetMemberName(columnExpression), false, columnExpression.Compile(), expectationFn);
        }

        public IExpectedTableBuilder<TRow> WithColumn<TValue>(string columnName, Func<TRow, TValue> columnExpression)
        {
            return Add(columnName, false, columnExpression, x => Expect.To.Equal(x));
        }

        public IExpectedTableBuilder<TRow> WithKey<TValue>(Expression<Func<TRow, TValue>> columnExpression)
        {
            return Add(Reflector.GetMemberName(columnExpression), true, columnExpression.Compile(), x => Expect.To.Equal(x));
        }

        public IExpectedTableBuilder<TRow> WithKey<TValue>(Expression<Func<TRow, TValue>> columnExpression, Func<TValue, IExpectation<TValue>> expectationFn)
        {
            return Add(Reflector.GetMemberName(columnExpression), true, columnExpression.Compile(), expectationFn);
        }

        public IExpectedTableBuilder<TRow> WithKey<TValue>(string columnName, Func<TRow, TValue> columnExpression, Func<TValue, IExpectation<TValue>> expectationFn)
        {
            return Add(columnName, true, columnExpression, expectationFn);
        }

        public IExpectedTableBuilder<TRow> WithKey<TValue>(string columnName, Func<TRow, TValue> columnExpression)
        {
            return Add(columnName, true, columnExpression, x => Expect.To.Equal(x));
        }

        public IExpectedTableBuilder<TRow> WithInferredColumns()
        {
            InferColumns = true;
            return this;
        }

        public IExpectedTableBuilder<TRow> WithColumn<TValue>(string columnName, Func<TRow, TValue> columnExpression, Func<TValue, IExpectation<TValue>> expectationFn)
        {
            return Add(columnName, false, columnExpression, expectationFn);
        }

        private IExpectedTableBuilder<TRow> Add<TValue>(string columnName, bool isKey, Func<TRow, TValue> columnExpression, Func<TValue, IExpectation<TValue>> expectationFn)
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

        public IExpectedTableBuilder<TRow> WithKey<TValue>(Expression<Func<TRow, TValue>> columnExpression, IExpectation<TValue> expectation)
        {
            return Add(Reflector.GetMemberName(columnExpression), true, columnExpression.Compile(), _ => expectation);
        }

        public IExpectedTableBuilder<TRow> WithKey<TValue>(string columnName, Func<TRow, TValue> columnExpression, IExpectation<TValue> expectation)
        {
            return Add(columnName, true, columnExpression, _ => expectation);
        }

        public IExpectedTableBuilder<TRow> WithColumn<TValue>(Expression<Func<TRow, TValue>> columnExpression, IExpectation<TValue> expectation)
        {
            return Add(Reflector.GetMemberName(columnExpression), false, columnExpression.Compile(), _ => expectation);
        }

        public IExpectedTableBuilder<TRow> WithColumn<TValue>(string columnName, Func<TRow, TValue> columnExpression, IExpectation<TValue> expectation)
        {
            return Add(columnName, false, columnExpression, _ => expectation);
        }
    }
}