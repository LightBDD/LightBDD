using LightBDD.Framework;
using LightBDD.Framework.Commenting;
using LightBDD.Framework.Scenarios.Extended;
using LightBDD.MsTest2;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Example.LightBDD.MsTest2.Features
{
    [Label("Story-8")]
    [FeatureDescription(
@"In order to perform calculations correctly
As a office assistant
I want to use calculator for my calculations

This example presents usage of MultiAssertAttribute.")]
    [TestClass]
    public partial class Calculator_feature
    {
        [Label("Ticket-13")]
        [Scenario]
        [MultiAssert]
        public void Adding_numbers()
        {
            Runner.RunScenario(
                _ => Given_a_calculator(),
                _ => Then_adding_X_to_Y_should_give_RESULT(2, 3, 5),
                _ => Then_adding_X_to_Y_should_give_RESULT(-3, 2, -1),
                _ => Then_adding_X_to_Y_should_give_RESULT(0, 1, 1),
                _ => Then_adding_X_to_Y_should_give_RESULT(-2, -1, -3));
        }

        [Label("Ticket-13")]
        [Scenario]
        [MultiAssert]
        public void Dividing_numbers()
        {
            Runner.RunScenario(
                _ => Given_a_calculator(),
                _ => Then_divinding_X_by_Y_should_give_RESULT(6, 2, 3),
                _ => Then_divinding_X_by_Y_should_give_RESULT(2, 0, 0),
                _ => Then_divinding_X_by_Y_should_give_RESULT(2, 3, 0),
                _ => Then_divinding_X_by_Y_should_give_RESULT(0, 5, 1));
        }

        [Label("Ticket-13")]
        [Scenario]
        [MultiAssert]
        public void Multiplying_numbers()
        {
            Runner.RunScenario(
                _ => Given_a_calculator(),
                _ => Then_multiplying_X_by_Y_should_give_RESULT(6, 2, 12),
                _ => Then_multiplying_X_by_Y_should_give_RESULT(-1, 2, -2),
                _ => Then_multiplying_X_by_Y_should_give_RESULT(2, 0, 0),
                _ => Then_multiplying_X_by_Y_should_give_RESULT(2, 3, 6),
                _ => Then_multiplying_X_by_Y_should_give_RESULT(2, -3, -6));
        }

        [Label("Ticket-13")]
        [Scenario]
        [MultiAssert]
        public void Composite_operations()
        {
            Runner.RunScenario(
                _ => Given_a_calculator(),
                _ => Then_it_should_add_numbers(),
                _ => Then_it_should_multiply_numbers(),
                _ => Then_it_should_divide_numbers());
        }

        [MultiAssert]
        private CompositeStep Then_it_should_divide_numbers()
        {
            return CompositeStep.DefineNew().AddSteps(
                _ => Then_divinding_X_by_Y_should_give_RESULT(6, 3, 2),
                _ => Then_multiplying_X_by_Y_should_give_RESULT(5, 2, 2))
            .Build();
        }

        private CompositeStep Then_it_should_multiply_numbers()
        {
            StepExecution.Current.Comment("This step does not have MultiAssertAttribute so will stop on first exception");
            return CompositeStep.DefineNew().AddSteps(
                    _ => Then_multiplying_X_by_Y_should_give_RESULT(2, 3, 6),
                    _ => Then_multiplying_X_by_Y_should_give_RESULT(2, -3, -6),
                    _ => Then_multiplying_X_by_Y_should_give_RESULT(1, 1, 1))
                .Build();
        }

        [MultiAssert]
        private CompositeStep Then_it_should_add_numbers()
        {
            StepExecution.Current.Comment("It is possible to add MultiAssertAttribute on composite step");
            return CompositeStep.DefineNew().AddSteps(
                    _ => Then_adding_X_to_Y_should_give_RESULT(2, 3, 5),
                    _ => Then_adding_X_to_Y_should_give_RESULT(2, -3, -1),
                    _ => Then_adding_X_to_Y_should_give_RESULT(0, 1, 0))
                .Build();
        }
    }
}