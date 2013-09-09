using System;

namespace SimpleBDD.Notification
{
	/// <summary>
	/// Progress notifier using console for displying progress.
	/// </summary>
	public class ConsoleProgressNotifier : IProgressNotifier
	{
		/// <summary>
		/// Notifies that scenario has been started.
		/// </summary>
		/// <param name="scenarioName">Scenario name.</param>
		public void NotifyScenarioStart(string scenarioName)
		{
			Console.WriteLine(string.Format("SCENARIO: {0}", scenarioName));
		}

		/// <summary>
		/// Notifies that step has been started.
		/// </summary>
		/// <param name="stepName">Step name.</param>
		/// <param name="stepNumber">Step number starting from 1.</param>
		/// <param name="totalStepCount">Total number of steps.</param>
		public void NotifyStepStart(string stepName, int stepNumber, int totalStepCount)
		{
			Console.WriteLine("STEP {0}/{1}: {2}", stepNumber, totalStepCount, stepName);
		}
	}
}