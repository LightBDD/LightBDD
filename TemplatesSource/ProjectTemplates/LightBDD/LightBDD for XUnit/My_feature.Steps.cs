using LightBDD;
using Xunit;
using Xunit.Abstractions;

namespace $safeprojectname$
{
	public partial class My_feature: FeatureFixture
	{
		public My_feature(ITestOutputHelper output) : base(output)
		{
		}

		private void Template_method()
		{
			ScenarioAssert.Ignore("Not implemented yet");
		}
	}
}