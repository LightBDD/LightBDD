using System;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Core.Execution.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LightBDD.Integration.MsTest.UnitTests
{
    [TestClass]
    [ScenarioCategory("Category E")]
    public class Runner_tests : FeatureFixture
    {
        [TestMethod]
        [Label(nameof(It_should_capture_scenario_name))]
        public void It_should_capture_scenario_name()
        {
            Runner.Basic().RunScenario(Some_step);
            var result = GetScenarioResult(nameof(It_should_capture_scenario_name));
            Assert.AreEqual("It should capture scenario name", result.Info.Name.ToString());
        }

        [TestMethod]
        [TestCategory("Category A"), TestCategory("Category B"), ScenarioCategory("Category C")]
        [Label(nameof(It_should_capture_mstest_specific_attributes))]
        public void It_should_capture_mstest_specific_attributes()
        {
            Runner.Basic().RunScenario(Some_step);

            var scenario = GetScenarioResult(nameof(It_should_capture_mstest_specific_attributes));
            var expected = new[]
            {
                "Category A",
                "Category B",
                "Category C",
                "Category E"
            };
            CollectionAssert.AreEqual(expected, scenario.Info.Categories.ToArray());
        }

        [TestMethod]
        [Label(nameof(It_should_capture_mstest_inconclusive_assertion))]
        public void It_should_capture_mstest_inconclusive_assertion()
        {
            try
            {
                Runner.Basic().RunScenario(Inconclusive_step);
            }
            catch
            {
            }
            var result = GetScenarioResult(nameof(It_should_capture_mstest_inconclusive_assertion));
            Assert.AreEqual(ExecutionStatus.Ignored, result.Status);
        }

        [TestMethod]
        public async Task Runner_should_throw_meaningful_exception_if_scenario_is_not_run_from_test()
        {
            Exception ex = null;
            try
            {
                await Task.Run(() => Runner.Basic().RunScenario(Some_step));
            }
            catch (Exception e)
            {
                ex = e;
            }
            Assert.IsNotNull(ex);
            Assert.AreEqual(
                    "Unable to locate Scenario name. Please ensure that scenario is executed from method with [TestMethod] attribute and [assembly:Debuggable(true, true)] attribute is present in test assembly.",
                    ex.Message);
        }

        private void Inconclusive_step()
        {
            Assert.Inconclusive();
        }

        private void Some_step()
        {
        }

        private IScenarioResult GetScenarioResult(string scenarioId)
        {
            return FeatureFactory.GetRunnerFor(GetType())
                .GetFeatureResult()
                .GetScenarios()
                .Single(s => s.Info.Labels.Contains(scenarioId));
        }
    }
}
