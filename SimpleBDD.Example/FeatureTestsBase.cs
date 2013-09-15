using NUnit.Framework;

namespace SimpleBDD.Example
{
	public abstract class FeatureTestsBase
	{
		protected BDDRunner _runner;

		[TestFixtureSetUp]
		public void FixtureSetUp()
		{
			_runner = new BDDRunner(GetType());
		}

		[TestFixtureTearDown]
		public void FixtureTearDown()
		{
			AcceptanceTests.Summary.AddResult(_runner.Result);
		}
	}
}
