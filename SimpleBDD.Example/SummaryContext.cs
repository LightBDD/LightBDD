using NUnit.Framework;

namespace SimpleBDD.Example
{
	[SetUpFixture]
	public class SummaryContext
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