using LightBDD.Framework.Scenarios.Fluent;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LightBDD.Framework.UnitTests.Scenarios.Fluent
{
	partial class Fluent_SceneContext_tests
	{
		class FuncTestContext	: SceneContext<
		FuncTestContext
		, FuncTestContext
		, FuncTestContext
		, Func<Task>>
		{
			public FuncTestContext(IBddRunner runner)
				: base(null, null, null, a => runner.Given(a))
			{
				Init(this, this, this);
			}

			Task Given_Task_A() => new Task(() => { });
			Task When_Task_B() => new Task(() => { });
			Task Then_Task_C() => new Task(() => { });

			public GivenResult<FuncTestContext, FuncTestContext> TaskA
				=> CreateGiven(Given_Task_A);
			public WhenResult<FuncTestContext, FuncTestContext> TaskB
				=> CreateWhen(When_Task_B);
			public ThenResult<FuncTestContext, Func<Task>> TaskC
				=> CreateThen(Then_Task_C);
		}
	}
}
