using ServiceStack.Text;

namespace SimpleBDD.Results.Formatters
{
	/// <summary>
	/// Formats story result as xml.
	/// </summary>
	public class XmlResultFormatter : IResultFormatter
	{
		/// <summary>
		/// Formats story result.
		/// </summary>
		/// <param name="result">Result to format.</param>
		public string Format(StoryResult result)
		{
			return XmlSerializer.SerializeToString(result);
		}
	}
}