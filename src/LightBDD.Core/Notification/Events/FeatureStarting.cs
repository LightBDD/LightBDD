using LightBDD.Core.Execution;
using LightBDD.Core.Metadata;

namespace LightBDD.Core.Notification.Events
{
    //TODO: move to Core.Events
    /// <summary>
    /// Event raised when feature execution is about to start.
    /// </summary>
    public class FeatureStarting : ProgressEvent
    {
        /// <summary>
        /// Feature.
        /// </summary>
        public IFeatureInfo Feature { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public FeatureStarting(EventTime time, IFeatureInfo feature) : base(time)
        {
            Feature = feature;
        }
    }
}