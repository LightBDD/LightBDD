using System.Text;

namespace SimpleBDD.Results.Formatters
{
	/// <summary>
	/// Formats feature results as plain text.
	/// </summary>
	public class PlainTextResultFormatter : IResultFormatter
	{
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

		private void FormatFeature(StringBuilder builder, IFeatureResult feature)
		{
			if (builder.Length > 0)
				builder.AppendLine();

			builder.Append("Feature: ").AppendLine(feature.Name);

			if (!string.IsNullOrWhiteSpace(feature.Description))
				builder.Append("\t").Append(feature.Description.Replace("\n", "\n\t")).AppendLine();

			foreach (var scenario in feature.Scenarios)
				FormatScenario(builder, scenario);
		}

		private void FormatScenario(StringBuilder builder, IScenarioResult scenario)
		{
			if (builder.Length > 0)
				builder.AppendLine();
			builder.Append("\tScenario: ").Append(scenario.Name).Append(" - ").AppendLine(scenario.Status.ToString());
			foreach (var step in scenario.Steps)
			{
				builder.Append("\t\tStep ")
					   .Append(step.Number).Append(": ")
					   .Append(step.Name).Append(" - ").AppendLine(step.Status.ToString());
			}
		}
	}
}