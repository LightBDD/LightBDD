using System;
using System.Linq.Expressions;
using LightBDD.Framework.Expectations;

namespace LightBDD.Framework.Parameters
{
    public interface IVerifiableTableBuilder<TRow>
    {
        IVerifiableTableBuilder<TRow> WithColumn<TValue>(Expression<Func<TRow, TValue>> columnExpression);
        IVerifiableTableBuilder<TRow> WithColumn<TValue>(Expression<Func<TRow, TValue>> columnExpression, Func<TValue, IExpectation<TValue>> expectationFn);
        IVerifiableTableBuilder<TRow> WithColumn<TValue>(string columnName, Func<TRow, TValue> columnExpression, Func<TValue, IExpectation<TValue>> expectationFn);
        IVerifiableTableBuilder<TRow> WithColumn<TValue>(string columnName, Func<TRow, TValue> columnExpression);

        IVerifiableTableBuilder<TRow> WithKey<TValue>(Expression<Func<TRow, TValue>> columnExpression);
        IVerifiableTableBuilder<TRow> WithKey<TValue>(Expression<Func<TRow, TValue>> columnExpression, Func<TValue, IExpectation<TValue>> expectationFn);
        IVerifiableTableBuilder<TRow> WithKey<TValue>(string columnName, Func<TRow, TValue> columnExpression, Func<TValue, IExpectation<TValue>> expectationFn);
        IVerifiableTableBuilder<TRow> WithKey<TValue>(string columnName, Func<TRow, TValue> columnExpression);
    }
}