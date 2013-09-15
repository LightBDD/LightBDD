using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using SimpleBDD.Results.Implementation;

namespace SimpleBDD.Results.Formatters
{
	/// <summary>
	/// Formats feature results as xml.
	/// </summary>
	public class XmlResultFormatter : IResultFormatter
	{
		#region Internals
		[Serializable]
		public class TestResults
		{
			[XmlElement("Feature")]
			public FeatureResult[] Features { get; set; }
		}
		#endregion

		#region IResultFormatter Members
		/// <summary>
		/// Formats feature results.
		/// </summary>
		/// <param name="features">Features to format.</param>
		public string Format(params IFeatureResult[] features)
		{
			using (var memory = new MemoryStream())
			using (var stream = new StreamWriter(memory))
			{
				new XmlSerializer(typeof(TestResults)).Serialize(stream, new TestResults { Features = features.Cast<FeatureResult>().ToArray() });
				stream.Flush();
				return Encoding.Default.GetString(memory.ToArray());
			}
		}

		#endregion
	}
}