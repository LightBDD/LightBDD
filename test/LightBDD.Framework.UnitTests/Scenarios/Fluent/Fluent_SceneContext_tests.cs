using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LightBDD.Framework.Scenarios.Fluent;
using LightBDD.Framework.Scenarios.Fluent.AsyncExtensions;
using LightBDD.Framework.UnitTests.Scenarios.Basic.Helpers;
using NUnit.Framework;

namespace LightBDD.Framework.UnitTests.Scenarios.Fluent
{
	[TestFixture]
	public partial class Fluent_SceneContext_tests : BasicScenarioTestsBase
	{
		[Test]
		public void FluentApi_Context_should_generate_array_of_steps()
		{
			var (stepsCapture, runCapture) = ExpectBasicScenarioRun();

			new ActionTestContext(this.Runner)
				.Given.A
				.When.B(5)
				.Then.C
				.Run();

			Builder.Verify();
			Assert.That(runCapture.Value, Is.True);
			Assert.That(stepsCapture.Count, Is.EqualTo(3));
		}

		[Test]
		public async Task FluentApi_should_generate_array_of_steps_async()
		{
			var (stepsCapture, runCapture) = ExpectBasicScenarioRun();

			await new FuncTestContext(this.Runner)
				.Given.TaskA
				.When.TaskB
				.Then.TaskC
				.RunAsync();

			Builder.Verify();
			Assert.That(runCapture.Value, Is.True);
			Assert.That(stepsCapture.Count, Is.EqualTo(3));
		}
	}
}
