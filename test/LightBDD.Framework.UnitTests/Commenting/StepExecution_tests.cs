using System;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Core.Execution;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Extensibility.Execution;
using LightBDD.Framework.Extensibility;
using LightBDD.Framework.UnitTests.Helpers;
using LightBDD.ScenarioHelpers;
using NUnit.Framework;

namespace LightBDD.Framework.UnitTests.Commenting
{
    [TestFixture]
    public class StepExecution_tests
    {
        [Test]
        [TestCase(null)]
        [TestCase("\t\n\r ")]
        public async Task Comment_should_ignore_empty_comments(string comment)
        {
            var scenario = await TestableBddRunner.Default.RunScenario(r => r.Test().TestScenario(TestStep.CreateAsync(Commented_step, comment)));

            Assert.That(scenario.GetSteps().Single().Comments.ToArray(), Is.Empty);
        }

        [Test]
        public async Task Comment_should_record_comment_in_currently_executed_step()
        {
            var comment = "abc";
            var otherComment = "def";

            var scenario = await TestableBddRunner.Default.RunScenario(r => r.Test().TestScenario(
                TestStep.CreateAsync(Commented_step, comment),
                TestStep.CreateAsync(Commented_step, otherComment)));

            var steps = scenario.GetSteps().ToArray();

            Assert.That(steps[0].Comments.ToArray(), Is.EqualTo(new[] { comment, comment }));
            Assert.That(steps[1].Comments.ToArray(), Is.EqualTo(new[] { otherComment, otherComment }));
        }

        [Test]
        public async Task Comment_should_record_comment_in_currently_executed_step_belonging_to_step_group()
        {
            var scenario = await TestableBddRunner.Default.RunScenario(r => r.Test().TestGroupScenario(Grouped_steps));

            var steps = scenario.GetSteps().Single().GetSubSteps().ToArray();

            Assert.That(steps[0].Comments.ToArray(), Is.EqualTo(new[] { nameof(Commented_step1) }));
            Assert.That(steps[1].Comments.ToArray(), Is.EqualTo(new[] { nameof(Commented_step2) }));
        }

        [Test]
        public async Task Comment_executed_from_step_decorator_should_be_properly_applied()
        {
            var scenario = await TestableBddRunner.Default.RunScenario(r => r.Test().TestGroupScenario(Decorated_grouped_steps));
            var mainStep = scenario.GetSteps().Single();
            Assert.That(mainStep.Comments.ToArray(), Is.EqualTo(new[] { "Start: Decorated grouped steps", "End: Decorated grouped steps" }));
            Assert.That(mainStep.GetSubSteps().Single().Comments.ToArray(), Is.EqualTo(new[] { "Start: Decorated step", "End: Decorated step" }));
        }

        private TestCompositeStep Grouped_steps()
        {
            return TestCompositeStep.Create(
                Commented_step1,
                Commented_step2);
        }

        [CommentingDecorator]
        private TestCompositeStep Decorated_grouped_steps()
        {
            return TestCompositeStep.Create(Decorated_step);
        }

        [CommentingDecorator]
        private static void Decorated_step() { }
        private static void Commented_step1() { StepExecution.Current.Comment(nameof(Commented_step1)); }
        private static void Commented_step2() { StepExecution.Current.Comment(nameof(Commented_step2)); }

        private static void Commented_step(string comment)
        {
            StepExecution.Current.Comment(comment);
            StepExecution.Current.Comment(comment);
        }

        private class CommentingDecoratorAttribute : Attribute, IStepDecoratorAttribute
        {
            public async Task ExecuteAsync(IStep step, Func<Task> stepInvocation)
            {
                StepExecution.Current.Comment("Start: " + step.Info.Name.ToString());
                try
                {
                    await stepInvocation();
                }
                finally
                {
                    StepExecution.Current.Comment("End: " + step.Info.Name.ToString());
                }
            }

            public int Order { get; }
        }
    }
}
