using LightBDD;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace $rootnamespace$
{
	[Label("FEAT-1"), FeatureDescription(
@"In order to 
As a
I want to ")]
	[TestClass]
	public partial class $safeitemname$
	{
		[Label("SCENARIO-1"), TestMethod]
		public void Template_scenario()
		{
			_runner.RunScenario(
				given => Template_method(),
				when => Template_method(),
				then => Template_method());
		}
	}
}