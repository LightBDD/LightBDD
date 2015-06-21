using System.Diagnostics;
using LightBDD.Formatting.Helpers;
using LightBDD.Results;

namespace LightBDD.Notification
{
    /// <summary>
    /// An abstract progress notifier, using WriteLine for writting preformatted notifications.
    /// </summary>
    [DebuggerStepThrough]
    public abstract class TextProgressNotifier : IProgressNotifier
    {
        #region IProgressNotifier Members

        /// <summary>
        /// Notifies that scenario has been started.
        /// </summary>
        /// <param name="scenarioName">Scenario name.</param>
        /// <param name="label">Scenario label.</param>
        public void NotifyScenarioStart(string scenarioName, string label)
        {
            WriteLine("SCENARIO: {0}{1}", FormatLabelText(label), scenarioName);
        }

        /// <summary>
        /// Notifies that step has been started.
        /// </summary>
        /// <param name="stepName">Step name.</param>
        /// <param name="stepNumber">Step number starting from 1.</param>
        /// <param name="totalStepCount">Total number of steps.</param>
        public void NotifyStepStart(string stepName, int stepNumber, int totalStepCount)
        {
            WriteLine("  STEP {0}/{1}: {2}...", stepNumber, totalStepCount, stepName);
        }

        /// <summary>
        /// Notifies that step execution has been finished.
        /// </summary>
        /// <param name="stepResult">Step result.</param>
        /// <param name="totalStepCount">Total step count</param>
        public void NotifyStepFinished(IStepResult stepResult, int totalStepCount)
        {
            WriteLine("  STEP {0}/{1}: {2} ({3} after {4})", stepResult.Number, totalStepCount, stepResult.Name, stepResult.Status, stepResult.ExecutionTime.FormatPretty());
        }

        /// <summary>
        /// Notifies that step has been commented.
        /// </summary>
        /// <param name="comment">Comment.</param>
        /// <param name="stepNumber">Step number starting from 1.</param>
        /// <param name="totalStepCount">Total number of steps.</param>
        public void NotifyStepComment(int stepNumber, int totalStepCount, string comment)
        {
            WriteLine("  STEP {0}/{1}: // {2} //", stepNumber, totalStepCount, comment);
        }

        /// <summary>
        /// Notifies that feature has been started.
        /// </summary>
        /// <param name="featureName">Feature name.</param>
        /// <param name="featureDescription">Feature description.</param>
        /// <param name="label">Feature label.</param>
        public void NotifyFeatureStart(string featureName, string featureDescription, string label)
        {
            WriteLine("FEATURE: {0}{1}", FormatLabelText(label), featureName);
            if (!string.IsNullOrWhiteSpace(featureDescription))
                WriteLine("  {0}", featureDescription.Replace("\n", "\n  "));
        }

        /// <summary>
        /// Notifies that scenario has been finished with given status and optional details.
        /// </summary>
        public void NotifyScenarioFinished(IScenarioResult scenarioResult)
        {
            if (scenarioResult.ExecutionTime.HasValue)
                WriteLine("  SCENARIO RESULT: {0} after {1}", scenarioResult.Status, scenarioResult.ExecutionTime.FormatPretty());
            else
                WriteLine("  SCENARIO RESULT: {0}", scenarioResult.Status);

            if (!string.IsNullOrWhiteSpace(scenarioResult.StatusDetails))
                WriteLine("    {0}", scenarioResult.StatusDetails.Replace("\n", "\n    "));
        }

        /// <summary>
        /// Writes a line of text
        /// </summary>
        /// <param name="format">Format.</param>
        /// <param name="args">Args.</param>
        protected abstract void WriteLine(string format, params object[] args);

        #endregion

        private static string FormatLabelText(string label)
        {
            return string.IsNullOrWhiteSpace(label)
                ? string.Empty
                : string.Format("[{0}] ", label);
        }
    }
}