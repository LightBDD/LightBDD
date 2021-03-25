using LightBDD.Core.Execution;
using LightBDD.Core.Metadata;
using LightBDD.Core.Notification.Events;
using LightBDD.Core.Results.Parameters.Tabular;

namespace LightBDD.Framework.Notification.Events
{
    /// <summary>
    /// Event raised when tabular parameter validation is finished.
    /// </summary>
    public class TabularParameterValidationFinished : ProgressEvent
    {
        /// <summary>
        /// Parameter info.
        /// </summary>
        public IParameterInfo Parameter { get; }
        /// <summary>
        /// Parameter details.
        /// </summary>
        public ITabularParameterDetails Details { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public TabularParameterValidationFinished(EventTime time, IParameterInfo parameter, ITabularParameterDetails details) : base(time)
        {
            Parameter = parameter;
            Details = details;
        }
    }
}