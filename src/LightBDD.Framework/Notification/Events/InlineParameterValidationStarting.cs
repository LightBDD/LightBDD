using LightBDD.Core.Execution;
using LightBDD.Core.Metadata;
using LightBDD.Core.Notification.Events;

namespace LightBDD.Framework.Notification.Events
{
    /// <summary>
    /// Event raised when inline parameter validation is about to start.
    /// </summary>
    public class InlineParameterValidationStarting : ProgressEvent
    {
        /// <summary>
        /// Parameter info.
        /// </summary>
        public IParameterInfo Parameter { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public InlineParameterValidationStarting(EventTime time, IParameterInfo parameter) : base(time)
        {
            Parameter = parameter;
        }
    }
}
