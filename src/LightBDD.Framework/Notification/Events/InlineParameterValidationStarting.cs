using LightBDD.Core.Execution;
using LightBDD.Core.Metadata;
using LightBDD.Core.Notification.Events;

namespace LightBDD.Framework.Notification.Events
{
    public class InlineParameterValidationStarting : ProgressEvent
    {
        public IParameterInfo Parameter { get; }

        public InlineParameterValidationStarting(EventTime time, IParameterInfo parameter) : base(time)
        {
            Parameter = parameter;
        }
    }
}
