using LightBDD.Framework.Scenarios.Fluent;
using System;
using System.Collections.Generic;
using System.Text;

namespace LightBDD.Framework.UnitTests.Scenarios.Fluent
{
	partial class Fluent_SceneContext_tests
	{
		class ActionTestContext	: SceneContext<
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

			void Given_A() { }
			void When_B() { }
			void Then_C() { }

			public GivenResult<ActionTestContext, ActionTestContext> A
				=> CreateGiven(Given_A);
			public WhenResult<ActionTestContext, ActionTestContext> B(int i)
				=> CreateWhen(When_B);
			public ThenResult<ActionTestContext, Action> C
				=> CreateThen(Then_C);
		}
	}
}
