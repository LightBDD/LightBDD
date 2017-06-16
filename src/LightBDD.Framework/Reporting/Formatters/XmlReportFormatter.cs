using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using LightBDD.Core.Metadata;
using LightBDD.Core.Results;

namespace LightBDD.Framework.Reporting.Formatters
{
    /// <summary>
    /// Formats feature results as XML.
    /// </summary>
    public class XmlReportFormatter : IReportFormatter
    {
        #region IReportFormatter Members

        /// <summary>
        /// Formats provided feature results and writes to the <paramref name="stream"/>.
        /// </summary>
        /// <param name="stream">Stream to write formatted results to.</param>
        /// <param name="features">Feature results to format.</param>
        public void Format(Stream stream, params IFeatureResult[] features)
        {
            using (var writer = new StreamWriter(stream, new UTF8Encoding(false)))
                ToXDocument(features).Save(writer);
        }

        #endregion

        private static XDocument ToXDocument(IFeatureResult[] features)
        {
            return new XDocument(
                new XDeclaration("1.0", "utf-8", null),
                new XElement("TestResults",
                    Enumerable.Repeat(ToSummaryXElement(features), 1).Concat(features.Select(ToXElement)).Cast<object>().ToArray()));
        }

        private static XElement ToXElement(IFeatureResult feature)
        {
            var objects = new List<object> { new XAttribute("Name", feature.Info.Name) };
            objects.AddRange(feature.Info.Labels.Select(label => new XElement("Label", new XAttribute("Name", label))));

            if (!string.IsNullOrWhiteSpace(feature.Info.Description))
                objects.Add(new XElement("Description", feature.Info.Description));

            objects.AddRange(feature.GetScenarios().Select(ToXElement));

            return new XElement("Feature", objects);
        }

        private static XElement ToXElement(IScenarioResult scenario)
        {
            var objects = new List<object>
            {
                new XAttribute("Status", scenario.Status.ToString()),
                new XAttribute("Name", scenario.Info.Name)
            };
            objects.Add(ToXElement(scenario.Info.Name));
            objects.AddRange(scenario.Info.Labels.Select(label => new XElement("Label", new XAttribute("Name", label))));

            if (scenario.ExecutionTime != null)
            {
                objects.Add(new XAttribute("ExecutionStart", scenario.ExecutionTime.Start));
                objects.Add(new XAttribute("ExecutionTime", scenario.ExecutionTime.Duration));
            }
            if (scenario.Info.Categories.Any())
                objects.AddRange(scenario.Info.Categories.Select(c => new XElement("Category", new XAttribute("Name", c))));
            objects.AddRange(scenario.GetSteps().Select(s => ToXElement(s, "Step")));

            if (!string.IsNullOrWhiteSpace(scenario.StatusDetails))
                objects.Add(new XElement("StatusDetails", scenario.StatusDetails));

            return new XElement("Scenario", objects);
        }

        private static XElement ToXElement(IStepResult step, string elementName)
        {
            var objects = new List<object>
            {
                new XAttribute("Status", step.Status.ToString()),
                new XAttribute("Number", step.Info.Number),
                new XAttribute("Name", step.Info.Name)
            };
            if (step.ExecutionTime != null)
            {
                objects.Add(new XAttribute("ExecutionStart", step.ExecutionTime.Start));
                objects.Add(new XAttribute("ExecutionTime", step.ExecutionTime.Duration));
            }
            if (!string.IsNullOrEmpty(step.Info.GroupPrefix))
                objects.Add(new XAttribute("GroupPrefix", step.Info.GroupPrefix));
            if (step.StatusDetails != null)
                objects.Add(new XElement("StatusDetails", step.StatusDetails));
            objects.Add(ToXElement(step.Info.Name));
            objects.AddRange(step.Comments.Select(c => new XElement("Comment", c)));
            objects.AddRange(step.GetSubSteps().Select(s => ToXElement(s, "SubStep")));
            return new XElement(elementName, objects);
        }

        private static XElement ToXElement(IStepNameInfo stepName)
        {
            var objects = new List<object>();
            if (stepName.StepTypeName != null)
                objects.Add(new XAttribute("StepType", stepName.StepTypeName.Name));
            objects.Add(new XAttribute("Format", stepName.NameFormat));
            objects.Add(stepName.Parameters.Select(ToXElement).Cast<object>().ToArray());

            return new XElement("StepName", objects);
        }

        private static XElement ToXElement(INameInfo name)
        {
            var objects = new List<object>();
            objects.Add(new XAttribute("Format", name.NameFormat));
            objects.Add(name.Parameters.Select(ToXElement).Cast<object>().ToArray());

            return new XElement("Name", objects);
        }

        private static XElement ToXElement(INameParameterInfo p)
        {
            return new XElement("Parameter",
                new XAttribute("IsEvaluated", p.IsEvaluated),
                new XText(p.FormattedValue));
        }

        private static XElement ToSummaryXElement(IFeatureResult[] features)
        {
            var timeSummary = features.GetTestExecutionTimeSummary();
            var objects = new List<object>
            {
                new XAttribute("TestExecutionStart", timeSummary.Start),
                new XAttribute("TestExecutionEnd", timeSummary.End),
                new XAttribute("TestExecutionTime", timeSummary.Duration),
                new XElement("Features", new object[] {new XAttribute("Count", features.Length)}),
                new XElement("Scenarios", new XAttribute("Count", features.CountScenarios()),
                    new XAttribute("Passed", features.CountScenariosWithStatus(ExecutionStatus.Passed)),
                    new XAttribute("Bypassed", features.CountScenariosWithStatus(ExecutionStatus.Bypassed)),
                    new XAttribute("Failed", features.CountScenariosWithStatus(ExecutionStatus.Failed)),
                    new XAttribute("Ignored", features.CountScenariosWithStatus(ExecutionStatus.Ignored))),
                new XElement("Steps", new XAttribute("Count", features.CountSteps()),
                    new XAttribute("Passed", features.CountStepsWithStatus(ExecutionStatus.Passed)),
                    new XAttribute("Bypassed", features.CountStepsWithStatus(ExecutionStatus.Bypassed)),
                    new XAttribute("Failed", features.CountStepsWithStatus(ExecutionStatus.Failed)),
                    new XAttribute("Ignored", features.CountStepsWithStatus(ExecutionStatus.Ignored)),
                    new XAttribute("NotRun", features.CountStepsWithStatus(ExecutionStatus.NotRun)))
            };

            return new XElement("Summary", objects);
        }
    }
}