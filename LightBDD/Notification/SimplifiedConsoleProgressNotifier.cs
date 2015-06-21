using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using LightBDD.Results;

namespace LightBDD.Notification
{
    /// <summary>
    /// Progress notifier using console for displaying simplified progress, dedicated for multithreaded environment.
    /// </summary>
    [DebuggerStepThrough]
    public class SimplifiedConsoleProgressNotifier : IProgressNotifier
    {
        private static readonly SimplifiedConsoleProgressNotifier Instance = new SimplifiedConsoleProgressNotifier();
        private int _scenariosPending;
        private int _scenariosFailed;
        private int _scenariosFinished;

        /// <summary>
        /// Progress notifier instance
        /// </summary>
        public static SimplifiedConsoleProgressNotifier GetInstance() { return Instance; }

        private SimplifiedConsoleProgressNotifier()
        {
        }

        /// <summary>
        /// Notifies that scenario has been started.
        /// </summary>
        /// <param name="scenarioName">Scenario name.</param>
        /// <param name="label">Scenario label.</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void NotifyScenarioStart(string scenarioName, string label)
        {
            _scenariosPending++;
            WriteProgress("starting: {0}", scenarioName);
        }

        /// <summary>
        /// Notifies that scenario has been finished with given status and optional details.
        /// </summary>
        /// <param name="scenarioResult">Scenario result</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void NotifyScenarioFinished(IScenarioResult scenarioResult)
        {
            _scenariosFinished++;
            _scenariosPending--;
            if (scenarioResult.Status == ResultStatus.Failed)
                _scenariosFailed++;
            WriteProgress("{0}: {1}", scenarioResult.Status.ToString().ToUpperInvariant(), scenarioResult.Name);
        }

        private void WriteProgress(string format, params object[] args)
        {
            Console.WriteLine("Finished={0}, Failed={1}, Pending={2}: {3}", _scenariosFinished, _scenariosFailed, _scenariosPending, string.Format(format, args));
        }

        /// <summary>
        /// Notifies that feature has been started.
        /// </summary>
        /// <param name="featureName">Feature name.</param>
        /// <param name="featureDescription">Feature description.</param>
        /// <param name="label">Feature label.</param>
        public void NotifyFeatureStart(string featureName, string featureDescription, string label)
        {
        }

        /// <summary>
        /// Notifies that step has been started.
        /// </summary>
        /// <param name="stepName">Step name.</param>
        /// <param name="stepNumber">Step number starting from 1.</param>
        /// <param name="totalStepCount">Total number of steps.</param>
        public void NotifyStepStart(string stepName, int stepNumber, int totalStepCount)
        {
        }

        /// <summary>
        /// Notifies that step execution has been finished.
        /// </summary>
        /// <param name="stepResult">Step result.</param>
        /// <param name="totalStepCount">Total step count</param>
        public void NotifyStepFinished(IStepResult stepResult, int totalStepCount)
        {
        }

        /// <summary>
        /// Notifies that step has been commented.
        /// </summary>
        /// <param name="comment">Comment.</param>
        /// <param name="stepNumber">Step number starting from 1.</param>
        /// <param name="totalStepCount">Total number of steps.</param>
        public void NotifyStepComment(int stepNumber, int totalStepCount, string comment)
        {
        }

        /// <summary>
        /// Resets counters.
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Reset()
        {
            _scenariosPending = _scenariosFinished = _scenariosFailed = 0;
        }
    }
}