using LightBDD.Core.Execution;
using LightBDD.Core.Metadata;
using LightBDD.Core.Notification.Events;
using LightBDD.Core.Results.Parameters;

namespace LightBDD.Framework.Notification.Events
{
    /// <summary>
    /// Event raised when inline parameter validation is finished.
    /// </summary>
    public class InlineParameterValidationFinished : ProgressEvent
    {
        /// <summary>
        /// Parameter info.
        /// </summary>
        public IParameterInfo Parameter { get; }
        /// <summary>
        /// Parameter details.
        /// </summary>
        public IInlineParameterDetails Details { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public InlineParameterValidationFinished(EventTime time, IParameterInfo parameter, IInlineParameterDetails details) : base(time)
        {
            Parameter = parameter;
            Details = details;
        }
    }
}