using LightBDD.Notification;
using LightBDD.Results;

namespace LightBDD.UnitTests.Helpers
{
    internal class NoNotifier : IProgressNotifier
    {
        public void NotifyFeatureStart(string featureName, string featureDescription, string label)
        {
        }

        public void NotifyScenarioFinished(IScenarioResult scenarioResult)
        {
        }

        public void NotifyScenarioStart(string scenarioName, string label)
        {
        }

        public void NotifyStepStart(string stepName, int stepNumber, int totalStepCount)
        {
        }

        public void NotifyStepFinished(IStepResult stepResult, int totalStepCount)
        {
        }
    }
}