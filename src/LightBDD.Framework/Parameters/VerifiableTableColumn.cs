using System;
using LightBDD.Core.Formatting.Values;
using LightBDD.Framework.Expectations;

namespace LightBDD.Framework.Parameters
{
    public class VerifiableTableColumn<TRow> : TableColumn<TRow>
    {
        public Func<TRow, TRow, IValueFormattingService, ExpectationResult> Verify { get; }

        public VerifiableTableColumn(string name, bool isKey, Func<TRow, ColumnValue> getValue, Func<TRow, TRow, IValueFormattingService, ExpectationResult> verify) : base(name, isKey, getValue)
        {
            Verify = verify;
        }
    }
}