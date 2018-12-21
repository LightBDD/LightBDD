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
	public class Fluent_Scene_tests 
	{

		[Test]
		public void FluentApi_should_generate_array_of_steps()
		{
			int given = 0;
			int when = 0;
			int then = 0;

			var runner = Mock.Of<IBddRunner>();
			
			runner
				.Given(() => ++given)
				.And(() => ++given)
				.When(() => ++when)
				.And(() => ++when)
				.Then(() => ++then)
				.And(() => ++then)
				.Run();

			return;
		}

		[Test]
		public Task FluentApi_should_generate_array_of_steps_async()
		{
			int given = 0;
			int when = 0;
			int then = 0;

			var runner = Mock.Of<IBddRunner>();

			return runner
				.Given(() => new Task(() => ++given))
				.And(() => new Task(() => ++given))
				.When(() => new Task(() => ++when))
				.And(() => new Task(() => ++when))
				.Then(() => new Task(() => ++then))
				.And(() => new Task(() => ++then))
				.RunAsync();
		}
	}
}
