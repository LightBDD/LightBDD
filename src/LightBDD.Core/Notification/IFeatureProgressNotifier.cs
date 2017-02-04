using LightBDD.Core.Metadata;
using LightBDD.Core.Results;

namespace LightBDD.Core.Notification
{
    /// <summary>
    /// Feature progress notification interface.
    /// </summary>
    public interface IFeatureProgressNotifier
    {
        /// <summary>
        /// Notifies that feature has started.
        /// </summary>
        /// <param name="feature">Feature info.</param>
        void NotifyFeatureStart(IFeatureInfo feature);
        /// <summary>
        /// Notifies that feature has finished.
        /// </summary>
        /// <param name="feature">Feature result.</param>
        void NotifyFeatureFinished(IFeatureResult feature);
    }
}
