using System;
using System.Web.UI;

namespace LightBDD.Results.Formatters.Html
{
    internal static class HtmlTextWriterExtensions
    {
        public static void WriteTag(this HtmlTextWriter writer, HtmlTextWriterTag tag, string className, Action contentRenderer)
        {
            writer.WriteClassName(className);
            writer.RenderBeginTag(tag);
            contentRenderer();
            writer.RenderEndTag();
        }

        public static void WriteTag(this HtmlTextWriter writer, Html5Tag tag, string className, Action contentRenderer)
        {
            writer.WriteClassName(className);
            writer.RenderBeginTag(tag.ToString());
            contentRenderer();
            writer.RenderEndTag();
        }

        public static void WriteCheckbox(this HtmlTextWriter writer, string id, string label, bool isChecked)
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Type, "checkbox");
            writer.AddAttribute(HtmlTextWriterAttribute.Id, id);
            if (isChecked)
                writer.AddAttribute(HtmlTextWriterAttribute.Checked, null);
            writer.RenderBeginTag(HtmlTextWriterTag.Input);
            writer.RenderEndTag();

            writer.AddAttribute(HtmlTextWriterAttribute.For, id);
            writer.RenderBeginTag(HtmlTextWriterTag.Label);
            writer.WriteContent(label);
            writer.RenderEndTag();
        }

        public static void WriteTag(this HtmlTextWriter writer, HtmlTextWriterTag tag, string className, string content, bool escapeContent = true)
        {
            if (content == null)
                return;
            writer.WriteClassName(className);
            writer.RenderBeginTag(tag);

            writer.WriteContent(content, escapeContent);

            writer.RenderEndTag();
        }

        private static void WriteContent(this HtmlTextWriter writer, string content, bool escapeContent = true)
        {
            if (escapeContent)
                writer.WriteEncodedText(content.Trim());
            else
                writer.Write(content.Trim());
        }

        private static void WriteClassName(this HtmlTextWriter writer, string className)
        {
            if (className != null)
                writer.AddAttribute(HtmlTextWriterAttribute.Class, className);
        }
    }
}