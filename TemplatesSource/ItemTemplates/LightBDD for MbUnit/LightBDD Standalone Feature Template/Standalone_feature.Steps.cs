using LightBDD;
using LightBDD.Coordination;
using MbUnit.Framework;

namespace $rootnamespace$
{
	public partial class $safeitemname$
	{
		private readonly BDDRunner _runner;

		public $safeitemname$()
		{
			_runner = new BDDRunner(GetType());
		}

		[FixtureTearDown]
		public void FixtureTearDown()
		{
			FeatureCoordinator.Instance.AddFeature(_runner.Result);
		}

		private void Template_method()
		{
			Assert.Inconclusive("Not implemented yet");
		}
	}
}