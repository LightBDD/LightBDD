using System;
using LightBDD.Execution;
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
		public void Step_with_comment() { StepExecution.Comment("comment one"); StepExecution.CommentFormat("comment {0}", 2); }
		public void Step_with_other_comment() { StepExecution.Comment("other comment"); }
		public void Step_throwing_exception() { throw new InvalidOperationException(ExceptionText); }
		public void Step_throwing_exception_MESSAGE(string message) { throw new InvalidOperationException(message); }
		public void Step_two() { }
		public void Step_with_ignore_assertion() { Assert.Ignore(IgnoreReason); }
		public void Step_with_inconclusive_assertion() { Assert.Inconclusive("some reason"); }
		public void Step_with_bypass() { StepExecution.Bypass(BypassReason); }
		public void Step_with_bypass2() { StepExecution.Bypass(BypassReason2); }
		public void Given_something() { }
		public void Given_something_else() { }
		public void When_something() { }
		public void When_something_else() { }
		public void Then_something() { }
		public void Then_something_else() { }
	}
}