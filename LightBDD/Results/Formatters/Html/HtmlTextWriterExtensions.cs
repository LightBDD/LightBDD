using System;
using System.Web.UI;

namespace LightBDD.Results.Formatters.Html
{
    internal static class HtmlTextWriterExtensions
    {
        public static void WriteTag(this HtmlTextWriter writer, HtmlTextWriterTag tag, string className, Action contentRenderer)
        {
            WriteClassName(writer, className);
            writer.RenderBeginTag(tag);
            contentRenderer();
            writer.RenderEndTag();
        }

        public static void WriteTag(this HtmlTextWriter writer, Html5Tag tag, string className, Action contentRenderer)
        {
            WriteClassName(writer, className);
            writer.RenderBeginTag(tag.ToString());
            contentRenderer();
            writer.RenderEndTag();
        }

        public static void WriteTag(this HtmlTextWriter writer, HtmlTextWriterTag tag, string className, string content, bool escapeContent = true)
        {
            if (content == null)
                return;
            WriteClassName(writer,className);
            writer.RenderBeginTag(tag);

            WriteContent(writer, content, escapeContent);

            writer.RenderEndTag();
        }

        private static void WriteContent(HtmlTextWriter writer, string content, bool escapeContent)
        {
            if (escapeContent)
                writer.WriteEncodedText(content.Trim());
            else
                writer.Write(content.Trim());
        }

        private static void WriteClassName(HtmlTextWriter writer, string className)
        {
            if (className != null)
                writer.AddAttribute(HtmlTextWriterAttribute.Class, className);
        }
    }
}