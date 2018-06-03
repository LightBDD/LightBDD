using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace LightBDD.Framework.Parameters.Implementation
{
    internal class TableBuilder<TRow> : AbstractTableBuilder<TRow, TableColumn>, ITableBuilder<TRow>
    {
        public Table<TRow> Build(IEnumerable<TRow> items)
        {
            var rows = items.ToArray();
            return new Table<TRow>(BuildColumns(rows), rows);
        }

        public ITableBuilder<TRow> WithColumn<TValue>(Expression<Func<TRow, TValue>> columnExpression)
        {
            return Add(Reflector.GetMemberName(columnExpression), columnExpression.Compile());
        }

        public ITableBuilder<TRow> WithColumn<TValue>(string columnName, Func<TRow, TValue> columnExpression)
        {
            return Add(columnName, columnExpression);
        }

        public ITableBuilder<TRow> WithInferredColumns()
        {
            InferColumns = true;
            return this;
        }

        private ITableBuilder<TRow> Add<TValue>(string columnName, Func<TRow, TValue> columnExpression)
        {
            AddCustomColumn(new TableColumn(
                columnName,
                row => ColumnValue.From(columnExpression((TRow)row))));

            return this;
        }

        protected override TableColumn CreateColumn(ColumnInfo columnInfo)
        {
            return TableColumn.FromColumnInfo(columnInfo);
        }
    }
}