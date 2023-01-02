using System.IO;
using System.Text;
using LightBDD.Core.Metadata;
using LightBDD.Core.Results.Parameters.Trees;

namespace LightBDD.Framework.Reporting.Formatters;

internal class TextTreeRenderer
{
    public static void Render(TextWriter writer, string prefix, ITreeParameterDetails tree)
    {
        var first = true;
        foreach (var node in tree.Root.EnumerateAll())
        {
            if (!first)
                writer.WriteLine();
            writer.Write(prefix);
            writer.Write(GetNodeStatus(node));
            writer.Write(" ");
            writer.Write(node.Path);
            writer.Write(": ");
            if (node.VerificationStatus > ParameterVerificationStatus.Success)
            {
                writer.Write(Escape(node.Expectation));
                writer.Write('/');
            }
            writer.Write(Escape(node.Value));
            first = false;
        }
    }
    public static string Render(string prefix, ITreeParameterDetails tree)
    {
        var sb = new StringBuilder();
        using (var writer = new StringWriter(sb))
            Render(writer, prefix, tree);
        return sb.ToString();
    }

    private static char GetNodeStatus(ITreeParameterNodeResult node)
    {
        return node.VerificationStatus switch
        {
            ParameterVerificationStatus.NotApplicable => ' ',
            ParameterVerificationStatus.Success => '=',
            _ => '!'
        };
    }

    private static string Escape(string text)
    {
        return text.Replace("\r", "").Replace('\t', ' ').Replace("\n", " ").Replace("\b", "");
    }
}