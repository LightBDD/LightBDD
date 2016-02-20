using LightBDD.Core.Execution.Results;
using LightBDD.Core.Metadata;

namespace LightBDD.Core.Notification
{
    public class NoProgressNotifier : IProgressNotifier
    {
        public void NotifyFeatureStart(IFeatureInfo feature) { }
        public void NotifyFeatureFinished(IFeatureResult feature) { }
        public void NotifyScenarioStart(IScenarioInfo scenario) { }
        public void NotifyScenarioFinished(IScenarioResult scenario) { }
        public void NotifyStepStart(IStepInfo step) { }
        public void NotifyStepFinished(IStepResult step) { }
        public void NotifyStepComment(string comment) { }
    }
}