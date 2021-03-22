using LightBDD.Core.Execution;
using LightBDD.Core.Metadata;
using LightBDD.Core.Notification.Events;
using LightBDD.Core.Results.Parameters;

namespace LightBDD.Framework.Notification.Events
{
    public class InlineParameterValidationFinished : ProgressEvent
    {
        public IParameterInfo Parameter { get; }
        public IInlineParameterDetails Details { get; }

        public InlineParameterValidationFinished(EventTime time, IParameterInfo parameter, IInlineParameterDetails details) : base(time)
        {
            Parameter = parameter;
            Details = details;
        }
    }
}