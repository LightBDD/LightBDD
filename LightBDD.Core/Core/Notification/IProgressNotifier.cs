using LightBDD.Core.Execution.Results;
using LightBDD.Core.Metadata;

namespace LightBDD.Core.Notification
{
    public interface IProgressNotifier
    {
        void NotifyFeatureStart(IFeatureInfo feature);
        void NotifyFeatureFinished(IFeatureResult feature);

        void NotifyScenarioStart(IScenarioInfo scenario);
        void NotifyScenarioFinished(IScenarioResult scenario);

        void NotifyStepStart(IStepInfo step);
        void NotifyStepFinished(IStepResult step);

        void NotifyStepComment(IStepInfo step, string comment);
    }
}
