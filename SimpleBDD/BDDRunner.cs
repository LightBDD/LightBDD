using System;

namespace SimpleBDD
{
	/// <summary>
	/// Allows to execute behavior test scenarios.
	/// </summary>
	public class BDDRunner
	{
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
			int i = 0;
			foreach (Action step in steps)
				PerformStep(step, ++i, steps.Length);
		}

		private void PerformStep(Action step, int stepNumber, int totalStepCount)
		{
			Console.WriteLine("STEP {0}/{1}: {2}", stepNumber, totalStepCount, TextFormatter.Format(step.Method.Name));
			step();
		}
	}
}
