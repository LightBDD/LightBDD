using System;
using System.Web.UI;

namespace LightBDD.Results.Formatters.Html
{
    internal static class HtmlTextWriterExtensions
    {
        public static HtmlTextWriter WriteTag(this HtmlTextWriter writer, HtmlTextWriterTag tag, string className, Action contentRenderer)
        {
            writer.WriteClassName(className);
            writer.RenderBeginTag(tag);
            contentRenderer();
            writer.RenderEndTag();
            return writer;
        }

        public static HtmlTextWriter WriteTag(this HtmlTextWriter writer, Html5Tag tag, string className, Action contentRenderer)
        {
            writer.WriteClassName(className);
            writer.RenderBeginTag(tag.ToString());
            contentRenderer();
            writer.RenderEndTag();
            return writer;
        }

        public static HtmlTextWriter WriteCheckbox(this HtmlTextWriter writer, string className,bool isChecked)
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Type, "checkbox");
            writer.WriteClassName(className);
            if (isChecked)
                writer.AddAttribute(HtmlTextWriterAttribute.Checked, null);
            writer.RenderBeginTag(HtmlTextWriterTag.Input);
            writer.RenderEndTag();
            return writer;
        }

        public static HtmlTextWriter WriteCheckbox(this HtmlTextWriter writer, string id, string label, bool isChecked)
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
            return writer;
        }

        public static HtmlTextWriter WriteLink(this HtmlTextWriter writer, string href, string content)
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Href, href);
            writer.RenderBeginTag(HtmlTextWriterTag.A);
            writer.WriteContent(content);
            writer.RenderEndTag();
            return writer;
        }

        public static HtmlTextWriter WriteTag(this HtmlTextWriter writer, HtmlTextWriterTag tag, string className, string content, bool escapeContent = true)
        {
            if (content == null)
                return writer;
            writer.WriteClassName(className);
            writer.RenderBeginTag(tag);

            writer.WriteContent(content, escapeContent);

            writer.RenderEndTag();
            return writer;
        }

        public static HtmlTextWriter WriteSpace(this HtmlTextWriter writer)
        {
            writer.WriteEncodedText(" ");
            return writer;
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