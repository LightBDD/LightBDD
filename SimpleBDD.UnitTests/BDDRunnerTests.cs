using System;
using System.Linq;
using NUnit.Framework;
using Rhino.Mocks;
using SimpleBDD.Notification;
using SimpleBDD.Results;

namespace SimpleBDD.UnitTests
{
	[TestFixture]
	public class BDDRunnerTests
	{
		private BDDRunner _subject;

		[SetUp]
		public void SetUp()
		{
			_subject = new BDDRunner();
		}

		[Test]
		public void Should_use_console_progress_by_default()
		{
			Assert.That(_subject.ProgressNotifier, Is.InstanceOf<ConsoleProgressNotifier>());
		}

		[Test]
		public void Should_display_scenario_name()
		{
			var progressNotifier = MockRepository.GenerateMock<IProgressNotifier>();
			_subject.ProgressNotifier = progressNotifier;
			_subject.RunScenario();
			progressNotifier.AssertWasCalled(n => n.NotifyScenarioStart("Should display scenario name"));
		}

		[Test]
		public void Should_display_steps()
		{
			var progressNotifier = MockRepository.GenerateMock<IProgressNotifier>();
			_subject.ProgressNotifier = progressNotifier;
			_subject.RunScenario(Step_one, Step_two);
			progressNotifier.AssertWasCalled(n => n.NotifyStepStart("Step one", 1, 2));
			progressNotifier.AssertWasCalled(n => n.NotifyStepStart("Step two", 2, 2));
		}

		[Test]
		public void Should_collect_scenario_result()
		{
			_subject.RunScenario(Step_one, Step_two);
			var result = _subject.StoryResult.Scenarios.Single();
			Assert.That(result.ScenarioName, Is.EqualTo("Should collect scenario result"));
			Assert.That(result.Status, Is.EqualTo(ResultStatus.Passed));
			Assert.That(result.Steps, Is.EqualTo(new[]
			{
			    new StepResult(1, 2, "Step one", ResultStatus.Passed),
			    new StepResult(2, 2, "Step two", ResultStatus.Passed)
			}));
		}

		[Test]
		public void Should_collect_scenario_result_for_failing_scenario()
		{
			try
			{
				_subject.RunScenario(Step_one, Step_throwing_exception, Step_two);
			}
			catch (Exception)
			{
			}

			var result = _subject.StoryResult.Scenarios.Single();
			Assert.That(result.ScenarioName, Is.EqualTo("Should collect scenario result for failing scenario"));
			Assert.That(result.Status, Is.EqualTo(ResultStatus.Failed));
			Assert.That(result.Steps, Is.EqualTo(new[]
			{
			    new StepResult(1, 3, "Step one", ResultStatus.Passed),
			    new StepResult(2, 3, "Step throwing exception", ResultStatus.Failed),
			    new StepResult(3, 3, "Step two", ResultStatus.NotRun)
			}));
		}

		void Step_one() { }
		void Step_two() { }
		void Step_throwing_exception() { throw new Exception(); }
	}
}
