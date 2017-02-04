using LightBDD.Core.Metadata;
using LightBDD.Core.Results;

namespace LightBDD.Core.Notification
{
    /// <summary>
    /// Scenario progress notification interface.
    /// </summary>
    public interface IScenarioProgressNotifier
    {
        /// <summary>
        /// Notifies that scenario has started.
        /// </summary>
        /// <param name="scenario">Scenario info.</param>
        void NotifyScenarioStart(IScenarioInfo scenario);
        /// <summary>
        /// Notifies that scenario has finished.
        /// </summary>
        /// <param name="scenario">Scenario result.</param>
        void NotifyScenarioFinished(IScenarioResult scenario);

        /// <summary>
        /// Notifies that step has started.
        /// </summary>
        /// <param name="step">Step info.</param>
        void NotifyStepStart(IStepInfo step);
        /// <summary>
        /// Notifies that step has finished.
        /// </summary>
        /// <param name="step">Step result.</param>
        void NotifyStepFinished(IStepResult step);

        /// <summary>
        /// Notifies that step has been commented.
        /// </summary>
        /// <param name="step">Step info.</param>
        /// <param name="comment">Comment.</param>
        void NotifyStepComment(IStepInfo step, string comment);
    }
}