using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SimpleBDD.Results;
using SimpleBDD.Results.Formatters;

namespace SimpleBDD
{
	/// <summary>
	/// Tests results summary class for collecting feature results and saving it to specified file.
	/// </summary>
	public class TestResultsSummary
	{
		private readonly IResultFormatter _formatter;
		private readonly IList<IFeatureResult> _results = new List<IFeatureResult>();

		/// <summary>
		/// Returns all collected results.
		/// </summary>
		public IEnumerable<IFeatureResult> Results { get { return _results; } }

		/// <summary>
		/// Default constructor. Uses XmlResultFormatter.
		/// </summary>
		public TestResultsSummary()
			: this(new XmlResultFormatter())
		{
		}

		/// <summary>
		/// Constructor allowing to define formatter type.
		/// </summary>
		/// <param name="formatter">Results formatter.</param>
		public TestResultsSummary(IResultFormatter formatter)
		{
			_formatter = formatter;
		}

		/// <summary>
		/// Adds feature result to summary.
		/// </summary>
		/// <param name="result">Feature result to add.</param>
		public void AddResult(IFeatureResult result)
		{
			_results.Add(result);
		}

		/// <summary>
		/// Saves test results summary to specified file, using formatter defined in constructor.
		/// </summary>
		/// <param name="filePath">Output file path.</param>
		public void SaveSummary(string filePath)
		{
			File.WriteAllText(filePath, _formatter.Format(_results.ToArray()), Encoding.UTF8);
		}
	}
}