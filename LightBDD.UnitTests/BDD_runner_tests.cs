using System;
using System.Linq;
using System.Runtime.CompilerServices;
using LightBDD.Notification;
using LightBDD.Results;
using LightBDD.Results.Implementation;
using NUnit.Framework;
using Rhino.Mocks;

namespace LightBDD.UnitTests
{
	[TestFixture]
	[Description("Runner tests description")]
	[Label("Ticket-1")]
	public class BDD_runner_tests
	{
		private BDDRunner _subject;
		private IProgressNotifier _progressNotifier;

		void Step_one() { }
		void Step_two() { }
		void Step_throwing_exception() { throw new Exception(); }
		void Step_with_ignore_assertion() { Assert.Ignore(); }
		void Step_with_inconclusive_assertion() { Assert.Inconclusive(); }

		#region Setup/Teardown

		[SetUp]
		public void SetUp()
		{
			_progressNotifier = MockRepository.GenerateMock<IProgressNotifier>();
			_subject = new BDDRunner(GetType(), _progressNotifier);
		}

		#endregion

		[Test]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public void Should_collect_scenario_result()
		{
			_subject.RunScenario(Step_one, Step_two);
			var result = _subject.Result.Scenarios.Single();
			Assert.That(result.Name, Is.EqualTo("Should collect scenario result"));
			Assert.That(result.Status, Is.EqualTo(ResultStatus.Passed));
			Assert.That(result.Steps, Is.EqualTo(new[]
			{
				new StepResult(1, "Step one", ResultStatus.Passed),
				new StepResult(2, "Step two", ResultStatus.Passed)
			}));
		}

		[Test]
		public void Should_collect_scenario_result_for_explicitly_named_scenario()
		{
			const string scenarioName = "my scenario";
			_subject.RunScenario(scenarioName, Step_one, Step_two);
			var result = _subject.Result.Scenarios.Single();
			Assert.That(result.Name, Is.EqualTo(scenarioName));
			Assert.That(result.Status, Is.EqualTo(ResultStatus.Passed));
			Assert.That(result.Steps, Is.EqualTo(new[]
			{
				new StepResult(1, "Step one", ResultStatus.Passed),
				new StepResult(2, "Step two", ResultStatus.Passed)
			}));
		}

		[Test]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public void Should_collect_scenario_result_for_failing_scenario()
		{
			try
			{
				_subject.RunScenario(Step_one, Step_throwing_exception, Step_two);
			}
			catch
			{
			}

			var result = _subject.Result.Scenarios.Single();
			Assert.That(result.Name, Is.EqualTo("Should collect scenario result for failing scenario"));
			Assert.That(result.Status, Is.EqualTo(ResultStatus.Failed));
			Assert.That(result.Steps, Is.EqualTo(new[]
			{
				new StepResult(1, "Step one", ResultStatus.Passed),
				new StepResult(2, "Step throwing exception", ResultStatus.Failed),
				new StepResult(3, "Step two", ResultStatus.NotRun)
			}));
		}

		[Test]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public void Should_collect_scenario_result_for_ignored_scenario_steps()
		{
			try
			{
				_subject.RunScenario(Step_one, Step_with_ignore_assertion, Step_two);
			}
			catch
			{
			}

			var result = _subject.Result.Scenarios.Single();
			Assert.That(result.Name, Is.EqualTo("Should collect scenario result for ignored scenario steps"));
			Assert.That(result.Status, Is.EqualTo(ResultStatus.Ignored));
			Assert.That(result.Steps, Is.EqualTo(new[]
			{
				new StepResult(1, "Step one", ResultStatus.Passed),
				new StepResult(2, "Step with ignore assertion", ResultStatus.Ignored),
				new StepResult(3, "Step two", ResultStatus.NotRun)
			}));
		}

		[Test]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public void Should_collect_scenario_result_for_inconclusive_scenario_steps()
		{
			try
			{
				_subject.RunScenario(Step_one, Step_with_inconclusive_assertion, Step_two);
			}
			catch
			{
			}

			var result = _subject.Result.Scenarios.Single();
			Assert.That(result.Name, Is.EqualTo("Should collect scenario result for inconclusive scenario steps"));
			Assert.That(result.Status, Is.EqualTo(ResultStatus.Ignored));
			Assert.That(result.Steps, Is.EqualTo(new[]
			{
				new StepResult(1, "Step one", ResultStatus.Passed),
				new StepResult(2, "Step with inconclusive assertion", ResultStatus.Ignored),
				new StepResult(3, "Step two", ResultStatus.NotRun)
			}));
		}

		[Test]
		public void Should_display_feature_name()
		{
			_progressNotifier.AssertWasCalled(n => n.NotifyFeatureStart("BDD runner tests", "Runner tests description", "Ticket-1"));
		}

		[Test]
		public void Should_display_scenario_failure()
		{
			try { _subject.RunScenario(Step_throwing_exception); }
			catch { }
			_progressNotifier.AssertWasCalled(n => n.NotifyScenarioFinished(ResultStatus.Failed));
		}

		[Test]
		public void Should_display_scenario_ignored()
		{
			try { _subject.RunScenario(Step_with_ignore_assertion); }
			catch { }
			_progressNotifier.AssertWasCalled(n => n.NotifyScenarioFinished(ResultStatus.Ignored));
		}

		[Test]
		public void Should_display_scenario_inconclusive()
		{
			try { _subject.RunScenario(Step_with_inconclusive_assertion); }
			catch { }
			_progressNotifier.AssertWasCalled(n => n.NotifyScenarioFinished(ResultStatus.Ignored));
		}

		[Test]
		[Label("Ticket-2")]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public void Should_display_scenario_name()
		{
			_subject.RunScenario();
			_progressNotifier.AssertWasCalled(n => n.NotifyScenarioStart("Should display scenario name", "Ticket-2"));
		}

		[Test]
		public void Should_display_scenario_success()
		{
			_subject.RunScenario(Step_one);
			_progressNotifier.AssertWasCalled(n => n.NotifyScenarioFinished(ResultStatus.Passed));
		}

		[Test]
		public void Should_display_steps()
		{
			_subject.RunScenario(Step_one, Step_two);
			_progressNotifier.AssertWasCalled(n => n.NotifyStepStart("Step one", 1, 2));
			_progressNotifier.AssertWasCalled(n => n.NotifyStepStart("Step two", 2, 2));
		}

		[Test]
		[Label("Label 1")]
		public void Should_include_labels_in_result()
		{
			_subject.RunScenario(Step_one, Step_two);
			Assert.That(_subject.Result.Label, Is.EqualTo("Ticket-1"));
			Assert.That(_subject.Result.Scenarios.Single().Label, Is.EqualTo("Label 1"));
		}

		[Test]
		public void Should_pass_exception_to_runner_caller()
		{
			Assert.Throws<Exception>(() => _subject.RunScenario(Step_throwing_exception));
		}

		[Test]
		public void Should_pass_ignore_exception_to_runner_caller()
		{
			Assert.Throws<IgnoreException>(() => _subject.RunScenario(Step_with_ignore_assertion));
		}

		[Test]
		public void Should_pass_inconclusive_exception_to_runner_caller()
		{
			Assert.Throws<InconclusiveException>(() => _subject.RunScenario(Step_with_inconclusive_assertion));
		}

		[Test]
		public void Should_use_console_progress_notifier_by_default()
		{
			Assert.That(new BDDRunner(GetType()).ProgressNotifier, Is.InstanceOf<ConsoleProgressNotifier>());
		}
	}
}
