using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;

namespace LightBDD.Results.Formatters.Html
{
    internal class TagBuilder : IHtmlNode
    {
        private readonly List<Action<HtmlTextWriter>> _attributeWriters = new List<Action<HtmlTextWriter>>();
        private readonly Action<HtmlTextWriter> _renderBeginTag;
        private bool _skipEmpty;
        private IHtmlNode[] _nodes = new IHtmlNode[0];
        private bool _spaceBefore;
        private bool _spaceAfter;

        internal TagBuilder(Action<HtmlTextWriter> renderBeginTag)
        {
            _renderBeginTag = renderBeginTag;
        }

        public TagBuilder Attribute(HtmlTextWriterAttribute attribute, string value)
        {
            _attributeWriters.Add(w => w.AddAttribute(attribute, value));
            return this;
        }

        public TagBuilder Attribute(string attribute, string value)
        {
            _attributeWriters.Add(w => w.AddAttribute(attribute, value));
            return this;
        }

        public TagBuilder Content(params IHtmlNode[] nodes)
        {
            _nodes = nodes;
            return this;
        }

        public TagBuilder Content(IEnumerable<IHtmlNode> nodes)
        {
            _nodes = nodes.ToArray();
            return this;
        }

        public TagBuilder Content(string content, bool trimContent = true, bool escapeContent = true)
        {
            content = content ?? string.Empty;
            if (trimContent)
                content = content.Trim();

            if (content == string.Empty)
                return Content();

            return Content(Html.Text(content).Escape(escapeContent));
        }

        public TagBuilder SkipEmpty()
        {
            _skipEmpty = true;
            return this;
        }

        public TagBuilder Class(string className)
        {
            return Attribute(HtmlTextWriterAttribute.Class, className);
        }

        public TagBuilder Href(string href)
        {
            return Attribute(HtmlTextWriterAttribute.Href, href);
        }

        public TagBuilder Id(string id)
        {
            return Attribute(HtmlTextWriterAttribute.Id, id);
        }

        public TagBuilder For(string forTag)
        {
            return Attribute(HtmlTextWriterAttribute.For, forTag);
        }

        public TagBuilder Checked()
        {
            return Attribute(HtmlTextWriterAttribute.Checked, null);
        }

        public TagBuilder SpaceBefore()
        {
            _spaceBefore = true;
            return this;
        }

        public TagBuilder SpaceAfter()
        {
            _spaceAfter = true;
            return this;
        }

        public HtmlTextWriter Write(HtmlTextWriter writer)
        {
            if (_skipEmpty && _nodes.Length == 0)
                return writer;

            if (_spaceBefore)
                writer.Write(" ");

            foreach (var writeAttribute in _attributeWriters)
                writeAttribute(writer);

            _renderBeginTag(writer);

            foreach (var node in _nodes)
                node.Write(writer);

            writer.RenderEndTag();

            if (_spaceAfter)
                writer.Write(" ");

            return writer;
        }
    }
}