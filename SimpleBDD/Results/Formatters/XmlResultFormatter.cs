using System.IO;
using System.Xml.Serialization;

namespace SimpleBDD.Results.Formatters
{
	/// <summary>
	/// Formats story result as xml.
	/// </summary>
	public class XmlResultFormatter : IResultFormatter
	{
		#region IResultFormatter Members

		/// <summary>
		/// Formats story result.
		/// </summary>
		/// <param name="result">Result to format.</param>
		public string Format(StoryResult result)
		{
			using (var stream = new StringWriter())
			{
				new XmlSerializer(typeof(StoryResult)).Serialize(stream, result);
				return stream.GetStringBuilder().ToString();
			}
		}

		#endregion
	}
}