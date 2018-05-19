using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LightBDD.Core.Metadata;
using LightBDD.Core.Results.Parameters;

namespace LightBDD.Framework.Reporting.Formatters
{
    internal class TextTableRenderer
    {
        private readonly TextWriter _writer;
        private readonly TextColumn[] _columns;
        private readonly List<TextRow> _rows = new List<TextRow>();

        public TextTableRenderer(ITabularParameterResult table, TextWriter writer)
        {
            _writer = writer;
            _columns = table.Columns.Select(c => new TextColumn(c.Name)).ToArray();
            foreach (var row in table.Rows)
                AddRow(row);
        }

        private void AddRow(ITabularParameterRow row)
        {
            var textRow = new TextRow();
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

        public void Render()
        {
            WriteHRule();
            _writer.Write('|');
            foreach (var column in _columns)
            {
                column.Render(_writer);
                _writer.Write('|');
            }
            _writer.WriteLine();
            WriteHRule();

            foreach (var row in _rows)
                row.Render(_writer, _columns);
            WriteHRule();
        }

        private void WriteHRule()
        {
            _writer.Write('+');
            foreach (var column in _columns)
            {
                WriteFill(_writer, '-', column.Size);
                _writer.Write('+');
            }
            _writer.WriteLine();
        }

        private static void WriteFill(TextWriter writer, char c, int repeat)
        {
            while (--repeat >= 0)
                writer.Write(c);
        }

        class TextRow
        {
            private readonly List<TextCell> _cells = new List<TextCell>();
            public void Add(TextCell cell)
            {
                _cells.Add(cell);
            }

            public void Render(TextWriter writer, TextColumn[] columns)
            {
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