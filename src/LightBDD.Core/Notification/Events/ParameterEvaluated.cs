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
        /// Parameter owner.
        /// </summary>
        public IMetadataInfo Owner { get; }

        /// <summary>
        /// Parameter.
        /// </summary>
        public IParameterResult Parameter { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public ParameterEvaluated(EventTime time, IMetadataInfo owner, IParameterResult parameter) : base(time)
        {
            Owner = owner;
            Parameter = parameter;
        }
    }
}