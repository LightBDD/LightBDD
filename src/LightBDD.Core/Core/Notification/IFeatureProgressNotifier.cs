using LightBDD.Core.Metadata;
using LightBDD.Core.Results;

namespace LightBDD.Core.Notification
{
    public interface IFeatureProgressNotifier
    {
        void NotifyFeatureStart(IFeatureInfo feature);
        void NotifyFeatureFinished(IFeatureResult feature);
    }
}
