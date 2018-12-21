using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LightBDD.Framework.Scenarios.Basic;

namespace LightBDD.Framework.Scenarios.Fluent
{
	public static class SceneExtentions
	{
		public static IGiven<Action> Given(this IBddRunner runner, Action given)
		{
			return new Scene<Action>(given, runner);
		}

		public static IGiven<Func<Task>> Given(this IBddRunner runner, Func<Task> given)
		{
			return new Scene<Func<Task>>(given, runner);
		}

		public static void Run<T>(this IThen<T> then)
		{
			var scene = (Scene<Action>)then;
			scene.Runner.RunScenario(scene.End());
		}

		public static Task RunAsync<T>(this IThen<T> then)
		{
			var scene = (Scene<Func<Task>>)then;
			return scene.Runner.RunScenarioAsync(scene.End());
		}
	}
}
