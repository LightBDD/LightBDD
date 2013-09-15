using NUnit.Framework;

namespace LightBDD.Example
{
	public abstract class FeatureTestsBase
	{
		protected BDDRunner Runner { get; private set; }

		[TestFixtureSetUp]
		public void FixtureSetUp()
		{
			Runner = new BDDRunner(GetType());
		}

		[TestFixtureTearDown]
		public void FixtureTearDown()
		{
			FeatureTests.Summary.AddResult(Runner.Result);
		}
	}
}
