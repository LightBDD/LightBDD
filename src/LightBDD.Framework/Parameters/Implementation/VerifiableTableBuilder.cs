using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using LightBDD.Framework.Expectations;

namespace LightBDD.Framework.Parameters.Implementation
{
    internal class VerifiableTableBuilder<TRow> : IVerifiableTableBuilder<TRow>
    {
        private readonly List<VerifiableTableColumn> _columns = new List<VerifiableTableColumn>();
        public VerifiableTable<TRow> Build(IEnumerable<TRow> items)
        {
            return new VerifiableTable<TRow>(items, _columns);
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

        public IVerifiableTableBuilder<TRow> WithColumn<TValue>(string columnName, Func<TRow, TValue> columnExpression, Func<TValue, IExpectation<TValue>> expectationFn)
        {
            return Add(columnName, false, columnExpression, expectationFn);
        }

        private IVerifiableTableBuilder<TRow> Add<TValue>(string columnName, bool isKey, Func<TRow, TValue> columnExpression, Func<TValue, IExpectation<TValue>> expectationFn)
        {
            _columns.Add(new VerifiableTableColumn(
                columnName,
                isKey,
                row => ColumnValue.From(columnExpression((TRow)row)),
                value => new ColumnExpectation<TValue>(expectationFn((TValue)value))));
            return this;
        }
    }
}