using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LightBDD.Framework.UnitTests.Scenarios.Fluent
{
	partial class Fluent_Scene_tests
	{
		void Given_A()
		{
		}
		void Given_B()
		{
		}

		void When_A()
		{
		}
		void When_B()
		{
		}

		void Then_A()
		{
		}

		void Then_B()
		{
		}

		Task Task_Given_A()
		{
			return new Task(Given_A);
		}
		Task Task_Given_B()
		{
			return new Task(Given_B);
		}
		Task Task_When_A()
		{
			return new Task(When_A);
		}
		Task Task_When_B()
		{
			return new Task(When_B);
		}

		Task Task_Then_A()
		{
			return new Task(Then_A);
		}
		Task Task_Then_B()
		{
			return new Task(Then_B);
		}
	}
}
