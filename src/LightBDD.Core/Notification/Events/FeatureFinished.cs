using LightBDD.Core.Execution;
using LightBDD.Core.Results;

namespace LightBDD.Core.Notification.Events
{
    /// <summary>
    /// Event raised when given feature execution is finished.
    /// </summary>
    public class FeatureFinished : ProgressEvent
    {
        /// <summary>
        /// Feature result.
        /// </summary>
        public IFeatureResult Result { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public FeatureFinished(EventTime time, IFeatureResult result) : base(time)
        {
            Result = result;
        }
    }
}