using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;

namespace LightBDD.Framework.Reporting.UnitTests.Formatters.Helpers;

internal class HtmlTableToPlainTextFormatter
{
    private readonly IEnumerable<string> _rowElements = new[] { "tr", "table" };
    private readonly IEnumerable<string> _colElements = new[] { "td", "th" };//browsers are treating td/th in special way while for span they put no spaces when copied to clipboard
    private readonly List<string> _lines = new List<string>();
    private readonly StringBuilder _current = new StringBuilder();

    public void FormatNode(HtmlNode node)
    {
        EnsureSeparatorFor(node);
        if (node.Name == "#text")
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

    private void EnsureCharacter(char c)
    {
        if (_current.Length > 0 && _current[_current.Length - 1] != c)
            _current.Append(c);
    }

    private void EnsureSeparatorFor(HtmlNode node)
    {
        var name = node.Name.ToLowerInvariant();
        if (_rowElements.Contains(name))
            EnsureNewLine();
        else if (_colElements.Contains(name))
            EnsureCharacter('|');
        else if (name == "hr")
            EnsureCharacter('/');
        else EnsureCharacter(' ');
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