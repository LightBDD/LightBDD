using LightBDD.Framework.Scenarios.Fluent;
using System;
using System.Collections.Generic;
using System.Text;

namespace LightBDD.Framework.UnitTests.Scenarios.Fluent
{
	partial class Fluent_SceneContext_tests
	{
		class ActionTestContext	: SceneContext<
		Givens
		, Whens
		, Thens
		, Action>
		{
			public ActionTestContext(IBddRunner runner)
				: base(null, null, null, a => runner.Given(a))
			{
				Init(new Givens(), new Whens(), new Thens());
			}
		}

		class Givens
		{
			void Given_A()
			{
			}
			public GivenResult<Givens, Whens> A	=> ActionTestContext.CreateGiven(Given_A);
		}
		class Whens
		{
			int i;

			void When_B()
			{
			}
			public WhenResult<Whens, Thens> B(int i)
			{
				this.i = i;
				return ActionTestContext.CreateWhen(When_B);
			}
		}

		class Thens
		{
			void Then_C()
			{
			}

			public ThenResult<Thens, Action> C => ActionTestContext.CreateThen(Then_C);
		}
	}
}
