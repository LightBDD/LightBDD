using System;
using LightBDD.Framework;
using LightBDD.Framework.Scenarios;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LightBDD.MsTest2.UnitTests
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
            Assert.AreEqual("Assert.Inconclusive failed. all ignored", ex.Message);
        }

        private void Some_step()
        {
            throw new NotImplementedException();
        }
    }
}