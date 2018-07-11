using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace LightBDD.Framework.UnitTests.Expectations.Helpers
{

    public abstract class ExpectationTests
    {
        private readonly IExpectationScenario[] _scenarios;

        public ExpectationTests()
        {
            _scenarios = GetScenarios().ToArray();
        }

        protected abstract IEnumerable<IExpectationScenario> GetScenarios();

        [Test]
        public void It_should_pass_expectation()
        {
            foreach (var scenario in _scenarios)
                scenario.AssertExpectationMatchingValues();
        }

        [Test]
        public void It_should_pass_negated_expectation()
        {
            foreach (var scenario in _scenarios)
                scenario.AssertNegatedExpectationMatchingValues();
        }

        [Test]
        public void It_should_fail_expectation()
        {
            foreach (var scenario in _scenarios)
                scenario.AssertExpectationNotMatchingValues();
        }

        [Test]
        public void It_should_fail_negated_expectation()
        {
            foreach (var scenario in _scenarios)
                scenario.AssertNegatedExpectationNotMatchingValues();
        }

        [Test]
        public void It_should_format_case()
        {
            foreach (var scenario in _scenarios)
                scenario.AssertFormat();
        }
    }
}