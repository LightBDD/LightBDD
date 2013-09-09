namespace SimpleBDD.Notification
{
	/// <summary>
	/// Interface for progress notification.
	/// </summary>
	public interface IProgressNotifier
	{
		/// <summary>
		/// Notifies that scenario has been started.
		/// </summary>
		/// <param name="scenarioName">Scenario name.</param>
		void NotifyScenarioStart(string scenarioName);

		/// <summary>
		/// Notifies that step has been started.
		/// </summary>
		/// <param name="stepName">Step name.</param>
		/// <param name="stepNumber">Step number starting from 1.</param>
		/// <param name="totalStepCount">Total number of steps.</param>
		void NotifyStepStart(string stepName, int stepNumber, int totalStepCount);
	}
}