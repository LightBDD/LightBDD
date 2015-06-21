using System.Diagnostics;
using LightBDD.Results;

namespace LightBDD.Notification
{
    /// <summary>
    /// Progress notifier allowing to delegate nofitication to one or more notifiers.
    /// </summary>
    [DebuggerStepThrough]
    public class DelegatingProgressNotifier : IProgressNotifier
    {
        /// <summary>
        /// Notifiers that would be used for progress notification
        /// </summary>
        public IProgressNotifier[] Notifiers { get; private set; }

        /// <summary>
        /// Constructor accepting a list of notifiers where notifications would be delegated to.
        /// </summary>
        /// <param name="notifiers">Notifiers</param>
        public DelegatingProgressNotifier(params IProgressNotifier[] notifiers)
        {
            Notifiers = notifiers;
        }

        /// <summary>
        /// Notifies that feature has been started.
        /// </summary>
        /// <param name="featureName">Feature name.</param>
        /// <param name="featureDescription">Feature description.</param>
        /// <param name="label">Feature label.</param>
        public void NotifyFeatureStart(string featureName, string featureDescription, string label)
        {
            foreach (var notifier in Notifiers)
                notifier.NotifyFeatureStart(featureName, featureDescription, label);
        }

        /// <summary>
        /// Notifies that scenario has been finished with given status and optional details.
        /// </summary>
        /// <param name="scenarioResult">Scenario result</param>
        public void NotifyScenarioFinished(IScenarioResult scenarioResult)
        {
            foreach (var notifier in Notifiers)
                notifier.NotifyScenarioFinished(scenarioResult);
        }

        /// <summary>
        /// Notifies that scenario has been started.
        /// </summary>
        /// <param name="scenarioName">Scenario name.</param>
        /// <param name="label">Scenario label.</param>
        public void NotifyScenarioStart(string scenarioName, string label)
        {
            foreach (var notifier in Notifiers)
                notifier.NotifyScenarioStart(scenarioName, label);
        }

        /// <summary>
        /// Notifies that step has been started.
        /// </summary>
        /// <param name="stepName">Step name.</param>
        /// <param name="stepNumber">Step number starting from 1.</param>
        /// <param name="totalStepCount">Total number of steps.</param>
        public void NotifyStepStart(string stepName, int stepNumber, int totalStepCount)
        {
            foreach (var notifier in Notifiers)
                notifier.NotifyStepStart(stepName, stepNumber, totalStepCount);
        }

        /// <summary>
        /// Notifies that step execution has been finished.
        /// </summary>
        /// <param name="stepResult">Step result.</param>
        /// <param name="totalStepCount">Total step count</param>
        public void NotifyStepFinished(IStepResult stepResult, int totalStepCount)
        {
            foreach (var notifier in Notifiers)
                notifier.NotifyStepFinished(stepResult, totalStepCount);
        }

        /// <summary>
        /// Notifies that step has been commented.
        /// </summary>
        /// <param name="comment">Comment.</param>
        /// <param name="stepNumber">Step number starting from 1.</param>
        /// <param name="totalStepCount">Total number of steps.</param>
        public void NotifyStepComment(int stepNumber, int totalStepCount, string comment)
        {
            foreach (var notifier in Notifiers)
                notifier.NotifyStepComment(stepNumber, totalStepCount, comment);
        }
    }
}