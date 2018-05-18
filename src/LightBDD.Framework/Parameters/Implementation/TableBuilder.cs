using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace LightBDD.Framework.Parameters.Implementation
{
    internal class TableBuilder<TRow> : ITableBuilder<TRow>
    {
        private readonly List<TableColumn> _columns = new List<TableColumn>();
        public Table<TRow> Build(IEnumerable<TRow> items)
        {
            return new Table<TRow>(items.ToArray(), _columns);
        }

        public ITableBuilder<TRow> WithColumn<TValue>(Expression<Func<TRow, TValue>> columnExpression)
        {
            return Add(Reflector.GetMemberName(columnExpression), columnExpression.Compile());
        }

        public ITableBuilder<TRow> WithColumn<TValue>(string columnName, Func<TRow, TValue> columnExpression)
        {
            return Add(columnName, columnExpression);
        }

        private ITableBuilder<TRow> Add<TValue>(string columnName, Func<TRow, TValue> columnExpression)
        {
            _columns.Add(new TableColumn(
                columnName,
                row => ColumnValue.From(columnExpression((TRow)row))));
            return this;
        }
    }
}