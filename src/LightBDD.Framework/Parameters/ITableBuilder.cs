using System;
using System.Linq.Expressions;

namespace LightBDD.Framework.Parameters
{
    public interface ITableBuilder<TRow>
    {
        ITableBuilder<TRow> WithColumn<TValue>(Expression<Func<TRow, TValue>> columnExpression);
        ITableBuilder<TRow> WithColumn<TValue>(string columnName, Func<TRow, TValue> columnExpression);
        ITableBuilder<TRow> WithInferredColumns();
    }
}