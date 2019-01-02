using System;
using System.Collections.Generic;
using System.Text;

namespace LightBDD.Framework.Scenarios.Fluent
{
	public class ThenResult<TThen,TAction>
	{
		internal static IThen<TAction> then;

		public ThenResult(TThen then)
		{
			this.And = then;
		}

		public TThen And
		{
			get;
		}

		public IThen<TAction> To() => then;
	}
}
