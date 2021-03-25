using LightBDD.Core.Execution;
using LightBDD.Core.Metadata;
using LightBDD.Core.Notification.Events;
using LightBDD.Core.Results.Parameters;

namespace LightBDD.Framework.Notification.Events
{
    /// <summary>
    /// Event raised when inline parameter is discovered.
    /// </summary>
    public class InlineParameterDiscovered : ProgressEvent
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
        public InlineParameterDiscovered(EventTime time, IParameterInfo parameter, IInlineParameterDetails details) : base(time)
        {
            Parameter = parameter;
            Details = details;
        }
    }
}