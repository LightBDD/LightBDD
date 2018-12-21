using System;
using System.Collections.Generic;
using System.Text;
using LightBDD.Core.Formatting;
using LightBDD.Framework;
using LightBDD.Framework.Formatting;
using LightBDD.Framework.Scenarios.Fluent;
using NUnit.Framework;

namespace LightBDD.NUnit3.UnitTests
{
	[TestFixture]
	[Description("")]
	[Category("LightBDD")]
	[ScenarioCategory("Scene-Kategorie")]
	public class Scene_Fluent_Tests : FeatureFixture
	{
		[Test]
		[Scenario]
		[Label(nameof(Test))]
		public void Test()
		{
			this.Runner
				.Given(Given_A)
				.When(B)
				.Then(C)
				.Run();
		}

		[Given("A")]
		void Given_A()
		{
		}
		[When]
		void B()
		{
		}
		[Then]
		void C()
		{
		}
	}
}
