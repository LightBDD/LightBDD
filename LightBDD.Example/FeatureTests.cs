using NUnit.Framework;

namespace LightBDD.Example
{
	/// <summary>
	/// Class gathering all feature tests output and saving it into summary xml file after all tests are finished.
	/// Please note that this class HAS TO BE PLACED IN SAME ON PARENT NAMESPACE to all feature tests in order to save all results.
	/// </summary>
	[SetUpFixture]
	public class FeatureTests
	{
		public static TestResultsSummary Summary { get; private set; }

		[SetUp]
		public void SetUp()
		{
			Summary = new TestResultsSummary();
		}

		[TearDown]
		public void TearDown()
		{
			Summary.SaveSummary("FeaturesSummary.xml");
		}
	}
}