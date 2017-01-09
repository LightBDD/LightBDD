
namespace LightBDD.SummaryGeneration.Formatters.Html
{
    internal class TextBuilder : IHtmlNode
    {
        private string _text;
        private bool _escape;

        public TextBuilder(string text)
        {
            _text = text ?? string.Empty;
        }

        public TextBuilder Trim()
        {
            _text = _text.Trim();
            return this;
        }

        public TextBuilder Escape(bool escape = true)
        {
            _escape = escape;
            return this;
        }

        public HtmlTextWriter Write(HtmlTextWriter writer)
        {
            if (_escape)
                writer.WriteEncodedText(_text);
            else
                writer.Write(_text);
            return writer;
        }
    }
}