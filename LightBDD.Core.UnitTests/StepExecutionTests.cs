using System;
using System.Linq;
using LightBDD.Core.Execution;
using LightBDD.Core.Extensibility;
using LightBDD.Core.UnitTests.TestableIntegration;
using NUnit.Framework;

namespace LightBDD.Core.UnitTests
{
    [TestFixture]
    public class StepExecutionTests
    {
        [Test]
        public void Bypass_should_throw_StepBypassException()
        {
            var bypassReason = "reason";

            var exception = Assert.Throws<StepBypassException>(() => StepExecution.Current.Bypass(bypassReason));
            Assert.That(exception.Message, Is.EqualTo(bypassReason));
        }

        [Test]
        public void Comment_should_throw_exception_if_used_outside_of_step()
        {
            var exception = Assert.Throws<InvalidOperationException>(() => StepExecution.Current.Comment("some comment"));
            Assert.That(exception.Message, Is.EqualTo("Current task is not executing any scenarios. Please ensure that ScenarioContext is accessed from task running scenario."));
        }

        [Test]
        [TestCase(null)]
        [TestCase("\t\n\r ")]
        public void Comment_should_ignore_empty_comments(string comment)
        {
            var runner = TestableBddRunnerFactory.GetRunner(GetType());

            runner.Test().TestScenario(TestStep.CreateAsync(Commented_step, comment));

            Assert.That(runner.Integrate().GetFeatureResult().GetScenarios().Single().GetSteps().Single().Comments.ToArray(), Is.Empty);
        }

        [Test]
        public void Comment_should_record_comment_in_currently_executed_step()
        {
            var runner = TestableBddRunnerFactory.GetRunner(GetType());

            var comment = "abc";
            var otherComment = "def";

            runner.Test().TestScenario(
                TestStep.CreateAsync(Commented_step, comment),
                TestStep.CreateAsync(Commented_step, otherComment));

            var steps = runner.Integrate().GetFeatureResult().GetScenarios().Single().GetSteps().ToArray();

            Assert.That(steps[0].Comments.ToArray(), Is.EqualTo(new[] { comment, comment }));
            Assert.That(steps[1].Comments.ToArray(), Is.EqualTo(new[] { otherComment, otherComment }));
        }

        private static void Commented_step(string comment)
        {
            StepExecution.Current.Comment(comment);
            StepExecution.Current.Comment(comment);
        }
    }
}
