using System;
using System.Text.RegularExpressions;
using LightBDD.Framework;
using LightBDD.Framework.Scenarios;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LightBDD.MsTest3.UnitTests
{
    [TestClass]
    [IgnoreScenario("all ignored")]
    public class Features_with_IgnoredScenarioTests : FeatureFixture
    {
        [Scenario]
        [Label(nameof(Runner_should_ignore_scenario_in_this_class))]
        public void Runner_should_ignore_scenario_in_this_class()
        {
            var ex = Assert.ThrowsException<AssertInconclusiveException>(() => Runner.RunScenario(_ => Some_step()));
            StringAssert.Matches(ex.Message, new Regex("Assert.Inconclusive .*. all ignored"));
        }

        private void Some_step()
        {
            throw new NotImplementedException();
        }
    }
}