using System.IO;
using System.Net;
using System.Text;
using LightBDD.Core.Formatting.NameDecorators;
using LightBDD.Core.Metadata;

namespace LightBDD.Framework.Reporting.Formatters.Html
{
    internal class HtmlStepNameDecorator : IStepNameDecorator
    {
        public string DecorateStepTypeName(IStepTypeNameInfo stepTypeName)
        {
            return AsString(Html.Tag(Html5Tag.Span).Class("stepType").Content(stepTypeName.Name));
        }

        public string DecorateParameterValue(INameParameterInfo parameter)
        {
            return AsString(
                Html.Tag(Html5Tag.Span)
                    .Class(parameter.IsEvaluated ? "stepParam" : "stepParamUnknown")
                    .Content(parameter.FormattedValue));
        }

        public string DecorateNameFormat(string nameFormat)
        {
            return WebUtility.HtmlEncode(nameFormat);
        }

        private static string AsString(IHtmlNode node)
        {
            var sb = new StringBuilder();
            using (var writer = new HtmlTextWriter(new StringWriter(sb)))
                node.Write(writer);
            return sb.ToString();
        }
    }
}