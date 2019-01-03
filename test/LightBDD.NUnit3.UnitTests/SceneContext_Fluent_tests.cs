using System;
using System.Collections.Generic;
using System.Text;
using LightBDD.Core.Formatting;
using LightBDD.Framework;
using LightBDD.Framework.Scenarios.Fluent;
using NUnit.Framework;

namespace LightBDD.NUnit3.UnitTests
{
	[TestFixture]
	[Description("")]
	[Category("LightBDD")]
	[ScenarioCategory("SceneContext-Kategorie")]
	public class SceneContext_Fluent_tests : FeatureFixture
	{
		[Test]
		[Scenario]
		[Label(nameof(Test))]
		public void Test()
		{
			new ActionTestContext(this.Runner)
				.Given.A
				.When.B(5)
				.Then.C
				.Run();
		}

		class ActionTestContext : SceneContext<
		ActionTestContext
		, ActionTestContext
		, ActionTestContext
		, Action>
		{
			public ActionTestContext(IBddRunner runner)
				: base(null, null, null, a => runner.Given(a))
			{
				Init(this, this, this);
			}

			[Given("description test")]
			void Given_A()
			{
			}
			void When_B()
			{
			}
			void Then_C()
			{
			}

			public GivenResult<ActionTestContext, ActionTestContext> A
				=> CreateGiven(Given_A);
			public WhenResult<ActionTestContext, ActionTestContext> B(int i)
				=> CreateWhen(When_B);
			public ThenResult<ActionTestContext, Action> C
				=> CreateThen(Then_C);
		}
	}
}
