using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace SimpleBDD.Results.Formatters
{
	/// <summary>
	/// Formats feature results as xml.
	/// </summary>
	public class XmlResultFormatter : IResultFormatter
	{
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
				ToXDocument(features).Save(stream);
				stream.Flush();
				return Encoding.Default.GetString(memory.ToArray());
			}
		}

		private XDocument ToXDocument(IEnumerable<IFeatureResult> features)
		{
			return new XDocument(
				new XDeclaration("1.0", "utf-8", null),
				new XElement("TestResults", features.Select(ToXElement).Cast<object>().ToArray()));
		}

		private XElement ToXElement(IFeatureResult feature)
		{
			return new XElement("Feature",
				new XAttribute("Name", feature.Name),
				new XElement("Description", feature.Description),
				feature.Scenarios.Select(ToXElement).Cast<object>().ToArray());
		}

		private XElement ToXElement(IScenarioResult scenario)
		{
			return new XElement("Scenario",
				new XAttribute("Status", scenario.Status.ToString()),
				new XAttribute("Name", scenario.Name),
				scenario.Steps.Select(ToXElement).Cast<object>().ToArray());
		}

		private XElement ToXElement(IStepResult step)
		{
			return new XElement("Step",
				new XAttribute("Status", step.Status.ToString()),
				new XAttribute("Number", step.Number),
				new XAttribute("Name", step.Name));
		}

		#endregion
	}
}