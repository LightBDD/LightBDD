using System;
using System.Collections.Generic;
using HtmlAgilityPack;

namespace LightBDD.Framework.UnitTests.Reporting.Formatters.Helpers;

internal class HtmlTreeToPlainTextFormatter
{
    private readonly List<string> _lines = new();

    public void FormatTree(HtmlNode root)
    {
        foreach (var treeNode in root.SelectNodes("//div[contains(@class, 'tree') and contains(@class, 'node')]"))
            _lines.Add($"{GetPath(treeNode)}={treeNode.SelectSingleNode("div[@class=\"detail\"]/*[contains(@class, 'value')]").InnerText}");
    }

    private string GetPath(HtmlNode node)
    {
        var parts = new Stack<string>();
        while (node != null)
        {
            if (node.GetAttributeValue("class", "").Contains("tree node"))
                parts.Push(node.SelectSingleNode("div[@class=\"detail\"]/*[contains(@class, 'node')]").InnerText);
            node = node.ParentNode;
        }

        return string.Join(".", parts);
    }

    public override string ToString()
    {
        return string.Join(Environment.NewLine, _lines);
    }
}