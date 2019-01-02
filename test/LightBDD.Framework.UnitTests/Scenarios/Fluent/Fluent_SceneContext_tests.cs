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
			: SceneContext<Fluent_SceneContext_tests
				, Fluent_SceneContext_tests
				, Fluent_SceneContext_tests
				, Action>
		{
			public ActionTestContext(Fluent_SceneContext_tests context) 
				: base(context, context, context, a => context.Runner.Given(a))
			{
			}
		}
		class FuncTestContext
			: SceneContext<Fluent_SceneContext_tests
				, Fluent_SceneContext_tests
				, Fluent_SceneContext_tests
				, Func<Task>>
		{
			public FuncTestContext(Fluent_SceneContext_tests context)
				: base(context, context, context, a => context.Runner.Given(a))
			{
			}
		}
	
		[Test]
		public void FluentApi_Context_should_generate_array_of_steps()
		{
			ExpectSynchronousExecution();

			new ActionTestContext(this)
				.Given.A
				.When.B
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


		GivenResult<Fluent_SceneContext_tests, Fluent_SceneContext_tests> A => ActionTestContext.CreateGiven(Given_A);
		WhenResult<Fluent_SceneContext_tests, Fluent_SceneContext_tests> B => ActionTestContext.CreateWhen(When_B);
		ThenResult<Fluent_SceneContext_tests,Action> C => ActionTestContext.CreateThen(Then_C);

		GivenResult<Fluent_SceneContext_tests, Fluent_SceneContext_tests> TaskA => FuncTestContext.CreateGiven(Given_Task_A);
		WhenResult<Fluent_SceneContext_tests, Fluent_SceneContext_tests> TaskB => FuncTestContext.CreateWhen(When_Task_B);
		ThenResult<Fluent_SceneContext_tests, Func<Task>> TaskC => FuncTestContext.CreateThen(Then_Task_C);

		void Given_A() { }
		void When_B() { }
		void Then_C() { }
		Task Given_Task_A() => new Task(() => { });
		Task When_Task_B() => new Task(() => { });
		Task Then_Task_C() => new Task(() => { });
	}
}
