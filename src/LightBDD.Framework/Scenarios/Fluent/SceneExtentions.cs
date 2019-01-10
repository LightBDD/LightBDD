using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LightBDD.Framework.Scenarios.Basic;

namespace LightBDD.Framework.Scenarios.Fluent
{
	/// <summary>
	/// Extention methods for Given-When-Then
	/// </summary>
	public static class SceneExtentions
	{
		/// <summary>
		/// Given Extention for IBddRunner 
		/// </summary>
		/// <param name="runner">Test-Runner</param>
		/// <param name="given">Test-Action to be executed</param>
		/// <returns>Given-object</returns>
		public static IGiven<Action> Given(this IBddRunner runner, Action given)
		{
			return new Scene<Action>(runner)
				.Given(given);
		}

		/// <summary>
		/// Run the definition of the SceneContext.
		/// <see cref="SceneContext{TGiven, TWhen, TThen, TFunc_Given}"/>
		/// Execute the steps
		/// </summary>
		/// <typeparam name="TThen">Type of then-methods class</typeparam>
		/// <param name="then">ThenResult object</param>
		public static void Run<TThen>(this ThenResult<TThen, Action> then)
		{
			var scene = (Scene<Action>)then.To();
			scene.Runner.RunScenario(scene.End());
		}

		/// <summary>
		/// Run the definition of the Scene
		/// <see cref="Scene{TAction}"/>
		/// </summary>
		/// <param name="then">Then-object</param>
		public static void Run(this IThen<Action> then)
		{
			var scene = (Scene<Action>)then;
			scene.Runner.RunScenario(scene.End());
		}

		/// <summary>
		/// Run actions async the definition of the SceneContext.
		/// <see cref="SceneContext{TGiven, TWhen, TThen, TFunc_Given}"/>
		/// Execute the steps
		/// </summary>
		/// <typeparam name="TThen">Type of then-methods class</typeparam>
		/// <param name="then">ThenResult object</param>
		public static Task RunActionsAsync<TThen>(this ThenResult<TThen, Action> then)
		{
			var scene = (Scene<Action>)then.To();
			return scene.Runner.RunScenarioActionsAsync(scene.End());
		}

		/// <summary>
		/// Run Actions Async the definition of the Scene
		/// <see cref="Scene{TAction}"/>
		/// </summary>
		/// <param name="then">Then-object</param>
		public static Task RunActionsAsync(this IThen<Action> then)
		{
			var scene = (Scene<Action>)then;
			return scene.Runner.RunScenarioActionsAsync(scene.End());
		}
	}
}
