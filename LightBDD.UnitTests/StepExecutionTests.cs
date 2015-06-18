using System;
using System.Linq;
using LightBDD.Execution;
using LightBDD.Execution.Exceptions;
using LightBDD.Notification;
using LightBDD.UnitTests.Helpers;
using NUnit.Framework;
using Rhino.Mocks;

namespace LightBDD.UnitTests
{
    [TestFixture]
    public class StepExecutionTests
    {
        [Test]
        public void Bypass_should_throw_StepBypassException()
        {
            var bypassReason = "reason";

            var exception = Assert.Throws<StepBypassException>(() => StepExecution.Bypass(bypassReason));
            Assert.That(exception.Message, Is.EqualTo(bypassReason));
        }

        [Test]
        public void Comment_should_throw_exception_if_used_outside_of_step()
        {
            var exception = Assert.Throws<InvalidOperationException>(() => StepExecution.Comment("some comment"));
            Assert.That(exception.Message, Is.EqualTo("Current thread is not executing any scenarios. Please ensure that ExecutionContext is accessed from a thread running scenario."));
        }

        [Test]
        [TestCase(null)]
        [TestCase("\t\n\r ")]
        public void Comment_should_ignore_empty_comments(string comment)
        {
            var runner = new TestableBDDRunner(GetType(), MockRepository.GenerateMock<IProgressNotifier>());
            runner.RunScenario(_ => Commented_step(comment));
            Assert.That(runner.Result.Scenarios.Single().Steps.Single().Comments.ToArray(), Is.Empty);
        }

        [Test]
        public void Comment_should_record_comment_in_currently_executed_step()
        {
            var runner = new TestableBDDRunner(GetType(), MockRepository.GenerateMock<IProgressNotifier>());
            var comment = "abc";
            runner.RunScenario(_ => Commented_step(comment));
            Assert.That(runner.Result.Scenarios.Single().Steps.Single().Comments.ToArray(), Is.EqualTo(new[] { comment }));
        }

        private static void Commented_step(string comment)
        {
            StepExecution.Comment(comment);
        }
    }
}
