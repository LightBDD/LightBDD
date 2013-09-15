using System;
using LightBDD.Results;

namespace LightBDD.Notification
{
	/// <summary>
	/// Progress notifier using console for displaying progress.
	/// </summary>
	public class ConsoleProgressNotifier : IProgressNotifier
	{
		#region IProgressNotifier Members

		/// <summary>
		/// Notifies that scenario has been started.
		/// </summary>
		/// <param name="scenarioName">Scenario name.</param>
		public void NotifyScenarioStart(string scenarioName)
		{
			Console.WriteLine("SCENARIO: {0}", scenarioName);
		}

		/// <summary>
		/// Notifies that step has been started.
		/// </summary>
		/// <param name="stepName">Step name.</param>
		/// <param name="stepNumber">Step number starting from 1.</param>
		/// <param name="totalStepCount">Total number of steps.</param>
		public void NotifyStepStart(string stepName, int stepNumber, int totalStepCount)
		{
			Console.WriteLine("  STEP {0}/{1}: {2}", stepNumber, totalStepCount, stepName);
		}

		/// <summary>
		/// Notifies that feature has been started.
		/// </summary>
		/// <param name="featureName">Feature name.</param>
		/// <param name="featureDescription">Feature description.</param>
		public void NotifyFeatureStart(string featureName, string featureDescription)
		{
			Console.WriteLine("FEATURE {0}:", featureName);
			if (!string.IsNullOrWhiteSpace(featureDescription))
				Console.WriteLine("  {0}", featureDescription.Replace("\n", "\n  "));
		}

		/// <summary>
		/// Notifies that scenario has been finished with given status.
		/// </summary>
		/// <param name="status">Status.</param>
		public void NotifyScenarioFinished(ResultStatus status)
		{
			Console.WriteLine("  SCENARIO RESULT: {0}", status);
		}

		#endregion
	}
}