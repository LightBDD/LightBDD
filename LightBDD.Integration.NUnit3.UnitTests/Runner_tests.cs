using System;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Core.Execution.Results;
using LightBDD.Core.Extensibility;
using NUnit.Framework;

namespace LightBDD.Integration.NUnit3.UnitTests
{
    [TestFixture]
    [Description("desc")]
    [Category("Category D"), ScenarioCategory("Category E")]
    public class Runner_tests : FeatureFixture
    {
        [Test]
        [Label(nameof(It_should_capture_scenario_name))]
        public void It_should_capture_scenario_name()
        {
            Runner.Basic().RunScenario(Some_step);
            var result = GetScenarioResult(nameof(It_should_capture_scenario_name));
            Assert.That(result.Info.Name.ToString(), Is.EqualTo("It should capture scenario name"));
        }

        [Test]
        [Category("Category A"), Category("Category B"), ScenarioCategory("Category C")]
        [Label(nameof(It_should_capture_nunit_specific_attributes))]
        public void It_should_capture_nunit_specific_attributes()
        {
            Runner.Basic().RunScenario(Some_step);

            var result = Runner.Integrate().GetFeatureResult();
            Assert.That(result.Info.Description, Is.EqualTo("desc"));

            var scenario = GetScenarioResult(nameof(It_should_capture_nunit_specific_attributes));
            Assert.That(scenario.Info.Categories.ToArray(), Is.EquivalentTo(new[]
            {
                "Category A",
                "Category B",
                "Category C",
                "Category D",
                "Category E"
            }));
        }

        [Test]
        [Label(nameof(It_should_capture_nunit_ignore_assertion))]
        public void It_should_capture_nunit_ignore_assertion()
        {
            try
            {
                Runner.Basic().RunScenario(Ignored_step);
            }
            catch
            {
            }
            var result = GetScenarioResult(nameof(It_should_capture_nunit_ignore_assertion));
            Assert.That(result.Status, Is.EqualTo(ExecutionStatus.Ignored));
        }

        [Test]
        [Label(nameof(It_should_capture_nunit_inconclusive_assertion))]
        public void It_should_capture_nunit_inconclusive_assertion()
        {
            try
            {
                Runner.Basic().RunScenario(Inconclusive_step);
            }
            catch
            {
            }
            var result = GetScenarioResult(nameof(It_should_capture_nunit_inconclusive_assertion));
            Assert.That(result.Status, Is.EqualTo(ExecutionStatus.Ignored));
        }

        [Test]
        public void Runner_should_throw_meaningful_exception_if_scenario_is_not_run_from_test()
        {
            var ex = Assert.ThrowsAsync<InvalidOperationException>(() => Task.Run(() => Runner.Basic().RunScenario(Some_step)));
            Assert.That(ex.Message, Is.EqualTo("Unable to locate Scenario name. Please ensure that scenario is executed from method with [Test] attribute and [assembly:Debuggable(true, true)] attribute is present in test assembly."));
        }

        private void Inconclusive_step()
        {
            Assert.Inconclusive();
        }

        private void Ignored_step()
        {
            Assert.Ignore();
        }

        private void Some_step()
        {
        }

        private IScenarioResult GetScenarioResult(string scenarioId)
        {
            return Runner.Integrate()
                .GetFeatureResult()
                .GetScenarios()
                .Single(s => s.Info.Labels.Contains(scenarioId));
        }
    }
}
