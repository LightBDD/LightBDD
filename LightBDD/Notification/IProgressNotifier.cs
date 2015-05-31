using LightBDD.Results;

namespace LightBDD.Notification
{
    /// <summary>
    /// Interface for progress notification.
    /// </summary>
    public interface IProgressNotifier
    {
        /// <summary>
        /// Notifies that feature has been started.
        /// </summary>
        /// <param name="featureName">Feature name.</param>
        /// <param name="featureDescription">Feature description.</param>
        /// <param name="label">Feature label.</param>
        void NotifyFeatureStart(string featureName, string featureDescription, string label);

        /// <summary>
        /// Notifies that scenario has been finished with given status and optional details.
        /// </summary>
        /// <param name="scenarioResult">Scenario result</param>
        void NotifyScenarioFinished(IScenarioResult scenarioResult);

        /// <summary>
        /// Notifies that scenario has been started.
        /// </summary>
        /// <param name="scenarioName">Scenario name.</param>
        /// <param name="label">Scenario label.</param>
        void NotifyScenarioStart(string scenarioName, string label);

        /// <summary>
        /// Notifies that step has been started.
        /// </summary>
        /// <param name="stepName">Step name.</param>
        /// <param name="stepNumber">Step number starting from 1.</param>
        /// <param name="totalStepCount">Total number of steps.</param>
        void NotifyStepStart(string stepName, int stepNumber, int totalStepCount);

        /// <summary>
        /// Notifies that step execution has been finished.
        /// </summary>
        /// <param name="stepResult">Step result.</param>
        /// <param name="totalStepCount">Total step count</param>
        void NotifyStepFinished(IStepResult stepResult, int totalStepCount);
        /// <summary>
        /// Notifies that step has been commented.
        /// </summary>
        /// <param name="comment">Comment.</param>
        /// <param name="stepNumber">Step number starting from 1.</param>
        /// <param name="totalStepCount">Total number of steps.</param>
        void NotifyStepComment(int stepNumber, int totalStepCount, string comment);
    }
}