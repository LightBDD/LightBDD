using System;
using System.Collections.Generic;
using System.Text;

namespace LightBDD.Framework.Scenarios.Fluent
{
	public class WhenResult<W,T>
	{
		public WhenResult(W when, T then)
		{
			this.Then = then;
			this.And = when;
		}

		public W And
		{
			get;
		}
		public T Then
		{
			get;
		}
	}
}
