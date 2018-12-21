using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LightBDD.Core.Extensibility;
using LightBDD.Framework.Scenarios.Basic;
using LightBDD.Framework.Scenarios.Fluent;
using LightBDD.Framework.UnitTests.Scenarios.Basic.Helpers;
using Moq;
using NUnit.Framework;

namespace LightBDD.Framework.UnitTests.Scenarios.Fluent
{
	[TestFixture]
	public partial class Fluent_Scene_tests : BasicScenarioTestsBase
	{

		[Test]
		public void FluentApi_should_generate_array_of_steps()
		{
			ExpectSynchronousExecution();

			Runner
				.Given(Given_A)
				.Given(Given_A)
				.And(Given_B)
				.When(When_A)
				.When(When_A)
				.And(When_B)
				.Then(Then_A)
				.Then(Then_A)
				.And(Then_B)
				.Run();

			VerifyAllExpectations();

			Assert.That(CapturedSteps, Is.Not.Null);
			Assert.That(CapturedSteps.Length, Is.EqualTo(9));
		}

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
	}
}
