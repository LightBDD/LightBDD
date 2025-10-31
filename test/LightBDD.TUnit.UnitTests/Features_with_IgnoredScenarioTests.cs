using System;
using System.Threading.Tasks;
using LightBDD.Framework;
using LightBDD.Framework.Scenarios;
using TUnit.Core.Exceptions;

namespace LightBDD.TUnit.UnitTests;

[IgnoreScenario("all ignored")]
public class Features_with_IgnoredScenarioTests : FeatureFixture
{
    [Scenario]
    [Label(nameof(Runner_should_ignore_scenario_in_this_class))]
    public async Task Runner_should_ignore_scenario_in_this_class()
    {
        await Assert.That(() => Runner.RunScenario(_ => Some_step()))
            .Throws<SkipTestException>()
            .WithMessage("all ignored");
    }

    private void Some_step()
    {
        throw new NotImplementedException();
    }
}