using LightBDD.Core.Execution;
using LightBDD.Core.Metadata;

namespace LightBDD.Core.Notification.Events
{
    /// <summary>
    /// Event raised when feature is discovered.
    /// </summary>
    public class FeatureDiscovered : ProgressEvent
    {
        /// <summary>
        /// Feature.
        /// </summary>
        public IFeatureInfo Feature { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public FeatureDiscovered(EventTime time, IFeatureInfo feature) : base(time)
        {
            Feature = feature;
        }
    }
}