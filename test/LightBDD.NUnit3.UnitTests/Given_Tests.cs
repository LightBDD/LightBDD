using LightBDD.Framework.Implementation;
using System;
using System.Collections.Generic;
using System.Text;

namespace LightBDD.NUnit3.UnitTests
{
	public class Given_Tests : FeatureFixture
	{
		public void Fluent()
		{
			Feature
				.Given(() => { })
				.And(() => { })
			
				.When(() => { })
				.And(() => { })
				
				.Then(() => { })
				.And(() => { })
				
				.Run(this.Runner);
		}
	}
}
