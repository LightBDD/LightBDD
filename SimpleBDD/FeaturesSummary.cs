using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SimpleBDD.Results;
using SimpleBDD.Results.Formatters;

namespace SimpleBDD
{
	/// <summary>
	/// Tests summary class for collecting feature results and saving it to specified file.
	/// </summary>
	public class FeaturesSummary
	{
		private readonly IResultFormatter _formatter;
		private readonly IList<IFeatureResult> _features = new List<IFeatureResult>();

		/// <summary>
		/// Default constructor. Uses XmlResultFormatter.
		/// </summary>
		public FeaturesSummary()
			: this(new XmlResultFormatter())
		{
		}

		/// <summary>
		/// Constructor allowing to define formatter type.
		/// </summary>
		/// <param name="formatter">Results formatter.</param>
		public FeaturesSummary(IResultFormatter formatter)
		{
			_formatter = formatter;
		}

		/// <summary>
		/// Adds feature result to summary.
		/// </summary>
		/// <param name="result">Feature result to add.</param>
		public void AddResults(IFeatureResult result)
		{
			_features.Add(result);
		}

		/// <summary>
		/// Returns all collected features.
		/// </summary>
		public IEnumerable<IFeatureResult> Features { get { return _features; } }
		/// <summary>
		/// Saves feature summary to specified file, using formatter defined in constructor.
		/// </summary>
		/// <param name="filePath">Output file path.</param>
		public void SaveSummary(string filePath)
		{
			File.WriteAllText(filePath, _formatter.Format(_features.ToArray()), Encoding.UTF8);
		}
	}
}