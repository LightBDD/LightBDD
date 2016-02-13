using System;
using System.Globalization;
using LightBDD.Core.Execution.Results;
using LightBDD.Core.UnitTests.TestableIntegration;
using NUnit.Framework;

namespace LightBDD.Core.UnitTests.Helpers
{
    [SetCulture("")]
    public class Steps
    {
        public const string BypassReason = "bypass reason";
        public const string ExceptionReason = "exception reason";
        public const string IgnoreReason = "ignore reason";

        public void Some_step() { }
        public void Given_step_one() { }
        public void When_step_two() { }
        public void When_step_two_is_bypassed() { StepExecution.Current.Bypass(BypassReason); }
        public void When_step_two_throwing_exception() { throw new InvalidOperationException(ExceptionReason); }
        public void Then_step_three() { }
        public void Then_step_three_should_throw_exception() { throw new InvalidOperationException(ExceptionReason); }
        public void When_step_two_ignoring_scenario() { StepExecution.Current.IgnoreScenario(IgnoreReason); }
        public void Then_step_four() { }
        public void Then_step_three_should_be_ignored() { StepExecution.Current.IgnoreScenario(IgnoreReason); }
    }
}