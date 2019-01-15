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

		class ActionTestContext : SceneContext<Givens, Whens, Thens, Action>
		{
			public ActionTestContext(IBddRunner runner)	: base(null, null, null, a => runner.Given(a))
			{
				Init(new Givens(), new Whens(), new Thens());
			}
		}

		class Givens
		{
			[Given("ok")]
			void Given_A()
			{
			}
			public GivenResult<Givens, Whens> A => ActionTestContext.CreateGiven(Given_A);
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
