using System;
using System.Collections.Generic;
using System.Linq;
using LightBDD.Core.Metadata;
using LightBDD.Core.Notification;
using LightBDD.Core.Results;

namespace LightBDD.Framework.Notification
{
    /// <summary>
    /// Scenario progress notifier allowing to delegate notification to zero or more notifiers.
    /// </summary>
    public class DelegatingScenarioProgressNotifier : IScenarioProgressNotifier
    {
        /// <summary>
        /// Returns notifiers used for notifications.
        /// </summary>
        public IEnumerable<IScenarioProgressNotifier> Notifiers { get; }

        /// <summary>
        /// Constructor configuring notifier to delegate all the notifications to provided <paramref name="notifiers"/>.
        /// </summary>
        /// <param name="notifiers">Notifiers to delegate notifications to.</param>
        /// <exception cref="ArgumentNullException">Throws when <paramref name="notifiers"/> is null.</exception>
        public DelegatingScenarioProgressNotifier(params IScenarioProgressNotifier[] notifiers)
            : this(EnsureNotNull(notifiers))
        {
        }

        private DelegatingScenarioProgressNotifier(IEnumerable<IScenarioProgressNotifier> notifiers)
        {
            Notifiers = notifiers.ToArray();
        }


        /// <summary>
        /// Notifies that scenario has started.
        /// </summary>
        /// <param name="scenario">Scenario info.</param>
        public void NotifyScenarioStart(IScenarioInfo scenario)
        {
            foreach (var notifier in Notifiers)
                notifier.NotifyScenarioStart(scenario);
        }
        /// <summary>
        /// Notifies that scenario has finished.
        /// </summary>
        /// <param name="scenario">Scenario result.</param>
        public void NotifyScenarioFinished(IScenarioResult scenario)
        {
            foreach (var notifier in Notifiers)
                notifier.NotifyScenarioFinished(scenario);
        }
        /// <summary>
        /// Notifies that step has started.
        /// </summary>
        /// <param name="step">Step info.</param>
        public void NotifyStepStart(IStepInfo step)
        {
            foreach (var notifier in Notifiers)
                notifier.NotifyStepStart(step);
        }
        /// <summary>
        /// Notifies that step has finished.
        /// </summary>
        /// <param name="step">Step result.</param>
        public void NotifyStepFinished(IStepResult step)
        {
            foreach (var notifier in Notifiers)
                notifier.NotifyStepFinished(step);
        }
        /// <summary>
        /// Notifies that step has been commented.
        /// </summary>
        /// <param name="step">Step info.</param>
        /// <param name="comment">Comment.</param>
        public void NotifyStepComment(IStepInfo step, string comment)
        {
            foreach (var notifier in Notifiers)
                notifier.NotifyStepComment(step, comment);
        }

        /// <summary>
        /// Composes <see cref="IScenarioProgressNotifier"/> from provided notifiers where
        /// any notifiers of <see cref="NoProgressNotifier"/> will be excluded
        /// and any notifiers of <see cref="DelegatingScenarioProgressNotifier"/> will be flattened.
        /// </summary>
        /// <param name="notifiers">Notifiers to compose.</param>
        /// <returns>Composition of provided notifiers.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="notifiers"/> or any of it's items is null.</exception>
        public static IScenarioProgressNotifier Compose(IEnumerable<IScenarioProgressNotifier> notifiers)
        {
            var results = new List<IScenarioProgressNotifier>();

            FlattenNotifiers(results, EnsureNotNull(notifiers));

            if (!results.Any())
                return NoProgressNotifier.Default;
            return results.Count == 1
                ? results[0]
                : new DelegatingScenarioProgressNotifier(results);
        }

        private static void FlattenNotifiers(ICollection<IScenarioProgressNotifier> output, IEnumerable<IScenarioProgressNotifier> notifiers)
        {
            foreach (var notifier in notifiers)
            {
                if (notifier is NoProgressNotifier)
                    continue;
                if (notifier is DelegatingScenarioProgressNotifier delegating)
                    FlattenNotifiers(output, delegating.Notifiers);
                else
                    output.Add(notifier);
            }
        }

        private static IEnumerable<IScenarioProgressNotifier> EnsureNotNull(IEnumerable<IScenarioProgressNotifier> notifiers)
        {
            if (notifiers == null)
                throw new ArgumentNullException(nameof(notifiers));

            return notifiers.Select((notifier, i) => notifier ?? throw new ArgumentNullException($"{nameof(notifiers)}[{i}]"));
        }
    }
}