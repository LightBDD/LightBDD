namespace LightBDD.SummaryGeneration.Formatters.Html
{
    internal static class Html
    {
        public static TagBuilder Checkbox()
        {
            return Tag(Html5Tag.Input).Attribute(Html5Attribute.Type, "checkbox");
        }

        public static TagBuilder Radio()
        {
            return Tag(Html5Tag.Input).Attribute(Html5Attribute.Type, "radio");
        }

        public static TagBuilder Tag(Html5Tag tag)
        {
            return new TagBuilder(tag);
        }

        public static TextBuilder Text(string text)
        {
            return new TextBuilder(text);
        }

        public static IHtmlNode Br()
        {
            return Text("<br/>");
        }

        public static IHtmlNode Nothing()
        {
            return Tag(Html5Tag.Div).SkipEmpty();
        }
    }
}