using System;

namespace LightBDD.Framework.Reporting.Formatters.Markdown;

internal class MarkdownFormatter
{
    public static string AsInlineBlock(string text)
    {
        text = EscapeToInline(text);
        var backticks = CountBackticks(text);

        if (backticks == 0) 
            return $"`{text}`";
        var pad = new string('`', backticks + 1);
        return $"{pad}{text}{pad}";
    }

    private static int CountBackticks(string text)
    {
        var max = 0;
        var count = 0;
        foreach (var c in text)
        {
            if (c == '`')
                ++count;
            else
            {
                max = Math.Max(max, count);
                count = 0;
            }
        }

        return max;
    }

    public static string EscapeToInline(string text)
    {
        return text.Replace("\r", "").Replace('\t', ' ').Replace("\n", " ").Replace("\b", "");
    }
}