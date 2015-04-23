using LightBDD;
using LightBDD.Coordination;
using MbUnit.Framework;

namespace $rootnamespace$
{
	public partial class $safeitemname$
	{
		private BDDRunner _runner;

		[FixtureSetUp]
		public void FixtureSetUp()
		{
			_runner = new BDDRunner(GetType());
		}

		[FixtureTearDown]
		public void FixtureTearDown()
		{
			FeatureCoordinator.Instance.AddFeature(_runner.Result);
		}

		private void Given_template_method()
		{
		}

		private void When_template_method()
		{
		}

		private void Then_template_method()
		{
		}
	}
}