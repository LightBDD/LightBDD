using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LightBDD.Core.Execution;
using LightBDD.Core.Formatting.Values;
using LightBDD.Core.Metadata;
using LightBDD.Core.Notification;
using LightBDD.Core.Results.Parameters;
using LightBDD.Core.Results.Parameters.Tabular;
using LightBDD.Framework.Formatting.Values;
using LightBDD.Framework.Notification.Events;
using LightBDD.Framework.Results.Implementation;

namespace LightBDD.Framework.Parameters
{
    /// <summary>
    /// Type representing tabular step parameter.
    /// When used in step methods, the tabular representation of the parameter will be rendered in reports and progress notification.<br/>
    ///
    /// Beside special rendering, the table behaves as a standard collection, i.e it offers <see cref="Count"/>, <see cref="GetEnumerator()"/> and indexing operator members.<br/>
    ///
    /// Please see <see cref="Parameters.Table"/> type to learn how to create tables effectively.
    /// </summary>
    /// <typeparam name="TRow">Row type.</typeparam>
    public class InputTable<TRow> : IComplexParameter, ISelfFormattable, IReadOnlyList<TRow>, ITraceableParameter
    {
        private readonly IReadOnlyList<TRow> _rows;
        private readonly InputTableColumn[] _columns;
        private IValueFormattingService _formattingService = ValueFormattingServices.Current;
        private TabularParameterDetails _details;

        /// <summary>
        /// Constructor creating table with specified <paramref name="columns"/> and <paramref name="rows"/>
        /// </summary>
        /// <param name="columns">Table columns.</param>
        /// <param name="rows">Table rows.</param>
        public InputTable(IEnumerable<InputTableColumn> columns, IReadOnlyList<TRow> rows)
        {
            _rows = rows;
            _columns = columns.ToArray();
        }

        void IComplexParameter.SetValueFormattingService(IValueFormattingService formattingService)
        {
            _formattingService = formattingService;
        }

        IParameterDetails IComplexParameter.Details => GetDetails();
        private TabularParameterDetails GetDetails() => _details ??= new TabularParameterDetails(GetColumns(), GetRows());

        private IEnumerable<ITabularParameterRow> GetRows()
        {
            return _rows.Select(GetRow);
        }

        private ITabularParameterRow GetRow(TRow row, int index)
        {
            return new TabularParameterRow(index, _columns.Select(x => _formattingService.FormatValue(x.GetValue(row))));
        }

        private IEnumerable<ITabularParameterColumn> GetColumns()
        {
            return _columns.Select(x => new TabularParameterColumn(x.Name, false));
        }

        /// <summary>
        /// Returns inline representation of table.
        /// </summary>
        public string Format(IValueFormattingService formattingService)
        {
            return "<table>";
        }

        /// <summary>
        /// Returns number of rows.
        /// </summary>
        public int Count => _rows.Count;

        /// <summary>
        /// Returns enumerator for the table rows.
        /// </summary>
        public IEnumerator<TRow> GetEnumerator()
        {
            return _rows.AsEnumerable().GetEnumerator();
        }

        /// <summary>
        /// Returns row at specified <paramref name="index"/>.
        /// </summary>
        /// <param name="index">Row index</param>
        public TRow this[int index] => _rows[index];

        /// <summary>
        /// Returns columns collection.
        /// </summary>
        public IReadOnlyList<InputTableColumn> Columns => _columns;

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        void ITraceableParameter.InitializeParameterTrace(IParameterInfo parameterInfo, IProgressPublisher progressPublisher)
        {
            progressPublisher?.Publish(time => new TabularParameterDiscovered(time, parameterInfo, GetDetails()));
        }
    }
}