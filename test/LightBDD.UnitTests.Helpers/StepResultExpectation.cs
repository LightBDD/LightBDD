using System.Collections.Generic;
using System.Linq;
using LightBDD.Core.Results;
using NUnit.Framework;

namespace LightBDD.UnitTests.Helpers
{
    public class StepResultExpectation
    {
        public StepResultExpectation(int stepNumber, int totalSteps, string name, ExecutionStatus status, string details = null, params string[] comments)
            : this(string.Empty, stepNumber, totalSteps, name, status, details, comments) { }

        public StepResultExpectation(string groupPrefix, int stepNumber, int totalSteps, string name, ExecutionStatus status, string details = null, params string[] comments)
        {
            GroupPrefix = groupPrefix;
            StatusDetails = details;
            Comments = comments;
            Status = status;
            Name = name;
            Number = stepNumber;
            TotalSteps = totalSteps;
        }

        public int Number { get; }
        public int TotalSteps { get; set; }
        public string Name { get; }
        public ExecutionStatus Status { get; }
        public string GroupPrefix { get; }
        public string StatusDetails { get; }
        public string[] Comments { get; }

        public static void AssertEqual(IEnumerable<IStepResult> stepResults, params StepResultExpectation[] stepResultExpectations)
        {
            var actual = stepResults.Select(r => $"{r.Info.GroupPrefix}{r.Info.Number}:{r.Info.GroupPrefix}{r.Info.Total} {r.Info.Name} - {r.Status} ({r.StatusDetails}) // {string.Join(" // ", r.Comments)}").ToArray();
            var expected = stepResultExpectations.Select(r => $"{r.GroupPrefix}{r.Number}:{r.GroupPrefix}{r.TotalSteps} {r.Name} - {r.Status} ({r.StatusDetails}) // {string.Join(" // ", r.Comments)}").ToArray();

            Assert.True(actual.SequenceEqual(expected),
                $"Expected:\r\n{string.Join("\r\n", expected)}\r\n\r\nGot:\r\n{string.Join("\r\n", actual)}");
        }
    }
}