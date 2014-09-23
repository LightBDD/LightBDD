using System;
using System.Text;
using LightBDD.Formatters;

namespace LightBDD.Results.Formatters
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

        private static void FormatDetails(StringBuilder builder, IScenarioResult scenario)
        {
            if (string.IsNullOrWhiteSpace(scenario.StatusDetails))
                return;

            builder.AppendLine();
            builder.Append("\t\tDetails: ");
            builder.Append(scenario.StatusDetails.Trim().Replace(Environment.NewLine, Environment.NewLine + "\t\t\t"));
            builder.AppendLine();
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
                builder.Append("\t").Append(feature.Description.Replace(Environment.NewLine, Environment.NewLine + "\t")).AppendLine();

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
            if (scenario.ExecutionTime != null)
                builder.Append(" (").Append(scenario.ExecutionTime.FormatPretty()).Append(")");
            builder.AppendLine();
            foreach (var step in scenario.Steps)
            {
                builder.Append("\t\tStep ")
                       .Append(step.Number).Append(": ")
                       .Append(step.Name).Append(" - ").Append(step.Status.ToString());
                if (step.ExecutionTime != null)
                    builder.Append(" (").Append(step.ExecutionTime.FormatPretty()).Append(")");
                builder.AppendLine();
            }
            FormatDetails(builder, scenario);
        }
    }
}