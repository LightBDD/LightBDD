using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;

namespace LightBDD.Framework.Parameters.Implementation
{
    [DebuggerStepThrough]
    internal class InputTableBuilder<TRow> : AbstractTableBuilder<TRow, InputTableColumn>, IInputTableBuilder<TRow>
    {
        public InputTable<TRow> Build(IEnumerable<TRow> items)
        {
            var rows = items.ToArray();
            return new InputTable<TRow>(BuildColumns(rows), rows);
        }

        public IInputTableBuilder<TRow> WithColumn<TValue>(Expression<Func<TRow, TValue>> columnExpression)
        {
            return Add(Reflector.GetMemberName(columnExpression), columnExpression.Compile());
        }

        public IInputTableBuilder<TRow> WithColumn<TValue>(string columnName, Func<TRow, TValue> columnExpression)
        {
            return Add(columnName, columnExpression);
        }

        public IInputTableBuilder<TRow> WithInferredColumns()
        {
            InferColumns = true;
            return this;
        }

        private IInputTableBuilder<TRow> Add<TValue>(string columnName, Func<TRow, TValue> columnExpression)
        {
            AddCustomColumn(new InputTableColumn(
                columnName,
                row => ColumnValue.From(columnExpression((TRow)row))));

            return this;
        }

        protected override InputTableColumn CreateColumn(ColumnInfo columnInfo)
        {
            return InputTableColumn.FromColumnInfo(columnInfo);
        }
    }
}