using System.Collections.Generic;
using System.Linq;
using LightBDD.Results;
using NUnit.Framework;

namespace LightBDD.UnitTests.Helpers
{
    class StepResultExpectation
    {
        public StepResultExpectation(int stepNumber, string name, ResultStatus status, string details = null, params string[] comments)
        {
            StatusDetails = details;
            Comments = comments;
            Status = status;
            Name = name;
            Number = stepNumber;
        }

        public int Number { get; private set; }
        public string Name { get; private set; }
        public ResultStatus Status { get; private set; }
        public string StatusDetails { get; private set; }
        public string[] Comments { get; private set; }

        public static void Assert(IEnumerable<IStepResult> stepResults, IEnumerable<StepResultExpectation> stepResultExpectations)
        {
            string[] actual = stepResults.Select(r => string.Format("{0} {1} - {2} ({3}) // {4}", r.Number, r.Name, r.Status, r.StatusDetails, string.Join(" // ", r.Comments))).ToArray();
            string[] expected = stepResultExpectations.Select(r => string.Format("{0} {1} - {2} ({3}) // {4}", r.Number, r.Name, r.Status, r.StatusDetails, string.Join(" // ", r.Comments))).ToArray();
            NUnit.Framework.Assert.That(actual, Is.EquivalentTo(expected));
        }
    }
}