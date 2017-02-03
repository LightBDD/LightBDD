using LightBDD.Core.Results;

namespace LightBDD.Core.Reporting
{
	/// <summary>
	/// Interface for feature result summary writers.
	/// </summary>
	public interface IReportWriter
	{
		/// <summary>
		/// Saves feature <c>results</c>.
		/// </summary>
		/// <param name="results">Results to save.</param>
		void Save(params IFeatureResult[] results);
	}
}