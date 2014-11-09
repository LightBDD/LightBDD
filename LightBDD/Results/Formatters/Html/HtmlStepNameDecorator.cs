using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using LightBDD.Naming;

namespace LightBDD.Results.Formatters.Html
{
    internal class HtmlStepNameDecorator : IStepNameDecorator
    {
        public string DecorateStepTypeName(string stepTypeName)
        {
            return AsString(Html.Tag(HtmlTextWriterTag.Span).Class("stepType").Content(stepTypeName));
        }

        public string DecorateParameterValue(IStepParameter parameter)
        {
            return AsString(
                Html.Tag(HtmlTextWriterTag.Span)
                    .Class(parameter.IsEvaluated ? "stepParam" : "stepParamUnknown")
                    .Content(parameter.FormattedValue));
        }

        public string DecorateNameFormat(string nameFormat)
        {
            return HttpUtility.HtmlEncode(nameFormat);
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