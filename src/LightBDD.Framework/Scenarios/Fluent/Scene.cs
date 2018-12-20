using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LightBDD.Framework.Scenarios.Fluent
{
	public class Scene : SceneBase<Action>
	{
		public static IGiven<Action> Given(Action given)
		{
			return new Scene(given);
		}

		Scene(Action given) : base(given)
		{
		}
	}
}
