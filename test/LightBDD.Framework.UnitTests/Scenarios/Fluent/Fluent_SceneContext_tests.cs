using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LightBDD.Framework.Scenarios.Fluent;
using LightBDD.Framework.UnitTests.Scenarios.Basic.Helpers;
using NUnit.Framework;

namespace LightBDD.Framework.UnitTests.Scenarios.Fluent
{
	[TestFixture]
	public class Fluent_SceneContext_tests : BasicScenarioTestsBase
	{
		class ActionTestContext 
			: SceneContext<
				ActionTestContext
				, ActionTestContext
				, ActionTestContext
				, Action>
		{
			public ActionTestContext(Fluent_SceneContext_tests context) 
				: base(null, null, null, a => context.Runner.Given(a))
			{
				Init(this, this, this);
			}

			void Given_A() { }
			void When_B() { }
			void Then_C() { }

			public GivenResult<ActionTestContext, ActionTestContext> A
				=> CreateGiven(Given_A);
			public WhenResult<ActionTestContext, ActionTestContext> B()
				=> CreateWhen(When_B);
			public ThenResult<ActionTestContext, Action> C
				=> CreateThen(Then_C);
		}
		class FuncTestContext
			: SceneContext<
				FuncTestContext
				, FuncTestContext
				, FuncTestContext
				, Func<Task>>
		{
			public FuncTestContext(Fluent_SceneContext_tests context)
				: base(null, null, null, a => context.Runner.Given(a))
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

		[Test]
		public void FluentApi_Context_should_generate_array_of_steps()
		{
			ExpectSynchronousExecution();

			new ActionTestContext(this)
				.Given.A
				.When.B()
				.Then.C
				.Run();			

			VerifyAllExpectations();

			Assert.That(CapturedSteps, Is.Not.Null);
			Assert.That(CapturedSteps.Length, Is.EqualTo(3));
		}

		[Test]
		public async Task FluentApi_should_generate_array_of_steps_async()
		{
			ExpectAsynchronousExecution();

			await new FuncTestContext(this)
				.Given.TaskA
				.When.TaskB
				.Then.TaskC
				.RunAsync();

			VerifyAllExpectations();

			Assert.That(CapturedSteps, Is.Not.Null);
			Assert.That(CapturedSteps.Length, Is.EqualTo(3));
		}
	}
}
