using System;
using LightBDD.Results;
using LightBDD.UnitTests.Helpers;

namespace LightBDD.UnitTests.Results.Formatters
{
    static class ResultFormatterTestData
    {
        private static DateTimeOffset _startDate = new DateTimeOffset(2014, 09, 23, 19, 21, 57, 55, TimeSpan.Zero);

        public static IFeatureResult GetFeatureResultWithDescription()
        {
            return Mocks.CreateFeatureResult("My feature", string.Format("My feature{0}long description", Environment.NewLine), "Label 1",
                Mocks.CreateScenarioResult("name", "Label 2", _startDate.AddSeconds(1), new TimeSpan(0, 0, 1, 2, 100),
                    Mocks.CreateStepResult(1, "step1", ResultStatus.Passed, _startDate.AddSeconds(2), new TimeSpan(0, 1, 1)),
                    Mocks.CreateStepResult(2, "step2", ResultStatus.Ignored, _startDate.AddSeconds(3), new TimeSpan(0, 0, 0, 1, 100), "Not implemented yet")),

                Mocks.CreateScenarioResult("name2", null, _startDate.AddSeconds(4), new TimeSpan(0, 0, 0, 2, 157),
                    Mocks.CreateStepResult(1, "step3", ResultStatus.Passed, _startDate.AddSeconds(5), new TimeSpan(0, 0, 0, 2, 107)),
                    Mocks.CreateStepResult(2, "step4", ResultStatus.Failed, _startDate.AddSeconds(6), new TimeSpan(0, 0, 0, 0, 50), string.Format("  Expected: True{0}  But was: False", Environment.NewLine)),
                    Mocks.CreateStepResult(3, "step5", ResultStatus.NotRun)));
        }
        public static IFeatureResult[] GetMultipleFeatureResults()
        {
            var feature1 = Mocks.CreateFeatureResult("My feature", null, null,
            Mocks.CreateScenarioResult("scenario1", null, _startDate.AddSeconds(1), TimeSpan.FromMilliseconds(20),
                Mocks.CreateStepResult(1, "step1", ResultStatus.Passed, _startDate.AddSeconds(2), TimeSpan.FromMilliseconds(20))));

            var feature2 = Mocks.CreateFeatureResult("My feature2", null, null,
                 Mocks.CreateScenarioResult("scenario1", null, _startDate.AddSeconds(4), TimeSpan.FromMilliseconds(20),
                 Mocks.CreateStepResult(1, "step1", ResultStatus.Passed, _startDate.AddSeconds(5), TimeSpan.FromMilliseconds(20))));

            return new[] { feature1, feature2 };
        }

        public static IFeatureResult GetFeatureResultWithoutDescriptionNorLabelNorDetails()
        {
            return Mocks.CreateFeatureResult("My feature", null, null,
                Mocks.CreateScenarioResult("name", null, _startDate.AddSeconds(1), TimeSpan.FromMilliseconds(25),
                    Mocks.CreateStepResult(1, "step1", ResultStatus.Passed, _startDate.AddSeconds(2), TimeSpan.FromMilliseconds(20)),
                    Mocks.CreateStepResult(2, "step2", ResultStatus.Ignored, _startDate.AddSeconds(3), TimeSpan.FromMilliseconds(5))));
        }
    }
}