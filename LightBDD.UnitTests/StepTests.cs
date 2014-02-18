using System;
using LightBDD.Results;
using NUnit.Framework;

namespace LightBDD.UnitTests
{
	[TestFixture]
	public class StepTests : SomeSteps
	{
		private ResultStatus Map(Type exception)
		{
			return exception == typeof(IgnoreException) || exception == typeof(InconclusiveException)
				? ResultStatus.Ignored
				: ResultStatus.Failed;
		}

		[Test]
		public void Should_assert_ignore_mark_step_ignored()
		{
			var step = new Step(Step_with_ignore_assertion, "step", 1, Map);
			var ex = Assert.Throws<IgnoreException>(step.Invoke);
			Assert.That(step.Result.Status, Is.EqualTo(ResultStatus.Ignored));
			Assert.That(step.Result.StatusDetails, Is.EqualTo(ex.Message));
		}

		[Test]
		public void Should_assert_inconclusive_mark_step_ignored()
		{
			var step = new Step(Step_with_inconclusive_assertion, "step", 1, Map);
			var ex = Assert.Throws<InconclusiveException>(step.Invoke);
			Assert.That(step.Result.Status, Is.EqualTo(ResultStatus.Ignored));
			Assert.That(step.Result.StatusDetails, Is.EqualTo(ex.Message));
		}

		[Test]
		public void Should_exception_mark_step_failed()
		{
			var step = new Step(Step_throwing_exception, "step", 1, Map);
			var ex = Assert.Throws<InvalidOperationException>(step.Invoke);
			Assert.That(step.Result.Status, Is.EqualTo(ResultStatus.Failed));
			Assert.That(step.Result.StatusDetails, Is.EqualTo(ex.Message));
		}

		[Test]
		public void Should_use_given_step_name()
		{
			var step = new Step(Step_one, "step", 1, Map);
			Assert.That(step.Result.Name, Is.EqualTo("step"));
		}

		[Test]
		public void Should_mark_step_not_run()
		{
			var step = new Step(Step_one, "step", 1, Map);
			Assert.That(step.Result.Status, Is.EqualTo(ResultStatus.NotRun));
			Assert.That(step.Result.StatusDetails, Is.Null);
		}

		[Test]
		public void Should_passing_action_mark_step_passed()
		{
			var step = new Step(Step_one, "step", 1, Map);
			step.Invoke();
			Assert.That(step.Result.Status, Is.EqualTo(ResultStatus.Passed));
			Assert.That(step.Result.StatusDetails, Is.Null);
		}
	}
}