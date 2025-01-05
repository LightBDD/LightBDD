using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LightBDD.Core.Metadata;
using LightBDD.Core.Results.Parameters;
using LightBDD.Core.Results.Parameters.Tabular;

namespace LightBDD.Framework.Reporting.Formatters;

internal class MarkdownTableRenderer
{
    private readonly TextColumn[] _columns;
    private readonly List<TextRow> _rows = new();
    private readonly bool _renderRowStatus;

    public MarkdownTableRenderer(ITabularParameterDetails table)
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

    public void Render(TextWriter writer)
    {
        writer.WriteLine("<div style=\"overflow-x: auto;\">");
        writer.WriteLine();
        writer.Write("|");
        if (_renderRowStatus)
            writer.Write("#|");
        foreach (var column in _columns)
        {
            column.Render(writer);
            writer.Write('|');
        }
        writer.WriteLine();
        WriteHRule(writer);
        writer.WriteLine();

        foreach (var row in _rows)
        {
            row.Render(writer, _columns, _renderRowStatus);
        }
        writer.WriteLine();
        writer.WriteLine("</div>");
    }

    private void WriteHRule(TextWriter writer)
    {
        if (_renderRowStatus)
            writer.Write("|-");
        writer.Write("|");
        foreach (var column in _columns)
        {
            WriteFill(writer, '-', column.Size);
            writer.Write('|');
        }
    }

    private static void WriteFill(TextWriter writer, char c, int repeat)
    {
        while (--repeat >= 0)
            writer.Write(c);
    }

    class TextRow
    {
        private readonly List<TextCell> _cells = new();
        private readonly string _status;

        public TextRow(TableRowType rowType, ParameterVerificationStatus rowVerificationStatus)
        {
            _status = GetStatus(rowType, rowVerificationStatus);
        }

        private static string GetStatus(TableRowType type, ParameterVerificationStatus status)
        {
            switch (type)
            {
                case TableRowType.Missing:
                    return "➖";
                case TableRowType.Surplus:
                    return "➕";
            }

            return MarkdownStepNameDecorator.Instance.GetVerificationStatus(status);
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
        for (var i = size - text.Length; i > 0; --i)
            writer.Write(' ');
    }
}