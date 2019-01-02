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
			public static ActionTestContext Instance { get; private set; }

			public ActionTestContext(Fluent_SceneContext_tests context) 
				: base(context, context, context, a => context.Runner.Given(a))
			{
				Instance = this;
			}
			public void Given_A() { }
			public void When_B() { }
			public void Then_C() { }
		}
		class FuncTestContext
			: SceneContext<Fluent_SceneContext_tests
				, Fluent_SceneContext_tests
				, Fluent_SceneContext_tests
				, Func<Task>>
		{
			public static FuncTestContext Instance { get; private set; }

			public FuncTestContext(Fluent_SceneContext_tests context)
				: base(context, context, context, a => context.Runner.Given(a))
			{
				Instance = this;
			}

			public Task Given_Task_A() => new Task(() => { });
			public Task When_Task_B() => new Task(() => { });
			public Task Then_Task_C() => new Task(() => { });
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

		GivenResult<Fluent_SceneContext_tests, Fluent_SceneContext_tests> A 
			=> ActionTestContext.CreateGiven(ActionTestContext.Instance.Given_A);
		WhenResult<Fluent_SceneContext_tests, Fluent_SceneContext_tests> B 
			=> ActionTestContext.CreateWhen(ActionTestContext.Instance.When_B);
		ThenResult<Fluent_SceneContext_tests,Action> C 
			=> ActionTestContext.CreateThen(ActionTestContext.Instance.Then_C);

		GivenResult<Fluent_SceneContext_tests, Fluent_SceneContext_tests> TaskA 
			=> FuncTestContext.CreateGiven(FuncTestContext.Instance.Given_Task_A);
		WhenResult<Fluent_SceneContext_tests, Fluent_SceneContext_tests> TaskB 
			=> FuncTestContext.CreateWhen(FuncTestContext.Instance.When_Task_B);
		ThenResult<Fluent_SceneContext_tests, Func<Task>> TaskC 
			=> FuncTestContext.CreateThen(FuncTestContext.Instance.Then_Task_C);
	}
}
