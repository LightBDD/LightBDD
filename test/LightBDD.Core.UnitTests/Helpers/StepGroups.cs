using LightBDD.Framework;
using LightBDD.Framework.Scenarios.Extended;
using LightBDD.UnitTests.Helpers.TestableIntegration;

namespace LightBDD.Core.UnitTests.Helpers
{
    //TODO: consider dropping Group definition dependency on runner
    public abstract class StepGroups : Steps
    {
        protected abstract IBddRunner Runner { get; }

        public StepGroup Composite_group()
        {
            return Runner.Test().CreateCompositeStepGroup(Passing_step_group_with_comment, Bypassed_step_group);
        }

        public StepGroup Passing_step_group()
        {
            return Runner.Test().CreateStepGroup(
                Given_step_one,
                When_step_two,
                Then_step_three);
        }

        public StepGroup Passing_step_group_with_comment()
        {
            return Runner.Test().CreateStepGroup(
                Given_step_one,
                When_step_two_with_comment,
                Then_step_three);
        }

        public StepGroup Failing_step_group()
        {
            return Runner.Test().CreateStepGroup(
                Given_step_one,
                When_step_two_throwing_exception,
                Then_step_three);
        }

        public StepGroup Ignored_step_group()
        {
            return Runner.Test().CreateStepGroup(
                Given_step_one,
                When_step_two_ignoring_scenario,
                Then_step_three);
        }

        public StepGroup Bypassed_step_group()
        {
            return Runner.Test().CreateStepGroup(
                Given_step_one,
                When_step_two_is_bypassed,
                Then_step_three);
        }
    }
}