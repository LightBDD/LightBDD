using System;
using LightBDD.Results;
using LightBDD.Results.Implementation;

namespace LightBDD.UnitTests.Results.Formatters
{
    static class ResultFormatterTestData
    {
        private static DateTimeOffset _startDate = new DateTimeOffset(2014, 09, 23, 19, 21, 57, 55, TimeSpan.Zero);

        public static FeatureResult GetFeatureResultWithDescription()
        {
            var result = new FeatureResult("My feature", string.Format("My feature{0}long description", Environment.NewLine), "Label 1");
            result.AddScenario(new ScenarioResult("name", new[]
            {
                new StepResult(1, "step1", ResultStatus.Passed).SetExecutionTime(new TimeSpan(0, 1, 1)).SetExecutionStart(_startDate.AddSeconds(2)), 
                new StepResult(2, "step2", ResultStatus.Ignored, "Not implemented yet").SetExecutionTime(new TimeSpan(0, 0, 0, 1, 100)).SetExecutionStart(_startDate.AddSeconds(3))
            }, "Label 2").SetExecutionTime(new TimeSpan(0, 0, 1, 2, 100)).SetExecutionStart(_startDate.AddSeconds(1)));
            result.AddScenario(new ScenarioResult("name2", new[]
            {
                new StepResult(1, "step3", ResultStatus.Passed).SetExecutionTime(new TimeSpan(0, 0, 0, 2, 107)).SetExecutionStart(_startDate.AddSeconds(5)), 
                new StepResult(2, "step4", ResultStatus.Failed, string.Format("  Expected: True{0}  But was: False", Environment.NewLine)).SetExecutionTime(new TimeSpan(0, 0, 0, 0, 50)).SetExecutionStart(_startDate.AddSeconds(6)),
                new StepResult(3, "step5", ResultStatus.NotRun)
            }, null).SetExecutionTime(new TimeSpan(0, 0, 0, 2, 157)).SetExecutionStart(_startDate.AddSeconds(4)));
            return result;
        }
        public static IFeatureResult[] GetMultipleFeatureResults()
        {
            var feature1 = new FeatureResult("My feature", null, null);
            feature1.AddScenario(new ScenarioResult("scenario1", new[]
            {
                new StepResult(1, "step1", ResultStatus.Passed).SetExecutionTime(TimeSpan.FromMilliseconds(20)).SetExecutionStart(_startDate.AddSeconds(2))
            }, null).SetExecutionTime(TimeSpan.FromMilliseconds(20)).SetExecutionStart(_startDate.AddSeconds(1)));

            var feature2 = new FeatureResult("My feature2", null, null);
            feature2.AddScenario(new ScenarioResult("scenario1", new[]
            {
                new StepResult(1, "step1", ResultStatus.Passed).SetExecutionTime(TimeSpan.FromMilliseconds(20)).SetExecutionStart(_startDate.AddSeconds(5))
            }, null).SetExecutionTime(TimeSpan.FromMilliseconds(20)).SetExecutionStart(_startDate.AddSeconds(4)));
            return new IFeatureResult[] { feature1, feature2 };
        }

        public static FeatureResult GetFeatureResultWithoutDescriptionNorLabelNorDetails()
        {
            var result = new FeatureResult("My feature", null, null);
            result.AddScenario(new ScenarioResult("name", new[]
            {
                new StepResult(1, "step1", ResultStatus.Passed).SetExecutionTime(TimeSpan.FromMilliseconds(20)).SetExecutionStart(_startDate.AddSeconds(2)),
                new StepResult(2, "step2", ResultStatus.Ignored).SetExecutionTime(TimeSpan.FromMilliseconds(5)).SetExecutionStart(_startDate.AddSeconds(3))
            }, null).SetExecutionTime(TimeSpan.FromMilliseconds(25)).SetExecutionStart(_startDate.AddSeconds(1)));
            return result;
        }
    }
}