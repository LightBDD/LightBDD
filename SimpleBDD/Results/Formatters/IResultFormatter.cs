namespace SimpleBDD.Results.Formatters
{
	/// <summary>
	/// Result formatter interface.
	/// </summary>
	public interface IResultFormatter
	{
		/// <summary>
		/// Formats story result.
		/// </summary>
		/// <param name="result">Result to format.</param>
		string Format(StoryResult result);
	}
}