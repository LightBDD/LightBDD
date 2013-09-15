namespace SimpleBDD.Results.Formatters
{
	/// <summary>
	/// Result formatter interface.
	/// </summary>
	public interface IResultFormatter
	{
		/// <summary>
		/// Formats feature results.
		/// </summary>
		/// <param name="features">Features to format.</param>
		string Format(params FeatureResult[] features);
	}
}