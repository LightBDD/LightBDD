using LightBDD.Framework;
using LightBDD.Framework.Scenarios.Extended;
using LightBDD.MsTest2;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Example.LightBDD.MsTest2.Features
{
    /// <summary>
    /// This feature class presents usage of MultiAssertAttribute in conjunction with scenarios and composite steps.
    /// 
    /// MultiAssertAttribute is described on the wiki page here: https://github.com/LightBDD/LightBDD/wiki/Tests-Structure-and-Conventions
    /// while composite steps are described here: https://github.com/LightBDD/LightBDD/wiki/Composite-Steps-Definition
    /// </summary>
    [Label("Story-8")]
    [FeatureDescription(
@"In order to perform calculations correctly
As a office assistant
I want to use calculator for my calculations

This example presents usage of MultiAssertAttribute.")]
    [TestClass]
    public partial class Calculator_feature
    {
        /// <summary>
        /// In this scenario we are calling an add operation on the calculator which logic is flawed causing steps expecting negative result to fail.
        /// Without MultiAssertAttribute, this scenario will stop execution on the first failure, however as this attribute is present,
        /// it will collect the results of all steps and throw AggregateException if there is more than 1 failure.
        /// </summary>
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

        /// <summary>
        /// In this scenario, some of the expectations are invalid and with MultiAssertAttribute being present all of the problematic steps will be reported.
        /// </summary>
        [Label("Ticket-13")]
        [Scenario]
        [MultiAssert]
        public void Dividing_numbers()
        {
            Runner.RunScenario(
                _ => Given_a_calculator(),
                _ => Then_dividing_X_by_Y_should_give_RESULT(6, 2, 3),
                _ => Then_dividing_X_by_Y_should_give_RESULT(2, 0, 0),
                _ => Then_dividing_X_by_Y_should_give_RESULT(2, 3, 0),
                _ => Then_dividing_X_by_Y_should_give_RESULT(0, 5, 1));
        }

        /// <summary>
        /// This scenario shows how ignored scenarios are handled with MultiAssertAttribute.
        /// In similar way to failed steps, the execution will proceed after step returning ignored result,
        /// and all ignored steps will be included in the execution report, however dislike failure scenario,
        /// only the first ignore exception will be reported back to the test framework, allowing to properly
        /// handle it and make scenario ignored.
        /// </summary>
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

        /// <summary>
        /// This scenario shows that MultiAssertAttribute has an effect only to the level where it is applied.
        /// If one of it's sub-step is a composite, the MultiAssertAttribute applied on the parent step won't have effect on it,
        /// unless it is explicitly applied on that sub-step as well.
        /// </summary>
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

        /// <summary>
        /// Below, there are few examples of the composite steps.
        /// Please note that composite step has to have signature with return type of CompositeStep or Task{CompositeStep},
        /// even thought those return values are not used directly in code (they are used by the framework).
        /// </summary>
        [MultiAssert]
        private CompositeStep Then_it_should_divide_numbers()
        {
            return CompositeStep.DefineNew().AddSteps(
                _ => Then_dividing_X_by_Y_should_give_RESULT(6, 3, 2),
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