using LightBDD;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace $rootnamespace$
{
	[FeatureDescription(
@"In order to 
As a
I want to ")]
	[TestClass]
	public partial class $safeitemname$
	{
		[TestMethod]
		public void Template_scenario()
		{
			Runner.RunScenario(
				Given_template_method,
				When_template_method,
				Then_template_method);
		}
	}	
}