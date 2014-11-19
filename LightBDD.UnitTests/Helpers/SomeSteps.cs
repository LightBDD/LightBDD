using System;
using LightBDD.Assertion;
using NUnit.Framework;

namespace LightBDD.UnitTests.Helpers
{
    public class SomeSteps
    {
        protected const string ExceptionText = "exception text";
        protected const string BypassReason = "bypass text";
        protected const string BypassReason2 = "bypass text2";
        protected const string IgnoreReason = "some reason";

        public void Step_one() { }
        public void Step_throwing_exception() { throw new InvalidOperationException(ExceptionText); }
        public void Step_throwing_exception_MESSAGE(string message) { throw new InvalidOperationException(message); }
        public void Step_two() { }
        public void Step_with_ignore_assertion() { Assert.Ignore(IgnoreReason); }
        public void Step_with_inconclusive_assertion() { Assert.Inconclusive("some reason"); }
        public void Step_with_bypass() { Step.Bypass(BypassReason); }
        public void Step_with_bypass2() { Step.Bypass(BypassReason2); }
    }
}