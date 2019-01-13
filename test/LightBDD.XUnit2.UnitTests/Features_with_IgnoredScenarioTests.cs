using System;
using LightBDD.Framework;
using LightBDD.Framework.Scenarios;
using Xunit;

namespace LightBDD.XUnit2.UnitTests
{
    [IgnoreScenario("all ignored")]
    public class Features_with_IgnoredScenarioTests : FeatureFixture
    {
        [Scenario]
        [Label(nameof(Runner_should_ignore_scenario_in_this_class))]
        public void Runner_should_ignore_scenario_in_this_class()
        {
            var ex = Assert.ThrowsAny<Exception>(() => Runner.RunScenario(_ => Some_step()));
            Assert.Equal("all ignored", ex.Message);
        }

        private void Some_step()
        {
            throw new NotImplementedException();
        }
    }
}