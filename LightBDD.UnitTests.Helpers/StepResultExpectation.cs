using System.Collections.Generic;
using System.Linq;
using LightBDD.Core.Results;
using NUnit.Framework;

namespace LightBDD.UnitTests.Helpers
{
    public class StepResultExpectation
    {
        public StepResultExpectation(int stepNumber, int totalSteps, string name, ExecutionStatus status, string details = null, params string[] comments)
        {
            StatusDetails = details;
            Comments = comments;
            Status = status;
            Name = name;
            Number = stepNumber;
            TotalSteps = totalSteps;
        }

        public int Number { get; private set; }
        public int TotalSteps { get; set; }
        public string Name { get; private set; }
        public ExecutionStatus Status { get; private set; }
        public string StatusDetails { get; private set; }
        public string[] Comments { get; private set; }

        public static void AssertEqual(IEnumerable<IStepResult> stepResults, params StepResultExpectation[] stepResultExpectations)
        {
            string[] actual = stepResults.Select(r =>$"{r.Info.Number}:{r.Info.Total} {r.Info.Name} - {r.Status} ({r.StatusDetails}) // {string.Join(" // ", new string[0])}").ToArray();
            string[] expected = stepResultExpectations.Select(r =>$"{r.Number}:{r.TotalSteps} {r.Name} - {r.Status} ({r.StatusDetails}) // {string.Join(" // ", r.Comments)}").ToArray();

            Assert.True(actual.SequenceEqual(expected),
                $"Expected:\r\n{string.Join("\r\n", expected)}\r\n\r\nGot:\r\n{string.Join("\r\n", actual)}");
        }
    }
}