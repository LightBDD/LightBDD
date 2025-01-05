using System.IO;
using LightBDD.Core.Metadata;
using LightBDD.Core.Results.Parameters.Trees;

namespace LightBDD.Framework.Reporting.Formatters.Markdown;

internal class MarkdownTreeRenderer
{
    public static void Render(TextWriter writer, ITreeParameterDetails tree)
    {
        writer.WriteLine("<div style=\"overflow-x: auto;\">");
        writer.WriteLine();
        if (tree.VerificationStatus == ParameterVerificationStatus.NotApplicable)
        {
            writer.WriteLine("| Node | Value |");
            writer.WriteLine("| ---- | ----- |");
        }
        else
        {
            writer.WriteLine("| # | Node | Value |");
            writer.WriteLine("|---| ---- | ----- |");
        }

        foreach (var node in tree.Root.EnumerateAll())
        {
            if (tree.VerificationStatus != ParameterVerificationStatus.NotApplicable)
            {
                writer.Write("| ");
                writer.Write(MarkdownStepNameDecorator.Instance.GetVerificationStatus(node.VerificationStatus));
                writer.Write(" ");
            }
            writer.Write($"| `{node.Path}` | ");
            if (node.VerificationStatus > ParameterVerificationStatus.Success)
                writer.Write($"{MarkdownFormatter.AsInlineBlock(node.Expectation)} / ");
            writer.WriteLine($"{MarkdownFormatter.AsInlineBlock(node.Value)} |");
        }
        writer.WriteLine();
        writer.WriteLine("</div>");
    }
}