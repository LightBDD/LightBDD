using LightBDD.Core.Metadata;
using LightBDD.Core.Notification;
using LightBDD.Core.Notification.Events;
using LightBDD.Core.Results;
#pragma warning disable 618

namespace LightBDD.Framework.Notification
{
    /// <summary>
    /// Progress notifier implementation that does nothing when called.
    /// </summary>
    public class NoProgressNotifier : IProgressNotifier, IFeatureProgressNotifier, IScenarioProgressNotifier
    {
        private NoProgressNotifier() { }
        /// <summary>
        /// Returns default instance.
        /// </summary>
        public static NoProgressNotifier Default { get; } = new NoProgressNotifier();
        /// <summary>
        /// Does nothing.
        /// </summary>
        public void NotifyFeatureStart(IFeatureInfo feature) { }
        /// <summary>
        /// Does nothing.
        /// </summary>
        public void NotifyFeatureFinished(IFeatureResult feature) { }
        /// <summary>
        /// Does nothing.
        /// </summary>
        public void NotifyScenarioStart(IScenarioInfo scenario) { }
        /// <summary>
        /// Does nothing.
        /// </summary>
        public void NotifyScenarioFinished(IScenarioResult scenario) { }
        /// <summary>
        /// Does nothing.
        /// </summary>
        public void NotifyStepStart(IStepInfo step) { }
        /// <summary>
        /// Does nothing.
        /// </summary>
        public void NotifyStepFinished(IStepResult step) { }
        /// <summary>
        /// Does nothing.
        /// </summary>
        public void NotifyStepComment(IStepInfo step, string comment) { }
        /// <summary>
        /// Does nothing.
        /// </summary>
        public void Notify(ProgressEvent e) { }
    }
}