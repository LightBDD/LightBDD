using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LightBDD.Framework.Scenarios.Basic;

namespace LightBDD.Framework.Scenarios.Fluent.AsyncExtensions
{
	/// <summary>
	/// Extention methods for Given-When-Then
	/// </summary>
	public static class AsyncSceneExtensions
	{
		/// <summary>
		/// Given Extention for IBddRunner 
		/// </summary>
		/// <param name="runner">Test-Runner</param>
		/// <param name="given">Test-Func to be executed</param>
		/// <returns>Given-object</returns>
		public static IGiven<Func<Task>> Given(this IBddRunner runner, Func<Task> given)
		{
			return new Scene<Func<Task>>(runner)
				.Given(given);
		}

		/// <summary>
		/// Run async the definition of the SceneContext.
		/// <see cref="SceneContext{TGiven, TWhen, TThen, TFunc_Given}"/>
		/// Execute the steps
		/// </summary>
		/// <typeparam name="TThen">Type of then-methods class</typeparam>
		/// <param name="then">ThenResult object</param>
		public static Task RunAsync<TThen>(this ThenResult<TThen, Func<Task>> then)
		{
			var scene = (Scene<Func<Task>>)then.To();
			return scene.Runner.RunScenarioAsync(scene.End());
		}

		/// <summary>
		/// Run Async the definition of the Scene
		/// <see cref="Scene{TAction}"/>
		/// </summary>
		/// <param name="then">Then-object</param>
		public static Task RunAsync(this IThen<Func<Task>> then) 
		{
			var scene = (Scene<Func<Task>>)then;
			return scene.Runner.RunScenarioAsync(scene.End());
		}
	}
}
