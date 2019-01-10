using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LightBDD.Framework.Scenarios.Fluent.AsyncExtensions;
using LightBDD.Framework.UnitTests.Scenarios.Basic.Helpers;
using NUnit.Framework;

namespace LightBDD.Framework.UnitTests.Scenarios.Fluent
{
	[TestFixture]
	public class Fluent_Scene_Async_tests : BasicScenarioTestsBase
	{
		[Test]
		public async Task FluentApi_should_generate_array_of_steps_async()
		{
			ExpectAsynchronousExecution();

			await Runner
				.Given(Task_Given_A)
				.And(Task_Given_B)
				.When(Task_When_A)
				.And(Task_When_B)
				.Then(Task_Then_A)
				.And(Task_Then_B)
				.RunAsync();

			VerifyAllExpectations();

			Assert.That(CapturedSteps, Is.Not.Null);
			Assert.That(CapturedSteps.Length, Is.EqualTo(6));
		}

		Task Task_Given_A()
		{
			return new Task(() =>
			{
			});
		}
		Task Task_Given_B()
		{
			return new Task(() =>
			{
			});
		}
		Task Task_When_A()
		{
			return new Task(() =>
			{
			});
		}
		Task Task_When_B()
		{
			return new Task(() =>
			{
			});
		}

		Task Task_Then_A()
		{
			return new Task(() =>
			{
			});
		}
		Task Task_Then_B()
		{
			return new Task(() =>
			{
			});
		}
	}
}
