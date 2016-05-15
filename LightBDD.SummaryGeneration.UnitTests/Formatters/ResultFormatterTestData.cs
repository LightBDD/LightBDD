using System;
using LightBDD.Core.Execution.Results;
using LightBDD.UnitTests.Helpers;

namespace LightBDD.SummaryGeneration.UnitTests.Formatters
{
    static class ResultFormatterTestData
    {
        private static DateTimeOffset _startDate = new DateTimeOffset(2014, 09, 23, 19, 21, 57, 55, TimeSpan.Zero);

        public static IFeatureResult GetFeatureResultWithDescription()
        {
            return Mocks.CreateFeatureResult("My feature", $"My feature{Environment.NewLine}long description", "Label 1",
                Mocks.CreateScenarioResult("name", "Label 2", _startDate.AddSeconds(1), new TimeSpan(0, 0, 1, 2, 100), new[] { "categoryA" },
                    Mocks.CreateStepResult(ExecutionStatus.Passed)
                        .WithExecutionTime(_startDate.AddSeconds(2), new TimeSpan(0, 1, 1))
                        .WithStepNameDetails(1, "call step1 \"arg1\"", "step1 \"{0}\"", "call", "arg1")
                        .WithComments(@"multiline
comment", "comment 2"),
                    Mocks.CreateStepResult(2, "step2", ExecutionStatus.Ignored, _startDate.AddSeconds(3), new TimeSpan(0, 0, 0, 1, 100), "Not implemented yet")),

                Mocks.CreateScenarioResult("name2", null, _startDate.AddSeconds(4), new TimeSpan(0, 0, 0, 2, 157), new[] { "categoryB", "categoryC" },
                    Mocks.CreateStepResult(1, "step3", ExecutionStatus.Bypassed, _startDate.AddSeconds(5), new TimeSpan(0, 0, 0, 2, 107), "bypass reason"),
                    Mocks.CreateStepResult(2, "step4", ExecutionStatus.Failed, _startDate.AddSeconds(6), new TimeSpan(0, 0, 0, 0, 50),
                        $"  Expected: True{Environment.NewLine}  But was: False"),
                    Mocks.CreateStepResult(3, "step5", ExecutionStatus.NotRun)));
        }
        public static IFeatureResult[] GetMultipleFeatureResults()
        {
            var feature1 = Mocks.CreateFeatureResult("My feature", null, null,
            Mocks.CreateScenarioResult("scenario1", null, _startDate.AddSeconds(1), TimeSpan.FromMilliseconds(20), new[] { "categoryA" },
                Mocks.CreateStepResult(1, "step1", ExecutionStatus.Passed, _startDate.AddSeconds(2), TimeSpan.FromMilliseconds(20))));

            var feature2 = Mocks.CreateFeatureResult("My feature2", null, null,
                 Mocks.CreateScenarioResult("scenario1", null, _startDate.AddSeconds(4), TimeSpan.FromMilliseconds(20), new[] { "categoryB" },
                 Mocks.CreateStepResult(1, "step1", ExecutionStatus.Passed, _startDate.AddSeconds(5), TimeSpan.FromMilliseconds(20))));

            return new[] { feature1, feature2 };
        }

        public static IFeatureResult GetFeatureResultWithoutDescriptionNorLabelNorDetails()
        {
            return Mocks.CreateFeatureResult("My feature", null, null,
                Mocks.CreateScenarioResult("name", null, _startDate.AddSeconds(1), TimeSpan.FromMilliseconds(25), null,
                    Mocks.CreateStepResult(1, "step1", ExecutionStatus.Passed, _startDate.AddSeconds(2), TimeSpan.FromMilliseconds(20)),
                    Mocks.CreateStepResult(2, "step2", ExecutionStatus.Ignored, _startDate.AddSeconds(3), TimeSpan.FromMilliseconds(5))));
        }
    }
}