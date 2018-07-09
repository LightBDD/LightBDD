using System;
using System.Linq.Expressions;

namespace LightBDD.Framework.Parameters
{
    /// <summary>
    /// Interface allowing to define <see cref="InputTable{TRow}"/>.
    /// </summary>
    /// <typeparam name="TRow">Type of table row.</typeparam>
    public interface IInputTableBuilder<TRow>
    {
        /// <summary>
        /// Defines column, based on <paramref name="columnExpression"/>.
        /// The <paramref name="columnExpression"/> is an expression used to provide column value.
        /// The <paramref name="columnExpression"/> has to be a field/property accessor expression, where the accessed member will be used for column name.
        /// </summary>
        /// <typeparam name="TValue">Column value type.</typeparam>
        /// <param name="columnExpression">Field/property accessor expression</param>
        /// <exception cref="InvalidOperationException">Thrown if <paramref name="columnExpression"/> is not member expression.</exception>
        /// <returns>Self</returns>
        IInputTableBuilder<TRow> WithColumn<TValue>(Expression<Func<TRow, TValue>> columnExpression);

        /// <summary>
        /// Defines column, based on <paramref name="columnExpression"/>.
        /// The <paramref name="columnExpression"/> is an expression used to provide column value and it can be any kind of expression (including computed ones).
        /// The <paramref name="columnName"/> defines column name.
        /// </summary>
        /// <typeparam name="TValue">Column value type.</typeparam>
        /// <param name="columnName">Column name.</param>
        /// <param name="columnExpression">Field/property accessor expression</param>
        /// <returns>Self</returns>
        IInputTableBuilder<TRow> WithColumn<TValue>(string columnName, Func<TRow, TValue> columnExpression);

        /// <summary>
        /// Instructs <see cref="IInputTableBuilder{TRow}"/> to infer columns basing on the <typeparamref name="TRow"/> type.
        /// It is possible to use this methods together with <c>WithColumn()</c> methods, where manually specified columns will override inferred columns of the same name.
        /// </summary>
        /// <returns>Self.</returns>
        IInputTableBuilder<TRow> WithInferredColumns();
    }
}