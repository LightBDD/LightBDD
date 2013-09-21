using System;
using NUnit.Framework;

namespace LightBDD.UnitTests
{
	public class SomeSteps
	{
		public void Step_one() { }
		public void Step_throwing_exception() { throw new InvalidOperationException("exception text"); }
		public void Step_two() { }
		public void Step_with_ignore_assertion() { Assert.Ignore("some reason"); }
		public void Step_with_inconclusive_assertion() { Assert.Inconclusive("some reason"); }
	}
}