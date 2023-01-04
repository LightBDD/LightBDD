using System;
using System.Linq.Expressions;
using LightBDD.Framework.Expectations;

namespace LightBDD.Framework.Parameters
{
    /// <summary>
    /// Interface allowing to define <see cref="VerifiableTable{TRow}"/>.
    /// </summary>
    /// <typeparam name="TRow">Type of table row.</typeparam>
    public interface IVerifiableDataTableBuilder<TRow>
    {
        /// <summary>
        /// Defines column, based on <paramref name="columnExpression"/>.
        /// The <paramref name="columnExpression"/> is an expression used to provide column value.
        /// The <paramref name="columnExpression"/> has to be a field/property accessor expression, where the accessed member will be used for column name.
        /// The <c>Expect.To.Equal(value)</c> expression is used for verifying column values.
        /// </summary>
        /// <typeparam name="TValue">Column value type.</typeparam>
        /// <param name="columnExpression">Field/property accessor expression</param>
        /// <exception cref="InvalidOperationException">Thrown if <paramref name="columnExpression"/> is not member expression.</exception>
        /// <returns>Self</returns>
        IVerifiableDataTableBuilder<TRow> WithColumn<TValue>(Expression<Func<TRow, TValue>> columnExpression);

        /// <summary>
        /// Defines column, based on <paramref name="columnExpression"/>.
        /// The <paramref name="columnExpression"/> is an expression used to provide column value.
        /// The <paramref name="columnExpression"/> has to be a field/property accessor expression, where the accessed member will be used for column name.
        /// The <paramref name="expectationFn"/> function is used to return expression for verifying column value.
        /// </summary>
        /// <typeparam name="TValue">Column value type.</typeparam>
        /// <param name="columnExpression">Field/property accessor expression</param>
        /// <param name="expectationFn">Function returning column expectation expression.</param>
        /// <exception cref="InvalidOperationException">Thrown if <paramref name="columnExpression"/> is not member expression.</exception>
        /// <returns>Self</returns>
        IVerifiableDataTableBuilder<TRow> WithColumn<TValue>(Expression<Func<TRow, TValue>> columnExpression, Func<TValue, IExpectation<TValue>> expectationFn);

        /// <summary>
        /// Defines column, based on <paramref name="columnExpression"/>.
        /// The <paramref name="columnExpression"/> is an expression used to provide column value and it can be any kind of expression (including computed ones).
        /// The <paramref name="columnName"/> defines column name.
        /// The <paramref name="expectationFn"/> function is used to return expression for verifying column value.
        /// </summary>
        /// <typeparam name="TValue">Column value type.</typeparam>
        /// <param name="columnName">Column name.</param>
        /// <param name="columnExpression">Field/property accessor expression</param>
        /// <param name="expectationFn">Function returning column expectation expression.</param>
        /// <returns>Self</returns>
        IVerifiableDataTableBuilder<TRow> WithColumn<TValue>(string columnName, Func<TRow, TValue> columnExpression, Func<TValue, IExpectation<TValue>> expectationFn);

        /// <summary>
        /// Defines column, based on <paramref name="columnExpression"/>.
        /// The <paramref name="columnExpression"/> is an expression used to provide column value and it can be any kind of expression (including computed ones).
        /// The <paramref name="columnName"/> defines column name.
        /// The <c>Expect.To.Equal(value)</c> expression is used for verifying column values.
        /// </summary>
        /// <typeparam name="TValue">Column value type.</typeparam>
        /// <param name="columnName">Column name.</param>
        /// <param name="columnExpression">Field/property accessor expression</param>
        /// <returns>Self</returns>
        IVerifiableDataTableBuilder<TRow> WithColumn<TValue>(string columnName, Func<TRow, TValue> columnExpression);

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
        IVerifiableDataTableBuilder<TRow> WithColumn<TValue>(Expression<Func<TRow, TValue>> columnExpression, IExpectation<TValue> expectation);

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
        IVerifiableDataTableBuilder<TRow> WithColumn<TValue>(string columnName, Func<TRow, TValue> columnExpression, IExpectation<TValue> expectation);

        /// <summary>
        /// Defines key column, based on <paramref name="columnExpression"/>.
        /// The <paramref name="columnExpression"/> is an expression used to provide column value.
        /// The <paramref name="columnExpression"/> has to be a field/property accessor expression, where the accessed member will be used for column name.
        /// The <c>Expect.To.Equal(value)</c> expression is used for verifying column values.
        /// </summary>
        /// <typeparam name="TValue">Column value type.</typeparam>
        /// <param name="columnExpression">Field/property accessor expression</param>
        /// <exception cref="InvalidOperationException">Thrown if <paramref name="columnExpression"/> is not member expression.</exception>
        /// <returns>Self</returns>
        IVerifiableDataTableBuilder<TRow> WithKey<TValue>(Expression<Func<TRow, TValue>> columnExpression);

        /// <summary>
        /// Defines key column, based on <paramref name="columnExpression"/>.
        /// The <paramref name="columnExpression"/> is an expression used to provide column value.
        /// The <paramref name="columnExpression"/> has to be a field/property accessor expression, where the accessed member will be used for column name.
        /// The <paramref name="expectationFn"/> function is used to return expression for verifying column value.
        /// </summary>
        /// <typeparam name="TValue">Column value type.</typeparam>
        /// <param name="columnExpression">Field/property accessor expression</param>
        /// <param name="expectationFn">Function returning column expectation expression.</param>
        /// <exception cref="InvalidOperationException">Thrown if <paramref name="columnExpression"/> is not member expression.</exception>
        /// <returns>Self</returns>
        IVerifiableDataTableBuilder<TRow> WithKey<TValue>(Expression<Func<TRow, TValue>> columnExpression, Func<TValue, IExpectation<TValue>> expectationFn);

        /// <summary>
        /// Defines key column, based on <paramref name="columnExpression"/>.
        /// The <paramref name="columnExpression"/> is an expression used to provide column value and it can be any kind of expression (including computed ones).
        /// The <paramref name="columnName"/> defines column name.
        /// The <paramref name="expectationFn"/> function is used to return expression for verifying column value.
        /// </summary>
        /// <typeparam name="TValue">Column value type.</typeparam>
        /// <param name="columnName">Column name.</param>
        /// <param name="columnExpression">Field/property accessor expression</param>
        /// <param name="expectationFn">Function returning column expectation expression.</param>
        /// <returns>Self</returns>
        IVerifiableDataTableBuilder<TRow> WithKey<TValue>(string columnName, Func<TRow, TValue> columnExpression, Func<TValue, IExpectation<TValue>> expectationFn);

        /// <summary>
        /// Defines key column, based on <paramref name="columnExpression"/>.
        /// The <paramref name="columnExpression"/> is an expression used to provide column value and it can be any kind of expression (including computed ones).
        /// The <paramref name="columnName"/> defines column name.
        /// The <c>Expect.To.Equal(value)</c> expression is used for verifying column values.
        /// </summary>
        /// <typeparam name="TValue">Column value type.</typeparam>
        /// <param name="columnName">Column name.</param>
        /// <param name="columnExpression">Field/property accessor expression</param>
        /// <returns>Self</returns>
        IVerifiableDataTableBuilder<TRow> WithKey<TValue>(string columnName, Func<TRow, TValue> columnExpression);

        /// <summary>
        /// Defines key column, based on <paramref name="columnExpression"/>.
        /// The <paramref name="columnExpression"/> is an expression used to provide column value.
        /// The <paramref name="columnExpression"/> has to be a field/property accessor expression, where the accessed member will be used for column name.
        /// The <paramref name="expectation"/> function is used to return expression for verifying column value.
        /// </summary>
        /// <typeparam name="TValue">Column value type.</typeparam>
        /// <param name="columnExpression">Field/property accessor expression</param>
        /// <param name="expectation">Function returning column expectation expression.</param>
        /// <exception cref="InvalidOperationException">Thrown if <paramref name="columnExpression"/> is not member expression.</exception>
        /// <returns>Self</returns>
        IVerifiableDataTableBuilder<TRow> WithKey<TValue>(Expression<Func<TRow, TValue>> columnExpression, IExpectation<TValue> expectation);

        /// <summary>
        /// Defines key column, based on <paramref name="columnExpression"/>.
        /// The <paramref name="columnExpression"/> is an expression used to provide column value and it can be any kind of expression (including computed ones).
        /// The <paramref name="columnName"/> defines column name.
        /// The <paramref name="expectation"/> function is used to return expression for verifying column value.
        /// </summary>
        /// <typeparam name="TValue">Column value type.</typeparam>
        /// <param name="columnName">Column name.</param>
        /// <param name="columnExpression">Field/property accessor expression</param>
        /// <param name="expectation">Function returning column expectation expression.</param>
        /// <returns>Self</returns>
        IVerifiableDataTableBuilder<TRow> WithKey<TValue>(string columnName, Func<TRow, TValue> columnExpression, IExpectation<TValue> expectation);


        /// <summary>
        /// Instructs <see cref="IInputTableBuilder{TRow}"/> to infer columns basing on the <typeparamref name="TRow"/> type.
        /// It is possible to use this methods together with <c>WithColumn() / WithKey()</c> methods, where manually specified columns will override inferred columns of the same name.
        /// The inferred columns will always use <c>Expect.To.Equal(value)</c> expression for verifying column values.
        /// </summary>
        /// <returns>Self.</returns>
        IVerifiableDataTableBuilder<TRow> WithInferredColumns();

        /// <summary>
        /// Instructs <see cref="IInputTableBuilder{TRow}"/> to infer columns basing on the <typeparamref name="TRow"/> type.
        /// It is possible to use this methods together with <c>WithColumn() / WithKey()</c> methods, where manually specified columns will override inferred columns of the same name.
        /// The inferred columns will always use <c>Expect.To.Equal(value)</c> expression for verifying column values.
        /// </summary>
        /// <param name="columnsOrder">Order of the columns</param>
        /// <returns>Self.</returns>
        IVerifiableDataTableBuilder<TRow> WithInferredColumns(InferredColumnsOrder columnsOrder);
    }
}