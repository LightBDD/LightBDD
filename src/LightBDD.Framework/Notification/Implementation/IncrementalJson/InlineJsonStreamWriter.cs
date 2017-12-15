using System.IO;

namespace LightBDD.Framework.Notification.Implementation.IncrementalJson
{
    internal static class InlineJsonStreamWriter
    {
        public static void WriteText(this StreamWriter writer, string name)
        {
            if (name == null)
                writer.Write("null");
            else
            {
                writer.Write('"');
                writer.Write(Escape(name));
                writer.Write('"');
            }
        }

        private static string Escape(string name)
        {
            return name
                .Replace("\\", "\\\\")
                .Replace("\b", "\\b")
                .Replace("\f", "\\f")
                .Replace("\n", "\\n")
                .Replace("\r", "\\r")
                .Replace("\t", "\\t")
                .Replace("\"", "\\\"");
        }
    }
}