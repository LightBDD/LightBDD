using LightBDD.Results;
using LightBDD.Results.Formatters;

namespace LightBDD.Coordination
{
	/// <summary>
	/// Feature summary aggregator allowing to collect feature results in order to save summary in output file.
	/// </summary>
	public class FeatureSummaryAggregator : IFeatureAggregator
	{
		private readonly TestResultsSummary _summary;

		/// <summary>
		/// File path where summary would be saved.
		/// </summary>
		public string FilePath { get; private set; }

		/// <summary>
		/// Default constructor, using XmlResultsFormatter for summary formatting and "FeaturesSummary.xml" for FilePath.
		/// </summary>
		public FeatureSummaryAggregator()
			: this(new XmlResultFormatter(), "FeaturesSummary.xml")
		{
		}
		/// <summary>
		/// Constructor allowing to specify result formatter and output file path. 
		/// </summary>
		/// <param name="resultFormatter">Formatter.</param>
		/// <param name="filePath">Output file path.</param>
		public FeatureSummaryAggregator(IResultFormatter resultFormatter, string filePath)
		{
			_summary = new TestResultsSummary(resultFormatter);
			FilePath = filePath;
		}

		#region IFeatureAggregator Members

		/// <summary>
		/// Aggregates given feature.
		/// </summary>
		/// <param name="feature">Feature to aggregate.</param>
		public void AddFeature(IFeatureResult feature)
		{
			_summary.AddResult(feature);
		}

		/// <summary>
		/// Notifies aggregator that no more features would be added.
		/// This implementation saves result summary in file.
		/// </summary>
		public void Finished()
		{
			_summary.SaveSummary(FilePath);
		}

		#endregion
	}
}