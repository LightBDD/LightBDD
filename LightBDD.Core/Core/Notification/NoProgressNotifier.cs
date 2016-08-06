using LightBDD.Core.Execution.Results;
using LightBDD.Core.Metadata;

namespace LightBDD.Core.Notification
{
    public class NoProgressNotifier : IFeatureProgressNotifier, IScenarioProgressNotifier
    {
        public static NoProgressNotifier Default { get; } = new NoProgressNotifier();

        public void NotifyFeatureStart(IFeatureInfo feature) { }
        public void NotifyFeatureFinished(IFeatureResult feature) { }
        public void NotifyScenarioStart(IScenarioInfo scenario) { }
        public void NotifyScenarioFinished(IScenarioResult scenario) { }
        public void NotifyStepStart(IStepInfo step) { }
        public void NotifyStepFinished(IStepResult step) { }
        public void NotifyStepComment(IStepInfo step, string comment) { }
    }
}