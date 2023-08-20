using System;
using System.Linq;
using LightBDD.Core.Results;
using LightBDD.Framework.Reporting;
using LightBDD.ScenarioHelpers;
using NUnit.Framework;

namespace LightBDD.Framework.UnitTests.Reporting
{
    [TestFixture]
    public class FeatureReportExtensions_tests
    {
        [Test]
        public void It_should_count_scenarios()
        {
            var features = new[]
            {
                new TestResults.TestFeatureResult
                {
                    Scenarios = new[]
                    {
                        new TestResults.TestScenarioResult(),
                        new TestResults.TestScenarioResult()
                    }
                },
                new TestResults.TestFeatureResult
                {
                    Scenarios = new[]
                    {
                        new TestResults.TestScenarioResult()
                    }
                }
            };
            Assert.That(features.CountScenarios(), Is.EqualTo(3));
        }

        [Test]
        public void It_should_count_steps()
        {
            var features = new[]
            {
                new TestResults.TestFeatureResult
                {
                    Scenarios = new[]
                    {
                        new TestResults.TestScenarioResult {Steps = new[] {new TestResults.TestStepResult()}},
                        new TestResults.TestScenarioResult {Steps = new[] {new TestResults.TestStepResult()}}
                    }
                },
                new TestResults.TestFeatureResult
                {
                    Scenarios = new[]
                    {
                        new TestResults.TestScenarioResult
                        {
                            Steps = new[]
                            {
                                new TestResults.TestStepResult(),
                                new TestResults.TestStepResult(),
                                new TestResults.TestStepResult
                                {
                                    SubSteps = new[]
                                    {
                                        new TestResults.TestStepResult
                                        {
                                            SubSteps = new[] {new TestResults.TestStepResult()}
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };
            Assert.That(features.CountSteps(), Is.EqualTo(7));
            Assert.That(features[0].CountSteps(), Is.EqualTo(2));
            Assert.That(features[1].CountSteps(), Is.EqualTo(5));
        }

        [Test]
        public void It_should_count_steps_with_status()
        {
            var features = new[]
            {
                new TestResults.TestFeatureResult
                {
                    Scenarios = new[]
                    {
                        new TestResults.TestScenarioResult
                        {
                            Steps = new[] {new TestResults.TestStepResult {Status = ExecutionStatus.Passed}}
                        },
                        new TestResults.TestScenarioResult
                        {
                            Steps = new[] {new TestResults.TestStepResult {Status = ExecutionStatus.Failed}}
                        }
                    }
                },
                new TestResults.TestFeatureResult
                {
                    Scenarios = new[]
                    {
                        new TestResults.TestScenarioResult
                        {
                            Steps = new[]
                            {
                                new TestResults.TestStepResult {Status = ExecutionStatus.Passed},
                                new TestResults.TestStepResult {Status = ExecutionStatus.Bypassed},
                                new TestResults.TestStepResult
                                {
                                    Status = ExecutionStatus.Failed,
                                    SubSteps = new[]
                                    {
                                        new TestResults.TestStepResult
                                        {
                                            SubSteps = new[] {new TestResults.TestStepResult()}
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };
            Assert.That(features.CountStepsWithStatus(ExecutionStatus.Passed), Is.EqualTo(2));
            Assert.That(features.CountStepsWithStatus(ExecutionStatus.Failed), Is.EqualTo(2));
            Assert.That(features.CountStepsWithStatus(ExecutionStatus.Bypassed), Is.EqualTo(1));
            Assert.That(features.CountStepsWithStatus(ExecutionStatus.NotRun), Is.EqualTo(2));

            Assert.That(features[0].CountStepsWithStatus(ExecutionStatus.Failed), Is.EqualTo(1));
            Assert.That(features[0].Scenarios[0].CountStepsWithStatus(ExecutionStatus.Failed), Is.EqualTo(0));
        }

        [Test]
        public void It_should_count_scenarios_with_status()
        {
            var features = new[]
            {
                new TestResults.TestFeatureResult
                {
                    Scenarios = new[]
                    {
                        new TestResults.TestScenarioResult
                        {
                            Status = ExecutionStatus.Passed
                        },
                        new TestResults.TestScenarioResult
                        {
                            Status = ExecutionStatus.Failed
                        }
                    }
                },
                new TestResults.TestFeatureResult
                {
                    Scenarios = new[]
                    {
                        new TestResults.TestScenarioResult
                        {
                            Status = ExecutionStatus.Passed
                        }
                    }
                }
            };
            Assert.That(features.CountScenariosWithStatus(ExecutionStatus.Passed), Is.EqualTo(2));
            Assert.That(features.CountScenariosWithStatus(ExecutionStatus.Failed), Is.EqualTo(1));
            Assert.That(features.CountScenariosWithStatus(ExecutionStatus.Bypassed), Is.EqualTo(0));

            Assert.That(features[0].CountScenariosWithStatus(ExecutionStatus.Passed), Is.EqualTo(1));
        }

        [Test]
        public void It_should_count_execution_times()
        {
            var baseTime = new DateTimeOffset(2017, 01, 19, 20, 54, 00, TimeSpan.Zero);
            var features = new[]
            {
                new TestResults.TestFeatureResult
                {
                    Scenarios = new[]
                    {
                        new TestResults.TestScenarioResult
                        {
                            ExecutionTime = new TestResults.TestExecutionTime {Start = baseTime, Duration = TimeSpan.FromSeconds(4)}
                        },
                        new TestResults.TestScenarioResult
                        {
                            ExecutionTime = new TestResults.TestExecutionTime{Start = baseTime.AddSeconds(1), Duration = TimeSpan.FromSeconds(2)}
                        }

                    }
                },
                new TestResults.TestFeatureResult
                {
                    Scenarios = new[]
                    {
                        new TestResults.TestScenarioResult
                        {
                            ExecutionTime = new TestResults.TestExecutionTime {Start = baseTime, Duration = TimeSpan.FromSeconds(7)}
                        },
                        new TestResults.TestScenarioResult
                        {
                            ExecutionTime = new TestResults.TestExecutionTime {Start = baseTime, Duration = TimeSpan.FromSeconds(7)}
                        }
                    }
                },
                new TestResults.TestFeatureResult
                {
                    Scenarios = Array.Empty<TestResults.TestScenarioResult>()
                }
            };
            AssertExecutionSummary(features.GetTestExecutionTimeSummary(), baseTime, baseTime.AddSeconds(7), TimeSpan.FromSeconds(7), TimeSpan.FromSeconds(20), TimeSpan.FromSeconds(5));
            AssertExecutionSummary(features[0].Scenarios.GetTestExecutionTimeSummary(), baseTime, baseTime.AddSeconds(4), TimeSpan.FromSeconds(4), TimeSpan.FromSeconds(6), TimeSpan.FromSeconds(3));
            AssertZeroExecutionSummary(features[2].Scenarios.GetTestExecutionTimeSummary());
        }

        [Test]
        public void It_should_return_all_steps_for_scenario_in_order()
        {
            var scenario = new TestResults.TestScenarioResult
            {
                Steps = new[]
                {
                    new TestResults.TestStepResult{StatusDetails = "a"},
                    new TestResults.TestStepResult{StatusDetails = "b",SubSteps = new []{new TestResults.TestStepResult{StatusDetails = "b1"} }},
                    new TestResults.TestStepResult
                    {
                        StatusDetails = "c",
                        SubSteps = new[]
                        {
                            new TestResults.TestStepResult
                            {
                                StatusDetails = "c1",
                                SubSteps = new[] {new TestResults.TestStepResult{StatusDetails = "c11"}}
                            },
                            new TestResults.TestStepResult
                            {
                                StatusDetails = "c2",
                                SubSteps = new[] {new TestResults.TestStepResult{StatusDetails = "c21"}}
                            }
                        }
                    }
                }
            };
            Assert.AreEqual(new[] { "a", "b", "b1", "c", "c1", "c11", "c2", "c21" }, scenario.GetAllSteps().Select(x => x.StatusDetails).ToArray());
        }

        [Test]
        public void It_should_return_all_steps_for_step_in_order()
        {
            var step = new TestResults.TestStepResult
            {
                StatusDetails = "root",
                SubSteps = new[]
                {
                    new TestResults.TestStepResult{StatusDetails = "a"},
                    new TestResults.TestStepResult{StatusDetails = "b",SubSteps = new []{new TestResults.TestStepResult{StatusDetails = "b1"} }},
                    new TestResults.TestStepResult
                    {
                        StatusDetails = "c",
                        SubSteps = new[]
                        {
                            new TestResults.TestStepResult
                            {
                                StatusDetails = "c1",
                                SubSteps = new[] {new TestResults.TestStepResult{StatusDetails = "c11"}}
                            },
                            new TestResults.TestStepResult
                            {
                                StatusDetails = "c2",
                                SubSteps = new[] {new TestResults.TestStepResult{StatusDetails = "c21"}}
                            }
                        }
                    }
                }
            };
            Assert.AreEqual(new[] { "root", "a", "b", "b1", "c", "c1", "c11", "c2", "c21" }, step.GetAllSteps().Select(x => x.StatusDetails).ToArray());
        }

        private static void AssertZeroExecutionSummary(ExecutionTimeSummary summary)
        {
            Assert.That(summary.Start, Is.EqualTo(summary.End), () => "start and end should be the same");
            Assert.That(summary.Start, Is.EqualTo(DateTimeOffset.UtcNow).Within(TimeSpan.FromSeconds(1)), () => "start should be DateTimeOffset.UtcNow");

            AssertDurations(summary, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero);
        }

        private void AssertExecutionSummary(ExecutionTimeSummary summary, DateTimeOffset expectedStart, DateTimeOffset expectedEnd, TimeSpan expectedDuration, TimeSpan expectedAggregated, TimeSpan expectedAverage)
        {
            Assert.That(summary.Start, Is.EqualTo(expectedStart), () => nameof(summary.Start));
            Assert.That(summary.End, Is.EqualTo(expectedEnd), () => nameof(summary.End));
            AssertDurations(summary, expectedDuration, expectedAggregated, expectedAverage);
        }

        private static void AssertDurations(ExecutionTimeSummary summary, TimeSpan expectedDuration, TimeSpan expectedAggregated, TimeSpan expectedAverage)
        {
            Assert.That(summary.Duration, Is.EqualTo(expectedDuration), () => nameof(summary.Duration));
            Assert.That(summary.Aggregated, Is.EqualTo(expectedAggregated), () => nameof(summary.Aggregated));
            Assert.That(summary.Average, Is.EqualTo(expectedAverage), () => nameof(summary.Average));
        }
    }
}
