using LightBDD.Framework.Scenarios.Fluent;
using LightBDD.Framework.UnitTests.Scenarios.Basic.Helpers;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

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
	}
}
