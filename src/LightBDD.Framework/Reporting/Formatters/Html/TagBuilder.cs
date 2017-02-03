using System.Collections.Generic;
using System.Linq;

namespace LightBDD.Framework.Reporting.Formatters.Html
{
    internal class TagBuilder : IHtmlNode
    {
        private readonly List<KeyValuePair<string, string>> _attributes = new List<KeyValuePair<string, string>>();
        private bool _skipEmpty;
        private IHtmlNode[] _nodes = new IHtmlNode[0];
        private bool _spaceBefore;
        private bool _spaceAfter;
        private string _tag;

        public TagBuilder(Html5Tag tag)
        {
            _tag = tag.ToString().ToLowerInvariant();
        }

        public TagBuilder Attribute(Html5Attribute attribute, string value)
        {
            return Attribute(attribute.ToString().ToLowerInvariant(), value);
        }

        public TagBuilder Attribute(string attribute, string value)
        {
            _attributes.Add(new KeyValuePair<string, string>(attribute, value));
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
            return Attribute(Html5Attribute.Class, className);
        }

        public TagBuilder Href(string href)
        {
            return Attribute(Html5Attribute.Href, href);
        }

        public TagBuilder Id(string id)
        {
            return Attribute(Html5Attribute.Id, id);
        }

        public TagBuilder For(string forTag)
        {
            return Attribute(Html5Attribute.For, forTag);
        }

        public TagBuilder Checked(bool isChecked = true)
        {
            if (!isChecked)
                return this;
            return Attribute(Html5Attribute.Checked, null);
        }

        public TagBuilder OnClick(string jsCode)
        {
            return Attribute(Html5Attribute.Onclick, jsCode);
        }

        public TagBuilder Name(string name)
        {
            return Attribute(Html5Attribute.Name, name);
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

            if (_nodes.Any())
            {
                writer.OpenTag(_tag, _attributes);

                foreach (var node in _nodes)
                    node.Write(writer);

                writer.CloseTag(_tag);
            }
            else
                writer.WriteTag(_tag, _attributes);

            if (_spaceAfter)
                writer.Write(" ");

            return writer;
        }
    }
}