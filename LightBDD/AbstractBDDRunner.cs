using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using LightBDD.Naming;
using LightBDD.Notification;
using LightBDD.Results;
using LightBDD.Results.Implementation;

namespace LightBDD
{
	/// <summary>
	/// Abstract class for executing behavior test scenarios.
	/// </summary>
	public abstract class AbstractBDDRunner
	{
		private readonly FeatureResult _result;

		/// <summary>
		/// Progress notifier.
		/// </summary>
		public IProgressNotifier ProgressNotifier { get; private set; }

		/// <summary>
		/// Returns feature execution result.
		/// </summary>
		public IFeatureResult Result
		{
			get { return _result; }
		}

		/// <summary>
		/// Initializes runner for given test class type with ConsoleProgressNotifier.
		/// Given testClass type Name is used as feature name.
		/// If test class is annotated with [FeatureDescription] attribute or implementation specific description attribute, it's content is used as feature description.
		/// </summary>
		/// <param name="testClass">Test class type.</param>
		protected AbstractBDDRunner(Type testClass)
			: this(testClass, new ConsoleProgressNotifier())
		{
		}

		/// <summary>
		/// Initializes runner for given test class type with given progress notifier.
		/// Given testClass type Name is used as feature name.
		/// If test class is annotated with [FeatureDescription] attribute or implementation specific description attribute, it's content is used as feature description.
		/// </summary>
		/// <param name="testClass">Test class type.</param>
		/// <param name="progressNotifier">Progress notifier.</param>
		protected AbstractBDDRunner(Type testClass, IProgressNotifier progressNotifier)
		{
			_result = new FeatureResult(NameFormatter.Format(testClass.Name), GetFeatureDescription(testClass), GetLabel(testClass));
			ProgressNotifier = progressNotifier;
			ProgressNotifier.NotifyFeatureStart(_result.Name, _result.Description, _result.Label);
		}

		private static string GetFeatureDescriptionFromAttribute(Type testClass)
		{
			return testClass.GetCustomAttributes(typeof(FeatureDescriptionAttribute), true)
			                .OfType<FeatureDescriptionAttribute>()
			                .Select(a => a.Description)
			                .SingleOrDefault();
		}

		/// <summary>
		/// Runs test scenario by executing given steps in order.
		/// If given step throws, other are not executed.
		/// Scenario name is determined on method name in which RunScenario() method was called.<br/>
		/// Scenario labels are determined on [Label] attributes applied on method in which RunScenario() method was called.<br/>
		/// Please note that test project has to be compiled in DEBUG mode (assembly has [Debuggable(true, true)] attribute), or test method has to have [MethodImpl(MethodImplOptions.NoInlining)] attribute in order to properly determine scenario name.
		/// Step name is determined on corresponding action name.<br/>
		/// Example usage:
		/// <code>
		/// [Test]
		/// [Label("Ticket-1")]
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
		[MethodImpl(MethodImplOptions.NoInlining)]
		public void RunScenario(params Action[] steps)
		{
			var method = GetScenarioMethod();
			RunScenario(GetScenarioName(method), GetLabel(method), steps);
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
			RunScenario(scenarioName, null, steps);
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
		/// 	_bddRunner.RunScenario("My successful login", "Ticket-1",
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
		/// <param name="label">Label associated to this scenario.</param>
		/// <param name="steps">List of steps to execute in order.</param>
		public void RunScenario(string scenarioName, string label, params Action[] steps)
		{
			ProgressNotifier.NotifyScenarioStart(scenarioName, label);

			var stepsToExecute = PrepareStepsToExecute(steps).ToArray();
			try
			{
				foreach (var step in stepsToExecute)
					PerformStep(step, stepsToExecute.Length);
			}
			finally
			{
				var result = new ScenarioResult(scenarioName, stepsToExecute.Select(s => s.Result), label);
				_result.AddScenario(result);
				ProgressNotifier.NotifyScenarioFinished(result.Status, result.StatusDetails);
			}
		}

		/// <summary>
		/// Returns implementation specific feature description or null if such is not provided.
		/// </summary>
		/// <param name="testClass">Class to analyze.</param>
		/// <returns>Feature description or null.</returns>
		protected abstract string GetImplementationSpecificFeatureDescription(Type testClass);

		/// <summary>
		/// Maps implementation specific exception to ResultStatus.
		/// </summary>
		protected abstract ResultStatus MapExceptionToStatus(Type exceptionType);

		private string GetFeatureDescription(Type testClass)
		{
			return GetFeatureDescriptionFromAttribute(testClass) ?? GetImplementationSpecificFeatureDescription(testClass);
		}

		private string GetLabel(MemberInfo member)
		{
			return member.GetCustomAttributes(typeof(LabelAttribute), true)
				.OfType<LabelAttribute>()
				.Select(a => a.Label)
				.SingleOrDefault();
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private MethodBase GetScenarioMethod()
		{
			return new StackTrace().GetFrame(2).GetMethod();
		}

		private string GetScenarioName(MethodBase method)
		{
			return NameFormatter.Format(method.Name);
		}

		private void PerformStep(Step step, int totalCount)
		{
			ProgressNotifier.NotifyStepStart(step.Result.Name, step.Result.Number, totalCount);
			step.Invoke();
		}

		private IEnumerable<Step> PrepareStepsToExecute(IEnumerable<Action> steps)
		{
			int i = 0;
			return steps.Select(step => new Step(step, ++i, MapExceptionToStatus));
		}
	}
}
