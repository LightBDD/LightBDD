using System;
using System.Collections.Generic;
using System.Text;
using LightBDD.Framework;
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
				.Given(A)
				.When(B)
				.Then(C)
				.Run();
		}

		[Description("Given")]
		void A()
		{
		}
		void B()
		{
		}
		void C()
		{
		}
	}
}
