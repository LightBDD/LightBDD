using System.Collections.Generic;
using System.Linq;
using LightBDD.Core.Execution.Results;
using Xunit;

namespace LightBDD.Core.UnitTests.Helpers
{
    class StepResultExpectation
    {
        public StepResultExpectation(int stepNumber, string name, ExecutionStatus status, string details = null, params string[] comments)
        {
            StatusDetails = details;
            Comments = comments;
            Status = status;
            Name = name;
            Number = stepNumber;
        }

        public int Number { get; private set; }
        public string Name { get; private set; }
        public ExecutionStatus Status { get; private set; }
        public string StatusDetails { get; private set; }
        public string[] Comments { get; private set; }

        public static void AssertEqual(IEnumerable<IStepResult> stepResults, params StepResultExpectation[] stepResultExpectations)
        {
            string[] actual = stepResults.Select(r => string.Format("{0} {1} - {2} ({3}) // {4}", r.Number, r.Info.Name, r.Status, r.StatusDetails, string.Join(" // ", new string[0]))).ToArray();
            string[] expected = stepResultExpectations.Select(r => string.Format("{0} {1} - {2} ({3}) // {4}", r.Number, r.Name, r.Status, r.StatusDetails, string.Join(" // ", r.Comments))).ToArray();

            Assert.True(actual.SequenceEqual(expected),
                string.Format("Expected:\n{0}\n\nGot:\n{1}", string.Join("\n", expected), string.Join("\n", actual)));
        }
    }
}