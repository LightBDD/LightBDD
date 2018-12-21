using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LightBDD.Framework.Scenarios.Fluent
{
	public class SceneAsync : SceneBase<Func<Task>>
	{
		public static IGiven<Func<Task>> Given(Func<Task> given)
		{
			return new SceneAsync(given);
		}

		SceneAsync(Func<Task> given) : base(given)
		{
		}
	}
}
