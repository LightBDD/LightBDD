using System.IO;
using System.Text;
using LightBDD.Core.Metadata;
using LightBDD.Core.Results.Parameters.Trees;

namespace LightBDD.Framework.Reporting.Formatters;

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
            writer.Write("| ");
            writer.Write(node.Path);
            writer.Write(" | ");
            if (node.VerificationStatus > ParameterVerificationStatus.Success)
            {
                writer.Write(Escape(node.Expectation));
                writer.Write('/');
            }
            writer.Write(Escape(node.Value));
            writer.WriteLine("|");
        }
        writer.WriteLine();
        writer.WriteLine("</div>");
    }
    public static string Render(ITreeParameterDetails tree)
    {
        var sb = new StringBuilder();
        using (var writer = new StringWriter(sb))
            Render(writer, tree);
        return sb.ToString();
    }

    private static string Escape(string text)
    {
        return text.Replace("\r", "").Replace('\t', ' ').Replace("\n", " ").Replace("\b", "");
    }
}