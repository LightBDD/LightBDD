using System;
using System.Collections.Generic;
using System.Text;
using LightBDD.Core.Extensibility;
using LightBDD.Framework.Scenarios.Basic;
using LightBDD.Framework.Scenarios.Fluent;
using LightBDD.Framework.UnitTests.Scenarios.Basic.Helpers;
using Moq;
using NUnit.Framework;

namespace LightBDD.Framework.UnitTests.Scenarios.Fluent
{
	[TestFixture]
	public class Fluent_Scene_tests 
	{

		[Test]
		public void Runner_should_accept_Fluent_Api()
		{
			int given = 0;
			int when = 0;
			int then = 0;

			var actions =
				SceneBase<Action>.GivenAsync
				.Given(() => ++given)
				.And(() => ++given)
				.When(() => ++when)
				.And(() => ++when)
				.Then(() => ++then)
				.And(() => ++then)
				.End();

			foreach (var action in actions)
				action();

			Assert.That(actions.Length, Is.EqualTo(6));
			Assert.That(given, Is.EqualTo(2));
			Assert.That(when, Is.EqualTo(2));
			Assert.That(then, Is.EqualTo(2));
		}
	}
}
