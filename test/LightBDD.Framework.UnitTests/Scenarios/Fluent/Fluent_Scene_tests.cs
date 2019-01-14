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
			var (stepsCapture, runCapture) = ExpectBasicScenarioRun();

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

			Builder.Verify();
			Assert.That(runCapture.Value, Is.True);
			Assert.That(stepsCapture.Count, Is.EqualTo(9));
		}
	}
}
