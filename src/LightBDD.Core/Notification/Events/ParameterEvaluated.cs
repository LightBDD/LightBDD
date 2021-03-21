using LightBDD.Core.Execution;
using LightBDD.Core.Metadata;
using LightBDD.Core.Results.Parameters;

namespace LightBDD.Core.Notification.Events
{
    /// <summary>
    /// Event raised when step is discovered.
    /// </summary>
    public class ParameterEvaluated : ProgressEvent
    {
        /// <summary>
        /// Parameter.
        /// </summary>
        public IParameterResult Result { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public ParameterEvaluated(EventTime time, IParameterResult result) : base(time)
        {
            Result = result;
        }
    }
}