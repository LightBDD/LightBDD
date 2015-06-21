using LightBDD;
using NUnit.Framework;

namespace $rootnamespace$
{
	[Label("FEAT-1"), FeatureDescription(
@"In order to 
As a
I want to ")]
	[TestFixture]
	public partial class $safeitemname$
	{
		[Label("SCENARIO-1"), Test]
		public void Template_scenario()
		{
			Runner.RunScenario(
				given => Template_method(),
				when => Template_method(),
				then => Template_method());
		}
	}
}