using System;
using LightBDD.Scenarios.Basic;
using NUnit.Framework;

namespace LightBDD.Integration.NUnit3.UnitTests
{
    public class FeatureFixtureAttributeTests
    {
        [Scenario]
        public void Runner_should_throw_meaningful_exception_if_scenario_is_not_run_from_class_with_FeatureFixture_attribute()
        {
            var runner = NUnit3FeatureRunnerFactory.GetRunnerFor(GetType()).GetRunner(this);
            Exception ex = Assert.Throws<InvalidOperationException>(() => runner.RunScenario(Some_step));
            Assert.That(
                ex.Message,
                Is.EqualTo("Unable to locate Scenario name. Please ensure that scenario is executed from method with [Scenario] attribute and test class deriving from FeatureFixture or with [FeatureFixture] attribute."));
        }

        private void Some_step()
        {
        }
    }
}