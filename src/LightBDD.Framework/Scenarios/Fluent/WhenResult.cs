using System;
using System.Collections.Generic;
using System.Text;

namespace LightBDD.Framework.Scenarios.Fluent
{
	public class WhenResult<TWhen,TThen>
	{
		public WhenResult(TWhen when, TThen then)
		{
			this.Then = then;
			this.And = when;
		}

		public TWhen And
		{
			get;
		}
		public TThen Then
		{
			get;
		}
	}
}
