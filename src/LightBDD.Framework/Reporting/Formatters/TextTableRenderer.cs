using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LightBDD.Core.Metadata;
using LightBDD.Core.Results.Parameters;
using LightBDD.Core.Results.Parameters.Tabular;

namespace LightBDD.Framework.Reporting.Formatters
{
    internal class TextTableRenderer
    {
        private readonly TextColumn[] _columns;
        private readonly List<TextRow> _rows = new List<TextRow>();
        private readonly bool _renderRowStatus;

        public TextTableRenderer(ITabularParameterDetails table)
        {
            _renderRowStatus = table.VerificationStatus != ParameterVerificationStatus.NotApplicable;

            _columns = table.Columns.Select(c => new TextColumn(c.Name)).ToArray();
            foreach (var row in table.Rows)
                AddRow(row);
        }

        private void AddRow(ITabularParameterRow row)
        {
            var textRow = new TextRow(row.Type, row.VerificationStatus);
            for (var i = 0; i < row.Values.Count; i++)
            {
                var cell = new TextCell(row.Values[i]);
                textRow.Add(cell);
                _columns[i].EnsureFit(cell.Text);
            }
            _rows.Add(textRow);
        }

        private struct TextCell
        {
            public TextCell(IValueResult result)
            {
                Text = Escape(result.Value);

                if (result.VerificationStatus != ParameterVerificationStatus.NotApplicable &&
                    result.VerificationStatus != ParameterVerificationStatus.Success)
                    Text += "/" + Escape(result.Expectation);
            }

            public string Text { get; }
        }

        public string Render(string prefix)
        {
            var builder = new StringBuilder();
            using (var writer = new StringWriter(builder))
                Render(writer, prefix);
            return builder.ToString();
        }

        public void Render(TextWriter writer, string prefix)
        {
            WriteHRule(writer, prefix);
            writer.WriteLine();
            writer.Write(prefix);
            writer.Write("|");
            if (_renderRowStatus)
                writer.Write("#|");
            foreach (var column in _columns)
            {
                column.Render(writer);
                writer.Write('|');
            }
            writer.WriteLine();
            WriteHRule(writer, prefix);
            writer.WriteLine();

            foreach (var row in _rows)
            {
                writer.Write(prefix);
                row.Render(writer, _columns, _renderRowStatus);
            }
            WriteHRule(writer, prefix);
        }

        private void WriteHRule(TextWriter writer, string prefix)
        {
            writer.Write(prefix);
            if (_renderRowStatus)
                writer.Write("+-");
            writer.Write("+");
            foreach (var column in _columns)
            {
                WriteFill(writer, '-', column.Size);
                writer.Write('+');
            }
        }

        private static void WriteFill(TextWriter writer, char c, int repeat)
        {
            while (--repeat >= 0)
                writer.Write(c);
        }

        class TextRow
        {
            private readonly List<TextCell> _cells = new List<TextCell>();
            private readonly char _status;

            public TextRow(TableRowType rowType, ParameterVerificationStatus rowVerificationStatus)
            {
                _status = GetStatus(rowType, rowVerificationStatus);
            }

            private static char GetStatus(TableRowType type, ParameterVerificationStatus status)
            {
                switch (type)
                {
                    case TableRowType.Missing:
                        return '-';
                    case TableRowType.Surplus:
                        return '+';
                }

                switch (status)
                {
                    case ParameterVerificationStatus.Success:
                        return '=';
                    case ParameterVerificationStatus.NotApplicable:
                        return ' ';
                }

                return '!';
            }

            public void Add(TextCell cell)
            {
                _cells.Add(cell);
            }

            public void Render(TextWriter writer, TextColumn[] columns, bool renderRowStatus)
            {
                if (renderRowStatus)
                {
                    writer.Write('|');
                    writer.Write(_status);
                }
                writer.Write('|');
                for (var i = 0; i < _cells.Count; i++)
                {
                    WritePadded(writer, _cells[i].Text, columns[i].Size);
                    writer.Write('|');
                }
                writer.WriteLine();
            }
        }
        class TextColumn
        {
            private readonly string _name;
            public int Size { get; private set; }

            public TextColumn(string name)
            {
                _name = Escape(name);
                EnsureFit(_name);
            }

            public void EnsureFit(string text)
            {
                Size = Math.Max(Size, text?.Length ?? 0);
            }

            public void Render(TextWriter writer)
            {
                WritePadded(writer, _name, Size);
            }
        }
        private static string Escape(string text)
        {
            return text.Replace("\r", "").Replace('\t', ' ').Replace("\n", " ").Replace("\b", "");
        }

        private static void WritePadded(TextWriter writer, string text, int size)
        {
            writer.Write(text);
            for (int i = size - text.Length; i > 0; --i)
                writer.Write(' ');
        }
    }
}