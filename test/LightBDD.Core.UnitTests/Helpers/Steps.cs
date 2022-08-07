using System;
using LightBDD.Framework;
using LightBDD.UnitTests.Helpers.TestableIntegration;

namespace LightBDD.Core.UnitTests.Helpers
{
    public class Steps
    {
        public const string BypassReason = "bypass reason";
        public const string ExceptionReason = "exception reason";
        public const string IgnoreReason = "ignore reason";
        public const string ParameterExceptionReason = "parameter exception";
        public const string CommentReason = "some comment";
        public const string AttachmentName = "attachment1";
        public const string AttachmentFile = "attachment1.txt";

        public void Some_step() { }
        public void Given_step_one() { }
        public void Setup_before_steps() { }
        public void When_step_two() { }

        public void When_step_two_with_comment() { StepCommentHelper.Comment(CommentReason); }
        public void When_step_two_with_attachment() { StepCommentHelper.AttachFile(AttachmentName, AttachmentFile); }
        public void When_step_two_is_bypassed() { StepExecution.Current.Bypass(BypassReason); }
        public void When_step_two_throwing_exception() { throw new InvalidOperationException(ExceptionReason); }
        public void Then_step_three() { }
        public void Then_step_three_should_throw_exception() { throw new InvalidOperationException(ExceptionReason); }
        public void When_step_two_ignoring_scenario() { StepExecution.Current.IgnoreScenario(IgnoreReason); }
        public void Then_step_four() { }
        public void Then_step_five() { }
        public void Then_step_three_should_be_ignored() { StepExecution.Current.IgnoreScenario(IgnoreReason); }
        public void Given_step_with_parameter(string parameter) { }
        public void When_step_with_parameter(int parameter) { }

        public void When_step_with_parameter_and_comments(int parameter)
        {
            StepCommentHelper.Comment(CommentReason);
            StepCommentHelper.Comment($"{parameter}");
        }
        public void When_step_with_parameter_throwing_exception(int parameter) { throw new InvalidOperationException(ExceptionReason); }
        public void Then_step_with_parameter(double parameter) { }


        public int ThrowingParameterInvocation() { throw new InvalidOperationException(ParameterExceptionReason); }

        public TestCompositeStep Composite_group()
        {
            return TestCompositeStep.CreateFromComposites(Passing_step_group_with_comment, Bypassed_step_group);
        }

        public TestCompositeStep Passing_step_group()
        {
            return TestCompositeStep.Create(
                Given_step_one,
                When_step_two,
                Then_step_three);
        }

        public TestCompositeStep Passing_step_group_with_comment()
        {
            return TestCompositeStep.Create(
                Given_step_one,
                When_step_two_with_comment,
                Then_step_three);
        }

        public TestCompositeStep Failing_step_group()
        {
            return TestCompositeStep.Create(
                Given_step_one,
                When_step_two_throwing_exception,
                Then_step_three);
        }

        public TestCompositeStep Ignored_step_group()
        {
            return TestCompositeStep.Create(
                Given_step_one,
                When_step_two_ignoring_scenario,
                Then_step_three);
        }

        public TestCompositeStep Bypassed_step_group()
        {
            return TestCompositeStep.Create(
                Given_step_one,
                When_step_two_is_bypassed,
                Then_step_three);
        }
    }
}