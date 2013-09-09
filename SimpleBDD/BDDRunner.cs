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
		public void RunScenario(params Action[] steps)
		{
			PrintScenario();

			int i = 0;
			foreach (Action step in steps)
				PerformStep(step, ++i, steps.Length);
		}

		private void PrintScenario()
		{
			var callingMethodName = new StackTrace().GetFrame(2).GetMethod().Name;
			ProgressNotifier.NotifyScenarioStart(TextFormatter.Format(callingMethodName));
		}

		private void PerformStep(Action step, int stepNumber, int totalStepCount)
		{
			ProgressNotifier.NotifyStepStart(TextFormatter.Format(step.Method.Name), stepNumber, totalStepCount);
			step();
		}

		/// <summary>
		/// Scenario execution progress notifier.
		/// </summary>
		public IProgressNotifier ProgressNotifier { get; set; }
	}
}
