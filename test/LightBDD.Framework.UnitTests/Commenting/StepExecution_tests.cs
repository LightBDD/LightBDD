using System;
using System.Linq;
using LightBDD.Core.Configuration;
using LightBDD.Core.Extensibility;
using LightBDD.Framework.Commenting;
using LightBDD.Framework.Commenting.Configuration;
using LightBDD.Framework.ExecutionContext.Configuration;
using LightBDD.UnitTests.Helpers.TestableIntegration;
using NUnit.Framework;

namespace LightBDD.Commenting.UnitTests
{
    [TestFixture]
    public class StepExecution_tests
    {
        [Test]
        public void Comment_should_throw_exception_if_feature_is_not_enabled()
        {
            var runner = new TestableFeatureBddRunnerFactory(
                    TestableIntegrationContextBuilder.Default()
                    .WithExecutionExtensions(new ExecutionExtensionsConfiguration().EnableScenarioExecutionContext())
                )
                .GetRunnerFor(GetType())
                .GetRunner(this);

            var exception = Assert.Throws<InvalidOperationException>(() => runner.Test().TestScenario(TestStep.CreateAsync(Commented_step, "some comment")));

            Assert.That(exception.Message, Is.EqualTo("Current task is not executing any scenario steps or commenting feature is not enabled in LightBddConfiguration. Ensure that configuration.ExecutionExtensionsConfiguration().EnableStepCommenting() is called during LightBDD initialization and commenting feature is called from task running scenario step."));
        }

        [Test]
        [TestCase(null)]
        [TestCase("\t\n\r ")]
        public void Comment_should_ignore_empty_comments(string comment)
        {
            var feature = GetFeatureRunner();
            var runner = feature.GetRunner(this);

            runner.Test().TestScenario(TestStep.CreateAsync(Commented_step, comment));

            Assert.That(feature.GetFeatureResult().GetScenarios().Single().GetSteps().Single().Comments.ToArray(), Is.Empty);
        }

        [Test]
        public void Comment_should_record_comment_in_currently_executed_step()
        {
            var feature = GetFeatureRunner();
            var runner = feature.GetRunner(this);

            var comment = "abc";
            var otherComment = "def";

            runner.Test().TestScenario(
                TestStep.CreateAsync(Commented_step, comment),
                TestStep.CreateAsync(Commented_step, otherComment));

            var steps = feature.GetFeatureResult().GetScenarios().Single().GetSteps().ToArray();

            Assert.That(steps[0].Comments.ToArray(), Is.EqualTo(new[] { comment, comment }));
            Assert.That(steps[1].Comments.ToArray(), Is.EqualTo(new[] { otherComment, otherComment }));
        }

        private static void Commented_step(string comment)
        {
            StepExecution.Current.Comment(comment);
            StepExecution.Current.Comment(comment);
        }

        private IFeatureBddRunner GetFeatureRunner()
        {
            var context = TestableIntegrationContextBuilder.Default()
                .WithExecutionExtensions(new ExecutionExtensionsConfiguration().EnableStepCommenting());

            return new TestableFeatureBddRunnerFactory(context).GetRunnerFor(GetType());
        }
    }
}
