using LightBDD.Core.Results;

namespace LightBDD.Reporting
{
	/// <summary>
	/// Interface for feature result summary writers.
	/// </summary>
	public interface ISummaryWriter
	{
		/// <summary>
		/// Saves feature <c>results</c>.
		/// </summary>
		/// <param name="results">Results to save.</param>
		void Save(params IFeatureResult[] results);
	}
}