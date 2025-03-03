using System;
using System.Threading.Tasks;
using LightBDD.Framework.Extensibility;
using LightBDD.Framework.Scenarios;
using TUnit.Assertions.AssertConditions.Throws;

namespace LightBDD.TUnit.UnitTests;

public class FeatureFixtureAttributeTests
{
    [Scenario]
    public async Task Runner_should_throw_meaningful_exception_if_scenario_is_not_run_from_class_with_FeatureFixture_attribute()
    {
        var runner = FeatureRunnerProvider.GetRunnerFor(GetType()).GetBddRunner(this);
            
        await Assert.That(() => runner.RunScenario(Some_step))
            .Throws<InvalidOperationException>()
            .WithMessage("Unable to locate Scenario name. Please ensure that scenario is executed from method with [Scenario] attribute and test class deriving from FeatureFixture or with [FeatureFixture] attribute.");
    }

    private void Some_step()
    {
    }
}