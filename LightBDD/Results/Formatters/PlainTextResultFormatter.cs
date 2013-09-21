using System.Text;
using System.Text.RegularExpressions;

namespace LightBDD.Results.Formatters
{
	/// <summary>
	/// Formats feature results as plain text.
	/// </summary>
	public class PlainTextResultFormatter : IResultFormatter
	{
		private static readonly Regex _whiteSpaceCleanup = new Regex("\\s+", RegexOptions.Compiled);

		#region IResultFormatter Members

		/// <summary>
		/// Formats feature results.
		/// </summary>
		/// <param name="features">Features to format.</param>
		public string Format(params IFeatureResult[] features)
		{
			var builder = new StringBuilder();
			foreach (var feature in features)
				FormatFeature(builder, feature);
			return builder.ToString();
		}

		#endregion

		private static void FormatDetails(StringBuilder builder, IScenarioResult scenario)
		{
			if (string.IsNullOrWhiteSpace(scenario.StatusDetails))
				builder.AppendLine();
			else
				builder.Append(" (").Append(FormatDetailsText(scenario.StatusDetails)).AppendLine(")");
		}

		private static string FormatDetailsText(string statusDetails)
		{
			return _whiteSpaceCleanup.Replace(statusDetails.Trim(), " ");
		}

		private void FormatFeature(StringBuilder builder, IFeatureResult feature)
		{
			if (builder.Length > 0)
				builder.AppendLine();

			builder.Append("Feature: ");
			if (!string.IsNullOrWhiteSpace(feature.Label))
				builder.Append("[").Append(feature.Label).Append("] ");
			builder.AppendLine(feature.Name);

			if (!string.IsNullOrWhiteSpace(feature.Description))
				builder.Append("\t").Append(feature.Description.Replace("\n", "\n\t")).AppendLine();

			foreach (var scenario in feature.Scenarios)
				FormatScenario(builder, scenario);
		}

		private void FormatScenario(StringBuilder builder, IScenarioResult scenario)
		{
			if (builder.Length > 0)
				builder.AppendLine();
			builder.Append("\tScenario: ");
			if (!string.IsNullOrWhiteSpace(scenario.Label))
				builder.Append("[").Append(scenario.Label).Append("] ");
			builder.Append(scenario.Name).Append(" - ").Append(scenario.Status);
			FormatDetails(builder, scenario);
			foreach (var step in scenario.Steps)
			{
				builder.Append("\t\tStep ")
				       .Append(step.Number).Append(": ")
				       .Append(step.Name).Append(" - ").AppendLine(step.Status.ToString());
			}
		}
	}
}