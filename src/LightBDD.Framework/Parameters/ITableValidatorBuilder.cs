using System;
using System.Linq.Expressions;
using LightBDD.Framework.Expectations;

namespace LightBDD.Framework.Parameters
{
    /// <summary>
    /// Builder interface allowing to define <see cref="TableValidator{TRow}"/> instance.
    /// </summary>
    public interface ITableValidatorBuilder<TRow>
    {
        /// <summary>
        /// Defines column, based on <paramref name="columnExpression"/>.
        /// The <paramref name="columnExpression"/> is an expression used to provide column value.
        /// The <paramref name="columnExpression"/> has to be a field/property accessor expression, where the accessed member will be used for column name.
        /// The <paramref name="expectation"/> function is used to return expression for verifying column value.
        /// </summary>
        /// <typeparam name="TValue">Column value type.</typeparam>
        /// <param name="columnExpression">Field/property accessor expression</param>
        /// <param name="expectation">Function returning column expectation expression.</param>
        /// <exception cref="InvalidOperationException">Thrown if <paramref name="columnExpression"/> is not member expression.</exception>
        /// <returns>Self</returns>
        ITableValidatorBuilder<TRow> WithColumn<TValue>(Expression<Func<TRow, TValue>> columnExpression, IExpectation<TValue> expectation);

        /// <summary>
        /// Defines column, based on <paramref name="columnExpression"/>.
        /// The <paramref name="columnExpression"/> is an expression used to provide column value and it can be any kind of expression (including computed ones).
        /// The <paramref name="columnName"/> defines column name.
        /// The <paramref name="expectation"/> function is used to return expression for verifying column value.
        /// </summary>
        /// <typeparam name="TValue">Column value type.</typeparam>
        /// <param name="columnName">Column name.</param>
        /// <param name="columnExpression">Field/property accessor expression</param>
        /// <param name="expectation">Function returning column expectation expression.</param>
        /// <returns>Self</returns>
        ITableValidatorBuilder<TRow> WithColumn<TValue>(string columnName, Func<TRow, TValue> columnExpression, IExpectation<TValue> expectation);
    }
}