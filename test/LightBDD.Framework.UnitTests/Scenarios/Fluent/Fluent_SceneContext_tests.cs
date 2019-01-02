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
		[Test]
		public void FluentApi_Context_should_generate_array_of_steps()
		{
			ExpectSynchronousExecution();

			new SceneContext<Fluent_SceneContext_tests, Fluent_SceneContext_tests, Fluent_SceneContext_tests, Action>(this, this, this, a => this.Runner.Given(a))
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

			var c = new SceneContext<Fluent_SceneContext_tests, Fluent_SceneContext_tests, Fluent_SceneContext_tests, Func<Task>>(this, this, this, b => this.Runner.Given(b))
				.Given.TaskA
				.When.TaskB
				.Then.TaskC
				.RunAsync();

			VerifyAllExpectations();

			Assert.That(CapturedSteps, Is.Not.Null);
			Assert.That(CapturedSteps.Length, Is.EqualTo(3));
		}


		GivenResult<Fluent_SceneContext_tests, Fluent_SceneContext_tests> A => SceneContext<Fluent_SceneContext_tests, Fluent_SceneContext_tests, Fluent_SceneContext_tests, Action>.CreateGiven(() =>
		{
		});
		WhenResult<Fluent_SceneContext_tests, Fluent_SceneContext_tests> B => SceneContext<Fluent_SceneContext_tests, Fluent_SceneContext_tests, Fluent_SceneContext_tests, Action>.CreateWhen(() =>
		{
		});
		ThenResult<Fluent_SceneContext_tests,Action> C => SceneContext<Fluent_SceneContext_tests, Fluent_SceneContext_tests, Fluent_SceneContext_tests, Action>.CreateThen(() =>
		{
		});

		GivenResult<Fluent_SceneContext_tests, Fluent_SceneContext_tests> TaskA => SceneContext<Fluent_SceneContext_tests, Fluent_SceneContext_tests, Fluent_SceneContext_tests, Func<Task>>.CreateGiven(() => new Task(() => { }));
		WhenResult<Fluent_SceneContext_tests, Fluent_SceneContext_tests> TaskB => SceneContext<Fluent_SceneContext_tests, Fluent_SceneContext_tests, Fluent_SceneContext_tests, Func<Task>>.CreateWhen(() => new Task(() => { }));
		ThenResult<Fluent_SceneContext_tests, Func<Task>> TaskC => SceneContext<Fluent_SceneContext_tests, Fluent_SceneContext_tests, Fluent_SceneContext_tests, Func<Task>>.CreateThen(()=> new Task( () =>{}));
	}
}
