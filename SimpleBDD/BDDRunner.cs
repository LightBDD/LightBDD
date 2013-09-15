using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using SimpleBDD.Naming;
using SimpleBDD.Notification;
using SimpleBDD.Results;
using SimpleBDD.Results.Implementation;

namespace SimpleBDD
{
	/// <summary>
	/// Allows to execute behavior test scenarios.
	/// </summary>
	public class BDDRunner
	{
		private readonly Type _context;
		private readonly FeatureResult _result;

		/// <summary>
		/// Scenario execution progress notifier.
		/// </summary>
		public IProgressNotifier ProgressNotifier { get; set; }

		/// <summary>
		/// Returns feature execution result.
		/// </summary>
		public IFeatureResult Result
		{
			get { return _result; }
		}

		/// <summary>
		/// Initializes runner with ConsoleProgressNotifier.
		/// </summary>
		public BDDRunner(Type testClass)
		{
			_context = testClass;
			_result = new FeatureResult();
			ProgressNotifier = new ConsoleProgressNotifier();
		}

		/// <summary>
		/// Runs test scenario by executing given steps in order.
		/// If given step throws, other are not executed.
		/// Scenario name is determined on method name in which RunScenario() method was called.<br/>
		/// Please note that test project has to be compiled in DEBUG mode, or test method has to have [MethodImpl(MethodImplOptions.NoInlining)] attribute in order to properly determine scenario name.
		/// Step name is determined on corresponding action name.<br/>
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
			RunScenario(GetScenarioName(), steps);
		}

		/// <summary>
		/// Runs test scenario by executing given steps in order.
		/// If given step throws, other are not executed.
		/// Scenario name is specified in parameter list.
		/// Step name is determined on corresponding action name.<br/>
		/// Example usage:
		/// <code>
		/// [Test]
		/// public void Successful_login()
		/// {
		/// 	_bddRunner.RunScenario("My successful login",
		/// 		Given_user_is_about_to_login,
		/// 		Given_user_entered_valid_login,
		/// 		Given_user_entered_valid_password,
		/// 		When_user_clicked_login_button,
		/// 		Then_login_is_successful,
		/// 		Then_welcome_message_is_returned_containing_user_name);
		/// }
		/// </code>
		/// </summary>
		/// <param name="scenarioName">Scenario name.</param>
		/// <param name="steps">List of steps to execute in order.</param>
		public void RunScenario(string scenarioName, params Action[] steps)
		{
			ProgressNotifier.NotifyScenarioStart(scenarioName);

			var stepsToExecute = PrepareStepsToExecute(steps).ToArray();
			try
			{
				foreach (var step in stepsToExecute)
					PerformStep(step, stepsToExecute.Length);
			}
			finally
			{
				_result.AddScenario(new ScenarioResult(scenarioName, stepsToExecute.Select(s => s.Result)));
			}
		}

		private string GetScenarioName()
		{
			var callingMethodName = new StackTrace().GetFrame(2).GetMethod().Name;
			return NameFormatter.Format(callingMethodName);
		}

		private void PerformStep(Step step, int totalCount)
		{
			ProgressNotifier.NotifyStepStart(step.Result.Name, step.Result.Number, totalCount);
			step.Invoke();
		}

		private IEnumerable<Step> PrepareStepsToExecute(Action[] steps)
		{
			int i = 0;
			return steps.Select(step => new Step(step, ++i));
		}
	}
}
