using System;
using System.Diagnostics;
using SimpleBDD.Notification;

namespace SimpleBDD
{
	/// <summary>
	/// Allows to execute behavior test scenarios.
	/// </summary>
	public class BDDRunner
	{
		/// <summary>
		/// Initializes runner with ConsoleProgressNotifier.
		/// </summary>
		public BDDRunner()
		{
			ProgressNotifier = new ConsoleProgressNotifier();
		}

		/// <summary>
		/// Runs test scenario by executing given steps in order.
		/// If given step throws, other are not executed.
		/// Scenario name is determined on method name in which RunScenario() method was called.
		/// Step name is determined on action name.<br/>
		/// Example usage:
		/// <code>
		/// [Test]
		/// public void Successful_login()
		/// {
		/// 	_bddRunner.RunScenario(
		/// 		Given_user_is_about_to_login,
		/// 		Given_user_entered_valid_login,
		/// 		Given_user_entered_valid_password,
		/// 		When_user_clicked_login_button,
		/// 		Then_login_is_successful,
		/// 		Then_welcome_message_is_returned_containing_user_name);
		/// }
		/// </code>
		/// </summary>
		/// <param name="steps">List of steps to execute in order.</param>
		public ScenarioResult RunScenario(params Action[] steps)
		{
			var result = new ScenarioResult(GetScenarioName()) {Status = ResultStatus.Passed};
			ProgressNotifier.NotifyScenarioStart(result.ScenarioName);

			int i = 0;
			foreach (Action step in steps)
				result.AddStep(PerformStep(step, ++i, steps.Length));
			return result;
		}

		private string GetScenarioName()
		{
			var callingMethodName = new StackTrace().GetFrame(2).GetMethod().Name;
			return TextFormatter.Format(callingMethodName);
		}

		private StepResult PerformStep(Action step, int stepNumber, int totalStepCount)
		{
			var stepName = TextFormatter.Format(step.Method.Name);
			ProgressNotifier.NotifyStepStart(stepName, stepNumber, totalStepCount);
			step();
			return new StepResult(stepNumber, totalStepCount, stepName, ResultStatus.Passed);
		}

		/// <summary>
		/// Scenario execution progress notifier.
		/// </summary>
		public IProgressNotifier ProgressNotifier { get; set; }
	}
}
