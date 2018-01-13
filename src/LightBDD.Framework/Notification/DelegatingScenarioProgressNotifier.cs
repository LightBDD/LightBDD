using System;
using System.Diagnostics;
using LightBDD.Core.Metadata;
using LightBDD.Core.Notification;
using LightBDD.Core.Results;

namespace LightBDD.Framework.Notification
{
    /// <summary>
    /// Scenario progress notifier allowing to delegate notification to zero or more notifiers.
    /// </summary>
    [DebuggerStepThrough]
    public class DelegatingScenarioProgressNotifier : IScenarioProgressNotifier
    {
        private readonly IScenarioProgressNotifier[] _notifiers;
        /// <summary>
        /// Constructor configuring notifier to delegate all the notifications to provided <paramref name="notifiers"/>.
        /// </summary>
        /// <param name="notifiers">Notifiers to delegate notifications to.</param>
        /// <exception cref="ArgumentNullException">Throws when <paramref name="notifiers"/> is null.</exception>
        public DelegatingScenarioProgressNotifier(params IScenarioProgressNotifier[] notifiers)
        {
            _notifiers = notifiers ?? throw new ArgumentNullException(nameof(notifiers));
        }
        /// <summary>
        /// Notifies that scenario has started.
        /// </summary>
        /// <param name="scenario">Scenario info.</param>
        public void NotifyScenarioStart(IScenarioInfo scenario)
        {
            foreach (var notifier in _notifiers)
                notifier.NotifyScenarioStart(scenario);
        }
        /// <summary>
        /// Notifies that scenario has finished.
        /// </summary>
        /// <param name="scenario">Scenario result.</param>
        public void NotifyScenarioFinished(IScenarioResult scenario)
        {
            foreach (var notifier in _notifiers)
                notifier.NotifyScenarioFinished(scenario);
        }
        /// <summary>
        /// Notifies that step has started.
        /// </summary>
        /// <param name="step">Step info.</param>
        public void NotifyStepStart(IStepInfo step)
        {
            foreach (var notifier in _notifiers)
                notifier.NotifyStepStart(step);
        }
        /// <summary>
        /// Notifies that step has finished.
        /// </summary>
        /// <param name="step">Step result.</param>
        public void NotifyStepFinished(IStepResult step)
        {
            foreach (var notifier in _notifiers)
                notifier.NotifyStepFinished(step);
        }
        /// <summary>
        /// Notifies that step has been commented.
        /// </summary>
        /// <param name="step">Step info.</param>
        /// <param name="comment">Comment.</param>
        public void NotifyStepComment(IStepInfo step, string comment)
        {
            foreach (var notifier in _notifiers)
                notifier.NotifyStepComment(step, comment);
        }
    }
}