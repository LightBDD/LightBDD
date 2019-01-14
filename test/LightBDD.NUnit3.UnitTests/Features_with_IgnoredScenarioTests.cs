using System;
using LightBDD.Framework;
using LightBDD.Framework.Scenarios;
using NUnit.Framework;

namespace LightBDD.NUnit3.UnitTests
{
    [IgnoreScenario("all ignored")]
    public class Features_with_IgnoredScenarioTests : FeatureFixture
    {
        [Scenario]
        [Label(nameof(Runner_should_ignore_scenario_in_this_class))]
        public void Runner_should_ignore_scenario_in_this_class()
        {
            var ex = Assert.Throws<IgnoreException>(() => Runner.RunScenario(_ => Some_step()));
            Assert.AreEqual("all ignored", ex.Message);
        }

        private void Some_step()
        {
            throw new NotImplementedException();
        }
    }
}