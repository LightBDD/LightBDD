using LightBDD.Core.Execution.Results;
using LightBDD.Core.Metadata;

namespace LightBDD.Core.Notification
{
    public interface IFeatureProgressNotifier
    {
        void NotifyFeatureStart(IFeatureInfo feature);
        void NotifyFeatureFinished(IFeatureResult feature);
    }
}
