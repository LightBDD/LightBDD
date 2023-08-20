using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;

namespace LightBDD.Framework.UnitTests.Reporting.Formatters.Helpers;

internal class HtmlToPlainTextFormatter
{
    private readonly IEnumerable<string> _blockElements = new[] { "div", "tr", "table", "section", "article", "h1", "h2", "h3", "br" };
    private readonly IEnumerable<string> _inlineElements = new[] { "td", "th" };//browsers are treating td/th in special way while for span they put no spaces when copied to clipboard
    private readonly List<string> _lines = new();
    private readonly StringBuilder _current = new();

    public void FormatNode(HtmlNode node)
    {
        EnsureSeparatorFor(node);

        if (node.Name == "hr")
            Append("/");
        else if (node.Name is "#text" or "#comment")
            Append(node.InnerText);

        foreach (var childNode in node.ChildNodes)
            FormatNode(childNode);

        EnsureSeparatorFor(node);
    }

    private void EnsureNewLine()
    {
        if (_current.Length > 0)
        {
            _lines.Add(_current.ToString().TrimEnd());
            _current.Clear();
        }
    }

    private void EnsureSpace()
    {
        if (_current.Length > 0 && !char.IsWhiteSpace(_current[_current.Length - 1]))
            _current.Append(' ');
    }

    private void EnsureSeparatorFor(HtmlNode node)
    {
        var name = node.Name.ToLowerInvariant();
        if (_blockElements.Contains(name))
            EnsureNewLine();
        else if (_inlineElements.Contains(name))
            EnsureSpace();
    }

    private void Append(string text)
    {
        _current.Append(text);
    }

    public override string ToString()
    {
        EnsureNewLine();//to flush current
        return string.Join(Environment.NewLine, _lines);
    }
}