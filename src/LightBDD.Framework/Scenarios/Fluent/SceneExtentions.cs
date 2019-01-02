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
			return new Scene<Action>(runner)
				.Given(given);
		}

		public static IGiven<Func<Task>> Given(this IBddRunner runner, Func<Task> given)
		{
			return new Scene<Func<Task>>(runner)
				.Given(given);
		}

		public static void Run<T>(this ThenResult<T, Action> then)
		{
			var scene = (Scene<Action>)then.To();
			scene.Runner.RunScenario(scene.End());
		}

		public static void Run(this IThen<Action> then)
		{
			var scene = (Scene<Action>)then;
			scene.Runner.RunScenario(scene.End());
		}

		public static Task RunAsync<T>(this ThenResult<T, Func<Task>> then)
		{
			var scene = (Scene<Func<Task>>)then.To();
			return scene.Runner.RunScenarioAsync(scene.End());
		}

		public static Task RunAsync(this IThen<Func<Task>> then) 
		{
			var scene = (Scene<Func<Task>>)then;
			return scene.Runner.RunScenarioAsync(scene.End());
		}

		public static Task RunActionsAsync<T>(this ThenResult<T, Action> then)
		{
			var scene = (Scene<Action>)then.To();
			return scene.Runner.RunScenarioActionsAsync(scene.End());
		}

		public static Task RunActionsAsync(this IThen<Action> then)
		{
			var scene = (Scene<Action>)then;
			return scene.Runner.RunScenarioActionsAsync(scene.End());
		}
	}
}
