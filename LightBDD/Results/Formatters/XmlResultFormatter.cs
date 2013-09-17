using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace LightBDD.Results.Formatters
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

		#endregion

		private XDocument ToXDocument(IEnumerable<IFeatureResult> features)
		{
			return new XDocument(
				new XDeclaration("1.0", "utf-8", null),
				new XElement("TestResults", features.Select(ToXElement).Cast<object>().ToArray()));
		}

		private XElement ToXElement(IFeatureResult feature)
		{
			var objects = new List<object> { new XAttribute("Name", feature.Name) };

			if (!string.IsNullOrWhiteSpace(feature.Label))
				objects.Add(new XAttribute("Label", feature.Label));

			if (!string.IsNullOrWhiteSpace(feature.Description))
				objects.Add(new XElement("Description", feature.Description));

			objects.Add(feature.Scenarios.Select(ToXElement).Cast<object>().ToArray());

			return new XElement("Feature", objects);
		}

		private XElement ToXElement(IScenarioResult scenario)
		{
			var objects = new List<object>
			{
				new XAttribute("Status", scenario.Status.ToString()),
				new XAttribute("Name", scenario.Name)
			};

			if (!string.IsNullOrWhiteSpace(scenario.Label))
				objects.Add(new XAttribute("Label", scenario.Label));

			objects.Add(scenario.Steps.Select(ToXElement).Cast<object>().ToArray());

			return new XElement("Scenario", objects);
		}

		private XElement ToXElement(IStepResult step)
		{
			return new XElement("Step",
				new XAttribute("Status", step.Status.ToString()),
				new XAttribute("Number", step.Number),
				new XAttribute("Name", step.Name));
		}
	}
}