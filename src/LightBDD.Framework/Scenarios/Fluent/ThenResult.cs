using System;
using System.Collections.Generic;
using System.Text;

namespace LightBDD.Framework.Scenarios.Fluent
{
	public class ThenResult<T,A>
	{
		internal static IThen<A> then;

		public ThenResult(T then)
		{
			this.And = then;
		}

		public T And
		{
			get;
		}

		public IThen<A> To() => then;
	}
}
