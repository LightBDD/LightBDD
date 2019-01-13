using LightBDD.Framework;
using LightBDD.Framework.Scenarios;
using Shouldly;
using System;

namespace LightBDD.Fixie2.UnitTests
{
    [IgnoreScenario("all ignored")]
    public class Features_with_IgnoredScenarioTests : FeatureFixture
    {
        [Scenario]
        [Label(nameof(Runner_should_ignore_scenario_in_this_class))]
        public void Runner_should_ignore_scenario_in_this_class()
        {
            Should.Throw<IgnoreException>(() => Runner.RunScenario(_ => Some_step()))
                .Message.ShouldBe("all ignored");
        }

        private void Some_step()
        {
            throw new NotImplementedException();
        }
    }
}