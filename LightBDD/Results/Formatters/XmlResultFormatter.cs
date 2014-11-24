using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace LightBDD.Results.Formatters
{
    /// <summary>
    /// Formats feature results as XML.
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

        private static XDocument ToXDocument(IFeatureResult[] features)
        {
            return new XDocument(
                new XDeclaration("1.0", "utf-8", null),
                new XElement("TestResults",
                    Enumerable.Repeat(ToSummaryXElement(features), 1).Concat(features.Select(ToXElement)).Cast<object>().ToArray()));
        }

        private static XElement ToXElement(IFeatureResult feature)
        {
            var objects = new List<object> { new XAttribute("Name", feature.Name) };

            if (!string.IsNullOrWhiteSpace(feature.Label))
                objects.Add(new XAttribute("Label", feature.Label));

            if (!string.IsNullOrWhiteSpace(feature.Description))
                objects.Add(new XElement("Description", feature.Description));

            objects.Add(feature.Scenarios.Select(ToXElement).Cast<object>().ToArray());

            return new XElement("Feature", objects);
        }

        private static XElement ToXElement(IScenarioResult scenario)
        {
            var objects = new List<object>
            {
                new XAttribute("Status", scenario.Status.ToString()),
                new XAttribute("Name", scenario.Name)
            };

            if (!string.IsNullOrWhiteSpace(scenario.Label))
                objects.Add(new XAttribute("Label", scenario.Label));

            if (scenario.ExecutionStart != null)
                objects.Add(new XAttribute("ExecutionStart", scenario.ExecutionStart));
            if (scenario.ExecutionTime != null)
                objects.Add(new XAttribute("ExecutionTime", scenario.ExecutionTime));
            if (scenario.Categories.Any())
                objects.Add(scenario.Categories.Select(c => new XElement("Category", new XAttribute("Name", c))).Cast<object>().ToArray());
            objects.Add(scenario.Steps.Select(ToXElement).Cast<object>().ToArray());

            if (!string.IsNullOrWhiteSpace(scenario.StatusDetails))
                objects.Add(new XElement("StatusDetails", scenario.StatusDetails));

            return new XElement("Scenario", objects);
        }

        private static XElement ToXElement(IStepResult step)
        {
            var objects = new List<object>
            {
                new XAttribute("Status", step.Status.ToString()),
                new XAttribute("Number", step.Number),
                new XAttribute("Name", step.Name)
            };
            if (step.ExecutionStart != null)
                objects.Add(new XAttribute("ExecutionStart", step.ExecutionStart));
            if (step.ExecutionTime != null)
                objects.Add(new XAttribute("ExecutionTime", step.ExecutionTime));
            if (step.StatusDetails != null)
                objects.Add(new XElement("StatusDetails", step.StatusDetails));
            return new XElement("Step", objects);
        }

        private static XElement ToSummaryXElement(IFeatureResult[] features)
        {
            var objects = new List<object>
            {
                new XAttribute("TestExecutionStart", features.GetTestExecutionStartTime()),
                new XAttribute("TestExecutionTime", features.GetTestExecutionTime()),
                new XElement("Features", new object[] { new XAttribute("Count",features.Length) }),
                new XElement("Scenarios", new object[]
                {
                    new XAttribute("Count",features.CountScenarios()),
                    new XAttribute("Passed",features.CountScenariosWithStatus(ResultStatus.Passed)),
                    new XAttribute("Bypassed",features.CountScenariosWithStatus(ResultStatus.Bypassed)),
                    new XAttribute("Failed",features.CountScenariosWithStatus(ResultStatus.Failed)),
                    new XAttribute("Ignored",features.CountScenariosWithStatus(ResultStatus.Ignored))
                }),
                new XElement("Steps", new object[]
                {
                    new XAttribute("Count",features.CountSteps()),
                    new XAttribute("Passed",features.CountStepsWithStatus(ResultStatus.Passed)),
                    new XAttribute("Bypassed",features.CountStepsWithStatus(ResultStatus.Bypassed)),
                    new XAttribute("Failed",features.CountStepsWithStatus(ResultStatus.Failed)),
                    new XAttribute("Ignored",features.CountStepsWithStatus(ResultStatus.Ignored)),
                    new XAttribute("NotRun",features.CountStepsWithStatus(ResultStatus.NotRun))
                })
            };

            return new XElement("Summary", objects);
        }
    }
}