using NUnit.Framework;

namespace SimpleBDD.Example
{
	[SetUpFixture]
	public class SummaryContext
	{
		public static FeaturesSummary Summary { get; private set; }

		[SetUp]
		public void SetUp()
		{
			Summary = new FeaturesSummary();
		}

		[TearDown]
		public void TearDown()
		{
			Summary.SaveSummary("FeaturesSummary.xml");
		}
	}
}