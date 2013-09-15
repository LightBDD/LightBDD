using System;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

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
		public string Format(params FeatureResult[] features)
		{
			using (var writer = new StringWriter())
			{
				new XmlSerializer(typeof(TestResults)).Serialize(writer, new TestResults { Features = features.ToArray() });
				return writer.GetStringBuilder().ToString();
			}
		}

		#endregion
	}
}