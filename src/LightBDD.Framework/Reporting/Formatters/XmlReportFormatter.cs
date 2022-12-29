using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using LightBDD.Core.Metadata;
using LightBDD.Core.Results;
using LightBDD.Core.Results.Parameters;
using LightBDD.Core.Results.Parameters.Tabular;
using LightBDD.Core.Results.Parameters.Trees;
using LightBDD.Framework.Results.Implementation;

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

            objects.AddRange(feature.GetScenariosOrderedByName().Select(ToXElement));
            objects.Add(new XAttribute("RuntimeId", feature.Info.RuntimeId.ToString("D")));
            return new XElement("Feature", objects);
        }

        private static XElement ToXElement(IScenarioResult scenario)
        {
            var objects = new List<object>
            {
                new XAttribute("Status", scenario.Status.ToString()),
                new XAttribute("Name", scenario.Info.Name),
                ToXElement(scenario.Info.Name)
            };
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
            objects.Add(new XAttribute("RuntimeId", scenario.Info.RuntimeId.ToString("D")));
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
            objects.AddRange(step.Parameters.Select(ToXElement));
            objects.AddRange(step.Comments.Select(c => new XElement("Comment", c)));
            objects.AddRange(step.FileAttachments.Select(ToXElement));
            objects.AddRange(step.GetSubSteps().Select(s => ToXElement(s, "SubStep")));
            objects.Add(new XAttribute("RuntimeId", step.Info.RuntimeId.ToString("D")));
            return new XElement(elementName, objects);
        }

        private static XElement ToXElement(FileAttachment attachment)
        {
            return new XElement("FileAttachment", new XAttribute("Name", attachment.Name), new XAttribute("Path", attachment.RelativePath));
        }

        private static XElement ToXElement(IParameterResult parameterResult)
        {
            var objects = new List<object>
            {
                new XAttribute("Name", parameterResult.Name)
            };
            var result = ToXElement(parameterResult.Details);
            if (result != null)
                objects.Add(result);
            return new XElement("Parameter", objects);
        }

        private static XElement ToXElement(IParameterDetails parameterVerification)
        {
            switch (parameterVerification)
            {
                case IInlineParameterDetails inline:
                    return ToXElement((IValueResult)inline);
                case ITabularParameterDetails tabular:
                    return ToXElement(tabular);
                case ITreeParameterDetails tree:
                    return ToXElement(tree);
            }

            return null;
        }

        private static XElement ToXElement(ITabularParameterDetails tabularDetails)
        {
            var objects = new List<object>
            {
                ToXAttribute(tabularDetails.VerificationStatus)
            };

            if (!string.IsNullOrWhiteSpace(tabularDetails.VerificationMessage))
                objects.Add(new XAttribute("Message", tabularDetails.VerificationMessage));
            objects.AddRange(tabularDetails.Columns.Select(ToXElement));
            objects.AddRange(tabularDetails.Rows.Select(ToXElement));
            return new XElement("Table", objects);
        }

        private static XElement ToXElement(ITreeParameterDetails treeDetails)
        {
            var objects = new List<object>
            {
                ToXAttribute(treeDetails.VerificationStatus)
            };

            if (!string.IsNullOrWhiteSpace(treeDetails.VerificationMessage))
                objects.Add(new XAttribute("Message", treeDetails.VerificationMessage));
            objects.AddRange(treeDetails.Root.EnumerateAll().Select(ToXElement));
            return new XElement("Tree", objects);
        }

        private static XElement ToXElement(ITreeParameterNodeResult nodeResult, int _)
        {
            var objects = new List<object>
            {
                new XAttribute("Path", nodeResult.Path),
                ToXAttribute(nodeResult.VerificationStatus)
            };
            if (nodeResult.Value != null)
                objects.Add(new XAttribute("Value", nodeResult.Value));
            if (nodeResult.Expectation != null)
                objects.Add(new XAttribute("Expectation", nodeResult.Expectation));
            if (!string.IsNullOrWhiteSpace(nodeResult.VerificationMessage))
                objects.Add(new XAttribute("Message", nodeResult.VerificationMessage));
            return new XElement("Node", objects);
        }

        private static XElement ToXElement(ITabularParameterRow row)
        {
            var objects = new List<object>
            {
                ToXAttribute(row.VerificationStatus),
                new XAttribute("Type", row.Type.ToString())
            };
            if (!string.IsNullOrWhiteSpace(row.VerificationMessage))
                objects.Add(new XAttribute("Message", row.VerificationMessage));
            objects.AddRange(row.Values.Select((v, i) => ToXElement(v, i)));
            return new XElement("Row", objects);
        }

        private static XAttribute ToXAttribute(ParameterVerificationStatus status)
        {
            return new XAttribute("Status", status.ToString());
        }

        private static XElement ToXElement(ITabularParameterColumn column, int index)
        {
            var objects = new List<object>
            {
                new XAttribute("Index", index),
                new XAttribute("Name", column.Name),
                new XAttribute("IsKey", column.IsKey)
            };
            return new XElement("Column", objects);
        }

        private static XElement ToXElement(IValueResult valueResult, int? index = null)
        {
            var objects = new List<object>();
            if (index.HasValue)
                objects.Add(new XAttribute("Index", index.Value));
            objects.Add(ToXAttribute(valueResult.VerificationStatus));
            if (valueResult.Value != null)
                objects.Add(new XAttribute("Value", valueResult.Value));
            if (valueResult.Expectation != null)
                objects.Add(new XAttribute("Expectation", valueResult.Expectation));
            if (!string.IsNullOrWhiteSpace(valueResult.VerificationMessage))
                objects.Add(new XAttribute("Message", valueResult.VerificationMessage));
            return new XElement("Value", objects);
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
            var objects = new List<object>
            {
                new XAttribute("Format", name.NameFormat),
                name.Parameters.Select(ToXElement).Cast<object>().ToArray()
            };

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