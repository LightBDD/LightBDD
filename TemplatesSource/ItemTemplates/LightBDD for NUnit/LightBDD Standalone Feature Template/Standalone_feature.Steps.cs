using NUnit.Framework;
using LightBDD;
using LightBDD.Coordination;

namespace $rootnamespace$
{
	public partial class $safeitemname$
	{
		private BDDRunner _runner;

		[TestFixtureSetUp]
		public void FixtureSetUp()
		{
			_runner = new BDDRunner(GetType());
		}

		[TestFixtureTearDown]
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