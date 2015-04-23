using LightBDD;
using MbUnit.Framework;

namespace $rootnamespace$
{
	[FeatureDescription(
@"In order to 
As a
I want to ")]
	[TestFixture]
	public partial class $safeitemname$
	{
		[Test]
		public void Template_scenario()
		{
			_runner.RunScenario(
				Given_template_method,
				When_template_method,
				Then_template_method);
		}
	}	
}