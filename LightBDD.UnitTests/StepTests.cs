using System;
using LightBDD.Results;
using NUnit.Framework;

namespace LightBDD.UnitTests
{
	[TestFixture]
	public class StepTests : SomeSteps
	{
		[Test]
		public void Should_assert_ignore_mark_step_ignored()
		{
			var step = new Step(Step_with_ignore_assertion, 1);
			var ex = Assert.Throws<IgnoreException>(step.Invoke);
			Assert.That(step.Result.Status, Is.EqualTo(ResultStatus.Ignored));
			Assert.That(step.Result.StatusDetails, Is.EqualTo(ex.Message));
		}

		[Test]
		public void Should_assert_inconclusive_mark_step_ignored()
		{
			var step = new Step(Step_with_inconclusive_assertion, 1);
			var ex = Assert.Throws<InconclusiveException>(step.Invoke);
			Assert.That(step.Result.Status, Is.EqualTo(ResultStatus.Ignored));
			Assert.That(step.Result.StatusDetails, Is.EqualTo(ex.Message));
		}

		[Test]
		public void Should_exception_mark_step_failed()
		{
			var step = new Step(Step_throwing_exception, 1);
			var ex = Assert.Throws<InvalidOperationException>(step.Invoke);
			Assert.That(step.Result.Status, Is.EqualTo(ResultStatus.Failed));
			Assert.That(step.Result.StatusDetails, Is.EqualTo(ex.Message));
		}

		[Test]
		public void Should_format_regular_method_name()
		{
			var step = new Step(Step_one, 1);
			Assert.That(step.Result.Name, Is.EqualTo("Step one"));
		}

		[Test]
		public void Should_mark_step_not_run()
		{
			var step = new Step(Step_one, 1);
			Assert.That(step.Result.Status, Is.EqualTo(ResultStatus.NotRun));
			Assert.That(step.Result.StatusDetails, Is.Null);
		}

		[Test]
		public void Should_passing_action_mark_step_passed()
		{
			var step = new Step(Step_one, 1);
			step.Invoke();
			Assert.That(step.Result.Status, Is.EqualTo(ResultStatus.Passed));
			Assert.That(step.Result.StatusDetails, Is.Null);
		}
	}
}