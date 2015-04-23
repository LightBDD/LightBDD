using LightBDD;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace $safeprojectname$
{
	[Label("FEAT-1"), FeatureDescription(
@"In order to 
As a
I want to ")]
	[TestClass]
	public partial class My_feature
	{
		[Label("SCENARIO-1"), TestMethod]
		public void Template_scenario()
		{
			Runner.RunScenario(
				given => Template_method(),
				when => Template_method(),
				then => Template_method());
		}
	}
}