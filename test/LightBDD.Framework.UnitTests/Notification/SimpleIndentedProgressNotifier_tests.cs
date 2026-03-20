using System;
using System.Collections.Generic;
using System.Linq;
using LightBDD.Core.Execution;
using LightBDD.Core.Formatting;
using LightBDD.Core.Metadata;
using LightBDD.Core.Notification.Events;
using LightBDD.Core.Results;
using LightBDD.Core.Results.Parameters;
using LightBDD.Core.Results.Parameters.Tabular;
using LightBDD.Framework.Notification;
using LightBDD.Framework.Parameters;
using LightBDD.UnitTests.Helpers;
using NUnit.Framework;

namespace LightBDD.Framework.UnitTests.Notification
{
    [TestFixture]
    public class SimpleIndentedProgressNotifier_tests
    {
        private Queue<string> _captured;
        private SimpleIndentedProgressNotifier _notifier;

        private void Notify(string message)
        {
            _captured.Enqueue(message);
        }

        [SetUp]
        public void SetUp()
        {
            _captured = new Queue<string>();
            _notifier = new SimpleIndentedProgressNotifier(Notify);
        }

        #region Feature notifications

        [Test]
        public void NotifyFeatureStart_should_include_labels_and_description()
        {
            var featureInfo = new TestResults.TestFeatureInfo
            {
                Name = TestResults.CreateNameInfo("Shopping cart"),
                Labels = new[] { "Story-1" },
                Description = "As a customer I want to manage my cart"
            };
            _notifier.Notify(new FeatureStarting(new EventTime(), featureInfo));

            var expected = NormalizeNewlines("""
                FEATURE:  [Story-1] Shopping cart
                          As a customer I want to manage my cart
                """);
            Assert.That(_captured.ToArray(), Is.EqualTo(new[] { expected, "" }));
        }

        [Test]
        public void NotifyFeatureStart_should_omit_description_if_not_provided()
        {
            var featureInfo = new TestResults.TestFeatureInfo
            {
                Name = TestResults.CreateNameInfo("Shopping cart"),
                Labels = new[] { "Story-1" },
                Description = null
            };
            _notifier.Notify(new FeatureStarting(new EventTime(), featureInfo));

            Assert.That(_captured.ToArray(), Is.EqualTo(new[] { "FEATURE:  [Story-1] Shopping cart", "" }));
        }

        [Test]
        public void NotifyFeatureStart_should_omit_labels_if_not_provided()
        {
            var featureInfo = new TestResults.TestFeatureInfo
            {
                Name = TestResults.CreateNameInfo("Shopping cart"),
                Labels = Array.Empty<string>(),
                Description = "As a customer I want to manage my cart"
            };
            _notifier.Notify(new FeatureStarting(new EventTime(), featureInfo));

            var expected = NormalizeNewlines("""
                FEATURE:  Shopping cart
                          As a customer I want to manage my cart
                """);
            Assert.That(_captured.ToArray(), Is.EqualTo(new[] { expected, "" }));
        }

        [Test]
        public void NotifyFeatureFinished_should_include_feature_name()
        {
            var featureResult = new TestResults.TestFeatureResult
            {
                Info = new TestResults.TestFeatureInfo
                {
                    Name = TestResults.CreateNameInfo("Shopping cart"),
                    Labels = Array.Empty<string>()
                }
            };
            _notifier.Notify(new FeatureFinished(new EventTime(), featureResult));

            Assert.That(_captured.Single(), Is.EqualTo("FEATURE FINISHED: Shopping cart"));
        }

        #endregion

        #region Scenario notifications

        [Test]
        public void NotifyScenarioStart_should_include_labels_and_description()
        {
            var scenarioInfo = new TestResults.TestScenarioInfo
            {
                Name = TestResults.CreateNameInfo("Adding items"),
                Labels = new[] { "Ticket-42" },
                Categories = Array.Empty<string>(),
                Description = "Verifies item addition"
            };
            _notifier.Notify(new ScenarioStarting(new EventTime(), scenarioInfo));

            var expected = NormalizeNewlines("""
                SCENARIO: [Ticket-42] Adding items
                          Verifies item addition
                """);
            Assert.That(_captured.ToArray(), Is.EqualTo(new[] { expected, "" }));
        }

        [Test]
        public void NotifyScenarioStart_should_omit_description_if_not_provided()
        {
            var scenarioInfo = new TestResults.TestScenarioInfo
            {
                Name = TestResults.CreateNameInfo("Adding items"),
                Labels = new[] { "Ticket-42" },
                Categories = Array.Empty<string>(),
                Description = null
            };
            _notifier.Notify(new ScenarioStarting(new EventTime(), scenarioInfo));

            Assert.That(_captured.ToArray(), Is.EqualTo(new[] { "SCENARIO: [Ticket-42] Adding items", "" }));
        }

        [Test]
        public void NotifyScenarioStart_should_omit_labels_if_not_provided()
        {
            var scenarioInfo = new TestResults.TestScenarioInfo
            {
                Name = TestResults.CreateNameInfo("Adding items"),
                Labels = Array.Empty<string>(),
                Categories = Array.Empty<string>(),
                Description = "Verifies item addition"
            };
            _notifier.Notify(new ScenarioStarting(new EventTime(), scenarioInfo));

            var expected = NormalizeNewlines("""
                SCENARIO: Adding items
                          Verifies item addition
                """);
            Assert.That(_captured.ToArray(), Is.EqualTo(new[] { expected, "" }));
        }

        [Test]
        public void NotifyScenarioFinished_should_include_status_and_execution_time()
        {
            var scenarioResult = new TestResults.TestScenarioResult
            {
                Info = new TestResults.TestScenarioInfo
                {
                    Name = TestResults.CreateNameInfo("Adding items"),
                    Labels = Array.Empty<string>(),
                    Categories = Array.Empty<string>()
                },
                Status = ExecutionStatus.Passed,
                StatusDetails = "Step 1: some detail",
                ExecutionTime = new TestResults.TestExecutionTime
                {
                    Start = DateTimeOffset.UtcNow,
                    Duration = TimeSpan.FromMilliseconds(250)
                }
            };
            _notifier.Notify(new ScenarioFinished(new EventTime(), scenarioResult));

            var expected = NormalizeNewlines("""
                SCENARIO RESULT: Passed after 250ms
                    Step 1: some detail
                """);
            Assert.That(_captured.ToArray(), Is.EqualTo(new[] { "", expected, "" }));
        }

        [Test]
        public void NotifyScenarioFinished_should_omit_execution_time_if_not_provided()
        {
            var scenarioResult = new TestResults.TestScenarioResult
            {
                Info = new TestResults.TestScenarioInfo
                {
                    Name = TestResults.CreateNameInfo("Adding items"),
                    Labels = Array.Empty<string>(),
                    Categories = Array.Empty<string>()
                },
                Status = ExecutionStatus.Failed,
                StatusDetails = "Step 1: some failure",
                ExecutionTime = null
            };

            _notifier.Notify(new ScenarioFinished(new EventTime(), scenarioResult));

            var expected = NormalizeNewlines("""
                SCENARIO RESULT: Failed
                    Step 1: some failure
                """);
            Assert.That(_captured.ToArray(), Is.EqualTo(new[] { "", expected, "" }));
        }

        [Test]
        public void NotifyScenarioFinished_should_omit_status_details_if_not_provided()
        {
            var scenarioResult = new TestResults.TestScenarioResult
            {
                Info = new TestResults.TestScenarioInfo
                {
                    Name = TestResults.CreateNameInfo("Adding items"),
                    Labels = Array.Empty<string>(),
                    Categories = Array.Empty<string>()
                },
                Status = ExecutionStatus.Passed,
                StatusDetails = null,
                ExecutionTime = new TestResults.TestExecutionTime
                {
                    Start = DateTimeOffset.UtcNow,
                    Duration = TimeSpan.FromMilliseconds(250)
                }
            };

            _notifier.Notify(new ScenarioFinished(new EventTime(), scenarioResult));

            Assert.That(_captured.ToArray(), Is.EqualTo(new[] { "", "SCENARIO RESULT: Passed after 250ms", "" }));
        }

        #endregion

        #region Step start notifications

        [Test]
        public void NotifyStepStart_should_use_suffix_numbering_by_default()
        {
            var stepInfo = CreateStepInfo("Given some condition", 1, 3, "");
            _notifier.Notify(new StepStarting(new EventTime(), stepInfo));

            Assert.That(_captured.Single(), Is.EqualTo("    Given some condition (STEP 1)"));
        }

        [Test]
        public void NotifyStepStart_with_prefix_numbering()
        {
            _notifier.StepWordAndStepNumberOnStart = StepWordAndStepNumberBehaviour.IncludeAsPrefix;
            var stepInfo = CreateStepInfo("Given some condition", 1, 3, "");
            _notifier.Notify(new StepStarting(new EventTime(), stepInfo));

            Assert.That(_captured.Single(), Is.EqualTo("    STEP 1: Given some condition"));
        }

        [Test]
        public void NotifyStepStart_with_excluded_numbering()
        {
            _notifier.StepWordAndStepNumberOnStart = StepWordAndStepNumberBehaviour.Exclude;
            var stepInfo = CreateStepInfo("Given some condition", 1, 3, "");
            _notifier.Notify(new StepStarting(new EventTime(), stepInfo));

            Assert.That(_captured.Single(), Is.EqualTo("    Given some condition"));
        }

        [Test]
        public void NotifyStepStart_should_include_ellipsis_when_configured()
        {
            _notifier.IncludeEllipsisAfterStep = true;
            var stepInfo = CreateStepInfo("Given some condition", 1, 3, "");
            _notifier.Notify(new StepStarting(new EventTime(), stepInfo));

            Assert.That(_captured.Single(), Is.EqualTo("    Given some condition... (STEP 1)"));
        }

        [Test]
        public void NotifyStepStart_should_show_final_step_count_when_configured()
        {
            _notifier.ShowFinalStepWithEachStep = true;
            var stepInfo = CreateStepInfo("Given some condition", 2, 5, "");
            _notifier.Notify(new StepStarting(new EventTime(), stepInfo));

            Assert.That(_captured.Single(), Is.EqualTo("    Given some condition (STEP 2/5)"));
        }

        [Test]
        public void NotifyStepStart_should_include_group_prefix()
        {
            var stepInfo = CreateStepInfo("Given some condition", 2, 5, "1.");
            _notifier.Notify(new StepStarting(new EventTime(), stepInfo));

            Assert.That(_captured.Single(), Is.EqualTo("    Given some condition (STEP 1.2)"));
        }

        [Test]
        public void NotifyStepStart_should_show_final_step_with_group_prefix_when_configured()
        {
            _notifier.ShowFinalStepWithEachStep = true;
            var stepInfo = CreateStepInfo("Given some condition", 2, 5, "1.");
            _notifier.Notify(new StepStarting(new EventTime(), stepInfo));

            Assert.That(_captured.Single(), Is.EqualTo("    Given some condition (STEP 1.2/1.5)"));
        }

        #endregion

        #region Step finish notifications

        [Test]
        public void NotifyStepFinished_should_not_emit_for_passed_steps_by_default()
        {
            var stepResult = CreateStepResult("Given some condition", 1, 3, "", ExecutionStatus.Passed);
            _notifier.Notify(new StepFinished(new EventTime(), stepResult));

            Assert.That(_captured, Is.Empty);
        }

        [Test]
        public void NotifyStepFinished_should_emit_for_passed_steps_when_WriteSuccessMessageForBasicSteps_is_true()
        {
            _notifier.WriteSuccessMessageForBasicSteps = true;
            var stepResult = CreateStepResult("Given some condition", 1, 3, "", ExecutionStatus.Passed);
            _notifier.Notify(new StepFinished(new EventTime(), stepResult));

            Assert.That(_captured.Single(), Is.EqualTo("      => (Passed after 125ms) (STEP 1)"));
        }

        [Test]
        public void NotifyStepFinished_should_always_emit_for_failed_steps()
        {
            var stepResult = CreateStepResult("Given some condition", 1, 3, "", ExecutionStatus.Failed);
            _notifier.Notify(new StepFinished(new EventTime(), stepResult));

            Assert.That(_captured.Single(), Is.EqualTo("      => (Failed after 125ms) (STEP 1)"));
        }

        [Test]
        public void NotifyStepFinished_should_always_emit_for_ignored_steps()
        {
            var stepResult = CreateStepResult("Given some condition", 1, 3, "", ExecutionStatus.Ignored);
            _notifier.Notify(new StepFinished(new EventTime(), stepResult));

            Assert.That(_captured.Single(), Is.EqualTo("      => (Ignored after 125ms) (STEP 1)"));
        }

        [Test]
        public void NotifyStepFinished_should_always_emit_for_bypassed_steps()
        {
            var stepResult = CreateStepResult("Given some condition", 1, 3, "", ExecutionStatus.Bypassed);
            _notifier.Notify(new StepFinished(new EventTime(), stepResult));

            Assert.That(_captured.Single(), Is.EqualTo("      => (Bypassed after 125ms) (STEP 1)"));
        }

        [Test]
        public void NotifyStepFinished_with_prefix_numbering()
        {
            _notifier.StepWordAndStepNumberOnFinish = StepWordAndStepNumberBehaviour.IncludeAsPrefix;
            var stepResult = CreateStepResult("Given some condition", 1, 3, "", ExecutionStatus.Failed);
            _notifier.Notify(new StepFinished(new EventTime(), stepResult));

            Assert.That(_captured.Single(), Is.EqualTo("    STEP 1: (Failed after 125ms)"));
        }

        [Test]
        public void NotifyStepFinished_with_excluded_numbering()
        {
            _notifier.StepWordAndStepNumberOnFinish = StepWordAndStepNumberBehaviour.Exclude;
            var stepResult = CreateStepResult("Given some condition", 1, 3, "", ExecutionStatus.Failed);
            _notifier.Notify(new StepFinished(new EventTime(), stepResult));

            Assert.That(_captured.Single(), Is.EqualTo("      => (Failed after 125ms)"));
        }

        [Test]
        public void NotifyStepFinished_should_include_step_name_when_configured()
        {
            _notifier.IncludeStepNameOnFinish = true;
            var stepResult = CreateStepResult("Given some condition", 1, 3, "", ExecutionStatus.Failed);
            _notifier.Notify(new StepFinished(new EventTime(), stepResult));

            Assert.That(_captured.Single(), Is.EqualTo("    Given some condition  => (Failed after 125ms) (STEP 1)"));
        }

        [Test]
        public void NotifyStepFinished_should_include_step_name_with_prefix_numbering()
        {
            _notifier.IncludeStepNameOnFinish = true;
            _notifier.StepWordAndStepNumberOnFinish = StepWordAndStepNumberBehaviour.IncludeAsPrefix;
            var stepResult = CreateStepResult("Given some condition", 1, 3, "", ExecutionStatus.Failed);
            _notifier.Notify(new StepFinished(new EventTime(), stepResult));

            Assert.That(_captured.Single(), Is.EqualTo("    STEP 1: Given some condition (Failed after 125ms)"));
        }

        [Test]
        public void NotifyStepFinished_should_report_tabular_and_tree_parameters()
        {
            _notifier.WriteSuccessMessageForBasicSteps = true;
            var stepResult = CreateStepResultWithParameters();
            _notifier.Notify(new StepFinished(new EventTime(), stepResult));

            var expectedTable = NormalizeNewlines("""
                    table:
                    +-+---+----------+----------+
                    |#|Key|Value1    |Value2    |
                    +-+---+----------+----------+
                    |=|1  |abc       |some value|
                    |!|2  |def       |val/value |
                    |-|3  |<null>/XXX|<null>/YYY|
                    |+|4  |XXX/<null>|YYY/<null>|
                    +-+---+----------+----------+
            """);

            var expectedTree = NormalizeNewlines("""
                    tree:
                    = $: <object>
                    = $.Items: <array:1>
                    ! $.Items[0]: False/True
                    = $.Name: Bob
                    ! $.Surname: Johnson/<none>
                    ! $.LastName: <none>/Johnson
            """);

            var fullMessage = _captured.Single();

            Assert.That(fullMessage, Does.StartWith("      => (Failed after 125ms) (STEP 1)"));
            Assert.That(fullMessage, Does.Contain(expectedTable));
            Assert.That(fullMessage, Does.Contain(expectedTree));
        }

        [Test]
        public void NotifyStepFinished_should_report_parameters_even_when_passed_and_WriteSuccessMessageForBasicSteps_is_false()
        {
            _notifier.WriteSuccessMessageForBasicSteps = false;
            var stepResult = CreateStepResultWithParameters();
            stepResult.Status = ExecutionStatus.Passed;
            _notifier.Notify(new StepFinished(new EventTime(), stepResult));

            // Parameters should still be rendered even though the status line is suppressed
            var fullMessage = _captured.Single();
            Assert.That(fullMessage, Does.Not.Contain("Passed after"));
            Assert.That(fullMessage, Does.Contain("table:"));
            Assert.That(fullMessage, Does.Contain("tree:"));
        }

        #endregion

        #region Step comment and file attachment

        [Test]
        public void NotifyStepComment_should_include_indented_comment()
        {
            var stepInfo = CreateStepInfo("Given some condition", 1, 3, "");
            var comment = "some important comment";
            _notifier.Notify(new StepCommented(new EventTime(), stepInfo, comment));

            Assert.That(_captured.Single(), Is.EqualTo($"        => /* {comment} */"));
        }

        [Test]
        public void NotifyStepFileAttached_should_include_indented_attachment_info()
        {
            var stepInfo = CreateStepInfo("Given some condition", 1, 3, "");
            var attachment = new FileAttachment("screenshot", "/path/to/file.png", "file.png");
            _notifier.Notify(new StepFileAttached(new EventTime(), stepInfo, attachment));

            Assert.That(_captured.Single(), Is.EqualTo("        => \U0001F517screenshot: /path/to/file.png"));
        }

        #endregion

        #region Indentation

        [Test]
        public void Steps_should_be_indented_under_scenario()
        {
            var stepInfo = CreateStepInfo("Given some condition", 1, 3, "");
            _notifier.Notify(new StepStarting(new EventTime(), stepInfo));

            // Default IndentLength=4, so top-level steps get 4 spaces
            Assert.That(_captured.Single(), Does.StartWith("    "));
        }

        [Test]
        public void Sub_steps_should_be_further_indented()
        {
            var parentStep = CreateStepInfo("Given a parent step", 1, 3, "");
            var childStep = CreateStepInfo("And a child step", 1, 2, "1.");
            childStep.Parent = parentStep;

            _notifier.Notify(new StepStarting(new EventTime(), childStep));

            // Parent adds IndentLength=4, plus base IndentLength=4 = 8 spaces
            Assert.That(_captured.Single(), Does.StartWith("        And a child step"));
        }

        [Test]
        public void Deeply_nested_steps_should_increase_indentation()
        {
            var grandparent = CreateStepInfo("Given a grandparent", 1, 3, "");
            var parent = CreateStepInfo("And a parent", 1, 2, "1.");
            parent.Parent = grandparent;
            var child = CreateStepInfo("And a child", 1, 1, "1.1.");
            child.Parent = parent;

            _notifier.Notify(new StepStarting(new EventTime(), child));

            // grandparent level + parent level + base = 3 * 4 = 12 spaces
            Assert.That(_captured.Single(), Does.StartWith("            And a child"));
        }

        [Test]
        public void Custom_IndentLength_should_be_respected()
        {
            _notifier.IndentLength = 2;
            var stepInfo = CreateStepInfo("Given some condition", 1, 3, "");
            _notifier.Notify(new StepStarting(new EventTime(), stepInfo));

            // IndentLength=2 => base indent is "  " (2 spaces)
            Assert.That(_captured.Single(), Does.StartWith("  Given some condition"));
        }

        [Test]
        public void IndentLength_of_1_should_add_minimum_padding()
        {
            _notifier.IndentLength = 1;
            var parentStep = CreateStepInfo("Given a parent", 1, 3, "");
            var childStep = CreateStepInfo("And a child", 1, 2, "1.");
            childStep.Parent = parentStep;

            _notifier.Notify(new StepStarting(new EventTime(), parentStep));
            _notifier.Notify(new StepStarting(new EventTime(), childStep));

            // IndentLength=1, so minimum 2-space padding applies: "  " + " " = "   "
            Assert.That(_captured.ElementAt(0), Does.StartWith("   Given a parent"));
            // Child: "  " (min padding) + " " (parent) + " " (base) = "    "
            Assert.That(_captured.ElementAt(1), Does.StartWith("    And a child"));
        }

        #endregion

        #region Full integration test

        [Test]
        public void It_should_capture_meaningful_information_with_defaults()
        {
            var featureInfo = new TestResults.TestFeatureInfo
            {
                Name = TestResults.CreateNameInfo("Shopping cart"),
                Labels = new[] { "Story-1" },
                Description = "As a customer I want to manage my cart"
            };
            var scenarioInfo = new TestResults.TestScenarioInfo
            {
                Name = TestResults.CreateNameInfo("Adding items"),
                Labels = new[] { "Ticket-42" },
                Categories = Array.Empty<string>(),
                Description = "Verifies item addition"
            };
            var stepInfo = CreateStepInfo("Given some condition", 1, 3, "");
            var stepResult = CreateStepResult("Given some condition", 1, 3, "", ExecutionStatus.Failed);
            var stepDuration = stepResult.ExecutionTime.Duration.FormatPretty();
            var comment = "important note";
            var attachment = new FileAttachment("screenshot", "/path/screenshot.png", "screenshot.png");

            var scenarioResult = new TestResults.TestScenarioResult
            {
                Info = new TestResults.TestScenarioInfo
                {
                    Name = TestResults.CreateNameInfo("Adding items"),
                    Labels = Array.Empty<string>(),
                    Categories = Array.Empty<string>()
                },
                Status = ExecutionStatus.Failed,
                StatusDetails = "Step 1: some failure",
                ExecutionTime = new TestResults.TestExecutionTime
                {
                    Start = DateTimeOffset.UtcNow,
                    Duration = TimeSpan.FromMilliseconds(250)
                }
            };

            var featureResult = new TestResults.TestFeatureResult
            {
                Info = new TestResults.TestFeatureInfo
                {
                    Name = TestResults.CreateNameInfo("Shopping cart"),
                    Labels = Array.Empty<string>()
                }
            };

            var eventTime = new EventTime();

            _notifier.Notify(new FeatureStarting(eventTime, featureInfo));
            _notifier.Notify(new ScenarioStarting(eventTime, scenarioInfo));
            _notifier.Notify(new StepStarting(eventTime, stepInfo));
            _notifier.Notify(new StepCommented(eventTime, stepInfo, comment));
            _notifier.Notify(new StepFileAttached(eventTime, stepInfo, attachment));
            _notifier.Notify(new StepFinished(eventTime, stepResult));
            _notifier.Notify(new ScenarioFinished(eventTime, scenarioResult));
            _notifier.Notify(new FeatureFinished(eventTime, featureResult));

            var expected = NormalizeNewlines($"""
                FEATURE:  [Story-1] Shopping cart
                          As a customer I want to manage my cart

                SCENARIO: [Ticket-42] Adding items
                          Verifies item addition

                    Given some condition (STEP 1)
                        => /* important note */
                        => 🔗screenshot: /path/screenshot.png
                      => (Failed after {stepDuration}) (STEP 1)

                SCENARIO RESULT: Failed after 250ms
                    Step 1: some failure

                FEATURE FINISHED: Shopping cart
                """);

            var actual = NormalizeNewlines(string.Join("\n", _captured));
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void It_should_capture_meaningful_information_with_known_values()
        {
            var featureInfo = new TestResults.TestFeatureInfo
            {
                Name = TestResults.CreateNameInfo("Shopping cart"),
                Labels = new[] { "Story-1" },
                Description = "As a customer I want to manage my cart"
            };
            var scenarioInfo = new TestResults.TestScenarioInfo
            {
                Name = TestResults.CreateNameInfo("Adding items"),
                Labels = new[] { "Ticket-42" },
                Categories = Array.Empty<string>(),
                Description = "Verifies item addition"
            };
            var stepInfo = CreateStepInfo("Given some condition", 1, 3, "");
            var stepResult = CreateStepResult("Given some condition", 1, 3, "", ExecutionStatus.Failed);
            var stepDuration = stepResult.ExecutionTime.Duration.FormatPretty();

            var scenarioResult = new TestResults.TestScenarioResult
            {
                Info = new TestResults.TestScenarioInfo
                {
                    Name = TestResults.CreateNameInfo("Adding items"),
                    Labels = Array.Empty<string>(),
                    Categories = Array.Empty<string>()
                },
                Status = ExecutionStatus.Failed,
                StatusDetails = "Step 1: some failure",
                ExecutionTime = new TestResults.TestExecutionTime
                {
                    Start = DateTimeOffset.UtcNow,
                    Duration = TimeSpan.FromMilliseconds(250)
                }
            };
            var scenarioDuration = scenarioResult.ExecutionTime.Duration.FormatPretty();

            var featureResult = new TestResults.TestFeatureResult
            {
                Info = new TestResults.TestFeatureInfo
                {
                    Name = TestResults.CreateNameInfo("Shopping cart"),
                    Labels = Array.Empty<string>()
                }
            };

            var eventTime = new EventTime();

            _notifier.Notify(new FeatureStarting(eventTime, featureInfo));
            _notifier.Notify(new ScenarioStarting(eventTime, scenarioInfo));
            _notifier.Notify(new StepStarting(eventTime, stepInfo));
            _notifier.Notify(new StepCommented(eventTime, stepInfo, "important note"));
            _notifier.Notify(new StepFileAttached(eventTime, stepInfo, new FileAttachment("screenshot", "/path/screenshot.png", "screenshot.png")));
            _notifier.Notify(new StepFinished(eventTime, stepResult));
            _notifier.Notify(new ScenarioFinished(eventTime, scenarioResult));
            _notifier.Notify(new FeatureFinished(eventTime, featureResult));

            var expected = NormalizeNewlines($"""
                FEATURE:  [Story-1] Shopping cart
                          As a customer I want to manage my cart

                SCENARIO: [Ticket-42] Adding items
                          Verifies item addition

                    Given some condition (STEP 1)
                        => /* important note */
                        => 🔗screenshot: /path/screenshot.png
                      => (Failed after {stepDuration}) (STEP 1)

                SCENARIO RESULT: Failed after {scenarioDuration}
                    Step 1: some failure

                FEATURE FINISHED: Shopping cart
                """);

            var actual = NormalizeNewlines(string.Join("\n", _captured));
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void It_should_capture_meaningful_information_with_composite_steps()
        {
            var featureInfo = new TestResults.TestFeatureInfo
            {
                Name = TestResults.CreateNameInfo("Checkout flow"),
                Labels = new[] { "Story-1" },
                Description = "As a customer I want to complete my purchase"
            };
            var scenarioInfo = new TestResults.TestScenarioInfo
            {
                Name = TestResults.CreateNameInfo("Composite step handling"),
                Labels = Array.Empty<string>(),
                Categories = Array.Empty<string>(),
                Description = $"Verifies nested step rendering{Environment.NewLine}Another line of the scenario description{Environment.NewLine}Third line of the scenario description"
            };

            // Top-level steps (4 total)
            var givenInfo = CreateStepInfo("Given some setup happens", 1, 4, "");
            var whenInfo = CreateStepInfo("When some action happens", 2, 4, "");
            var thenInfo = CreateStepInfo("Then some top level assertion", 3, 4, "");
            var andInfo = CreateStepInfo("And some other top level assertion", 4, 4, "");

            // Sub-steps of GIVEN (step 1): 3 sub-steps
            var given1Info = CreateStepInfo("A start to the setup happens", 1, 3, "1.");
            given1Info.Parent = givenInfo;
            var given2Info = CreateStepInfo("The setup is executed", 2, 3, "1.");
            given2Info.Parent = givenInfo;
            var given3Info = CreateStepInfo("The setup is verified to be correct", 3, 3, "1.");
            given3Info.Parent = givenInfo;

            // Sub-steps of THEN (step 3): 2 sub-steps
            var then1Info = CreateStepInfo("Some nested assertion", 1, 2, "3.");
            then1Info.Parent = thenInfo;
            var then2Info = CreateStepInfo("Some second nested assertion", 2, 2, "3.");
            then2Info.Parent = thenInfo;

            TestResults.TestStepResult ResultFrom(TestResults.TestStepInfo info, ExecutionStatus status) =>
                new()
                {
                    Info = info,
                    Status = status,
                    ExecutionTime = new TestResults.TestExecutionTime
                    {
                        Start = DateTimeOffset.UtcNow,
                        Duration = TimeSpan.FromMilliseconds(125)
                    }
                };

            var scenarioResult = new TestResults.TestScenarioResult
            {
                Info = new TestResults.TestScenarioInfo
                {
                    Name = TestResults.CreateNameInfo("Composite step handling"),
                    Labels = Array.Empty<string>(),
                    Categories = Array.Empty<string>()
                },
                Status = ExecutionStatus.Failed,
                StatusDetails = "Step 1.3: assertion failed",
                ExecutionTime = new TestResults.TestExecutionTime
                {
                    Start = DateTimeOffset.UtcNow,
                    Duration = TimeSpan.FromMilliseconds(250)
                }
            };

            var featureResult = new TestResults.TestFeatureResult
            {
                Info = new TestResults.TestFeatureInfo
                {
                    Name = TestResults.CreateNameInfo("Checkout flow"),
                    Labels = Array.Empty<string>()
                }
            };

            var eventTime = new EventTime();

            // Feature and scenario start
            _notifier.Notify(new FeatureStarting(eventTime, featureInfo));
            _notifier.Notify(new ScenarioStarting(eventTime, scenarioInfo));

            // GIVEN composite step with 3 sub-steps (sub-step 3 fails)
            _notifier.Notify(new StepStarting(eventTime, givenInfo));
            _notifier.Notify(new StepStarting(eventTime, given1Info));
            _notifier.Notify(new StepFinished(eventTime, ResultFrom(given1Info, ExecutionStatus.Passed)));
            _notifier.Notify(new StepStarting(eventTime, given2Info));
            _notifier.Notify(new StepFinished(eventTime, ResultFrom(given2Info, ExecutionStatus.Passed)));
            _notifier.Notify(new StepStarting(eventTime, given3Info));
            _notifier.Notify(new StepFinished(eventTime, ResultFrom(given3Info, ExecutionStatus.Failed)));
            _notifier.Notify(new StepFinished(eventTime, ResultFrom(givenInfo, ExecutionStatus.Failed)));

            // WHEN simple step (passes)
            _notifier.Notify(new StepStarting(eventTime, whenInfo));
            _notifier.Notify(new StepFinished(eventTime, ResultFrom(whenInfo, ExecutionStatus.Passed)));

            // THEN composite step with 2 sub-steps (sub-step 2 fails)
            _notifier.Notify(new StepStarting(eventTime, thenInfo));
            _notifier.Notify(new StepStarting(eventTime, then1Info));
            _notifier.Notify(new StepFinished(eventTime, ResultFrom(then1Info, ExecutionStatus.Passed)));
            _notifier.Notify(new StepStarting(eventTime, then2Info));
            _notifier.Notify(new StepFinished(eventTime, ResultFrom(then2Info, ExecutionStatus.Failed)));
            _notifier.Notify(new StepFinished(eventTime, ResultFrom(thenInfo, ExecutionStatus.Failed)));

            // AND simple step (ignored after previous failure)
            _notifier.Notify(new StepStarting(eventTime, andInfo));
            _notifier.Notify(new StepFinished(eventTime, ResultFrom(andInfo, ExecutionStatus.Ignored)));

            // Scenario and feature finish
            _notifier.Notify(new ScenarioFinished(eventTime, scenarioResult));
            _notifier.Notify(new FeatureFinished(eventTime, featureResult));

            var expected = NormalizeNewlines("""
                FEATURE:  [Story-1] Checkout flow
                          As a customer I want to complete my purchase

                SCENARIO: Composite step handling
                          Verifies nested step rendering
                          Another line of the scenario description
                          Third line of the scenario description

                    Given some setup happens (STEP 1)
                        A start to the setup happens (STEP 1.1)
                        The setup is executed (STEP 1.2)
                        The setup is verified to be correct (STEP 1.3)
                          => (Failed after 125ms) (STEP 1.3)
                      => (Failed after 125ms) (STEP 1)
                    When some action happens (STEP 2)
                    Then some top level assertion (STEP 3)
                        Some nested assertion (STEP 3.1)
                        Some second nested assertion (STEP 3.2)
                          => (Failed after 125ms) (STEP 3.2)
                      => (Failed after 125ms) (STEP 3)
                    And some other top level assertion (STEP 4)
                      => (Ignored after 125ms) (STEP 4)

                SCENARIO RESULT: Failed after 250ms
                    Step 1.3: assertion failed

                FEATURE FINISHED: Checkout flow
                """);

            var actual = NormalizeNewlines(string.Join("\n", _captured));
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void It_should_capture_multiple_passing_scenarios_with_mixed_nesting()
        {
            var featureInfo = new TestResults.TestFeatureInfo
            {
                Name = TestResults.CreateNameInfo("User management"),
                Labels = new[] { "Epic-5" },
                Description = "Covers user lifecycle operations"
            };
            var featureResult = new TestResults.TestFeatureResult
            {
                Info = new TestResults.TestFeatureInfo
                {
                    Name = TestResults.CreateNameInfo("User management"),
                    Labels = Array.Empty<string>()
                }
            };

            TestResults.TestScenarioInfo ScenarioInfo(string name) => new()
            {
                Name = TestResults.CreateNameInfo(name),
                Labels = Array.Empty<string>(),
                Categories = Array.Empty<string>()
            };

            TestResults.TestScenarioResult PassedScenarioResult(string name) => new()
            {
                Info = ScenarioInfo(name),
                Status = ExecutionStatus.Passed,
                ExecutionTime = new TestResults.TestExecutionTime
                {
                    Start = DateTimeOffset.UtcNow,
                    Duration = TimeSpan.FromMilliseconds(250)
                }
            };

            TestResults.TestStepResult PassedResult(TestResults.TestStepInfo info) => new()
            {
                Info = info,
                Status = ExecutionStatus.Passed,
                ExecutionTime = new TestResults.TestExecutionTime
                {
                    Start = DateTimeOffset.UtcNow,
                    Duration = TimeSpan.FromMilliseconds(125)
                }
            };

            var eventTime = new EventTime();

            _notifier.Notify(new FeatureStarting(eventTime, featureInfo));

            // --- Scenario 1: flat (0 levels of nesting), has GIVEN-AND and THEN-AND ---
            var s1 = ScenarioInfo("List users");
            var s1_given = CreateStepInfo("Given the system has users", 1, 5, "");
            var s1_givenAnd = CreateStepInfo("And the user has permissions", 2, 5, "");
            var s1_when = CreateStepInfo("When I request the user list", 3, 5, "");
            var s1_then = CreateStepInfo("Then all users are returned", 4, 5, "");
            var s1_thenAnd = CreateStepInfo("And the response contains user details", 5, 5, "");

            _notifier.Notify(new ScenarioStarting(eventTime, s1));
            _notifier.Notify(new StepStarting(eventTime, s1_given));
            _notifier.Notify(new StepFinished(eventTime, PassedResult(s1_given)));
            _notifier.Notify(new StepStarting(eventTime, s1_givenAnd));
            _notifier.Notify(new StepFinished(eventTime, PassedResult(s1_givenAnd)));
            _notifier.Notify(new StepStarting(eventTime, s1_when));
            _notifier.Notify(new StepFinished(eventTime, PassedResult(s1_when)));
            _notifier.Notify(new StepStarting(eventTime, s1_then));
            _notifier.Notify(new StepFinished(eventTime, PassedResult(s1_then)));
            _notifier.Notify(new StepStarting(eventTime, s1_thenAnd));
            _notifier.Notify(new StepFinished(eventTime, PassedResult(s1_thenAnd)));
            _notifier.Notify(new ScenarioFinished(eventTime, PassedScenarioResult("List users")));

            // --- Scenario 2: 1 level of nesting ---
            var s2 = ScenarioInfo("Create user");
            var s2_given = CreateStepInfo("Given valid registration data", 1, 3, "");
            var s2_when = CreateStepInfo("When the user is created", 2, 3, "");
            var s2_when1 = CreateStepInfo("The API is called", 1, 3, "2.");
            s2_when1.Parent = s2_when;
            var s2_when2 = CreateStepInfo("The response is parsed", 2, 3, "2.");
            s2_when2.Parent = s2_when;
            var s2_when3 = CreateStepInfo("The user is stored", 3, 3, "2.");
            s2_when3.Parent = s2_when;
            var s2_then = CreateStepInfo("Then the user appears in the system", 3, 3, "");

            _notifier.Notify(new ScenarioStarting(eventTime, s2));
            _notifier.Notify(new StepStarting(eventTime, s2_given));
            _notifier.Notify(new StepFinished(eventTime, PassedResult(s2_given)));
            _notifier.Notify(new StepStarting(eventTime, s2_when));
            _notifier.Notify(new StepStarting(eventTime, s2_when1));
            _notifier.Notify(new StepFinished(eventTime, PassedResult(s2_when1)));
            _notifier.Notify(new StepStarting(eventTime, s2_when2));
            _notifier.Notify(new StepFinished(eventTime, PassedResult(s2_when2)));
            _notifier.Notify(new StepStarting(eventTime, s2_when3));
            _notifier.Notify(new StepFinished(eventTime, PassedResult(s2_when3)));
            _notifier.Notify(new StepFinished(eventTime, PassedResult(s2_when)));
            _notifier.Notify(new StepStarting(eventTime, s2_then));
            _notifier.Notify(new StepFinished(eventTime, PassedResult(s2_then)));
            _notifier.Notify(new ScenarioFinished(eventTime, PassedScenarioResult("Create user")));

            // --- Scenario 3: 2 levels of nesting, has THEN-AND ---
            var s3 = ScenarioInfo("Delete user with cascade");
            var s3_given = CreateStepInfo("Given a user with nested resources", 1, 4, "");
            var s3_when = CreateStepInfo("When the user is deleted", 2, 4, "");
            // sub-steps of s3_when (level 1)
            var s3_when1 = CreateStepInfo("The dependencies are resolved", 1, 2, "2.");
            s3_when1.Parent = s3_when;
            var s3_when2 = CreateStepInfo("The deletion is executed", 2, 2, "2.");
            s3_when2.Parent = s3_when;
            // sub-sub-steps of s3_when2 (level 2)
            var s3_when2a = CreateStepInfo("Child resources are removed", 1, 2, "2.2.");
            s3_when2a.Parent = s3_when2;
            var s3_when2b = CreateStepInfo("The user record is removed", 2, 2, "2.2.");
            s3_when2b.Parent = s3_when2;
            var s3_then = CreateStepInfo("Then the user no longer exists", 3, 4, "");
            var s3_thenAnd = CreateStepInfo("And all nested resources are gone", 4, 4, "");

            _notifier.Notify(new ScenarioStarting(eventTime, s3));
            _notifier.Notify(new StepStarting(eventTime, s3_given));
            _notifier.Notify(new StepFinished(eventTime, PassedResult(s3_given)));
            _notifier.Notify(new StepStarting(eventTime, s3_when));
            _notifier.Notify(new StepStarting(eventTime, s3_when1));
            _notifier.Notify(new StepFinished(eventTime, PassedResult(s3_when1)));
            _notifier.Notify(new StepStarting(eventTime, s3_when2));
            _notifier.Notify(new StepStarting(eventTime, s3_when2a));
            _notifier.Notify(new StepFinished(eventTime, PassedResult(s3_when2a)));
            _notifier.Notify(new StepStarting(eventTime, s3_when2b));
            _notifier.Notify(new StepFinished(eventTime, PassedResult(s3_when2b)));
            _notifier.Notify(new StepFinished(eventTime, PassedResult(s3_when2)));
            _notifier.Notify(new StepFinished(eventTime, PassedResult(s3_when)));
            _notifier.Notify(new StepStarting(eventTime, s3_then));
            _notifier.Notify(new StepFinished(eventTime, PassedResult(s3_then)));
            _notifier.Notify(new StepStarting(eventTime, s3_thenAnd));
            _notifier.Notify(new StepFinished(eventTime, PassedResult(s3_thenAnd)));
            _notifier.Notify(new ScenarioFinished(eventTime, PassedScenarioResult("Delete user with cascade")));

            // --- Scenario 4: complex nesting in GIVEN, WHEN and THEN ---
            var s4 = ScenarioInfo("Transfer user ownership");
            // GIVEN with nested + double-nested sub-steps
            var s4_given = CreateStepInfo("Given an owner with resources", 1, 5, "");
            var s4_given1 = CreateStepInfo("The owner account is loaded", 1, 2, "1.");
            s4_given1.Parent = s4_given;
            var s4_given2 = CreateStepInfo("The resources are enumerated", 2, 2, "1.");
            s4_given2.Parent = s4_given;
            var s4_given2a = CreateStepInfo("Active resources are found", 1, 2, "1.2.");
            s4_given2a.Parent = s4_given2;
            var s4_given2b = CreateStepInfo("Archived resources are found", 2, 2, "1.2.");
            s4_given2b.Parent = s4_given2;
            // AND (flat)
            var s4_givenAnd = CreateStepInfo("And a target user exists", 2, 5, "");
            // WHEN with nested sub-steps
            var s4_when = CreateStepInfo("When ownership is transferred", 3, 5, "");
            var s4_when1 = CreateStepInfo("Permissions are reassigned", 1, 2, "3.");
            s4_when1.Parent = s4_when;
            var s4_when2 = CreateStepInfo("Notifications are sent", 2, 2, "3.");
            s4_when2.Parent = s4_when;
            // THEN with nested + double-nested sub-steps
            var s4_then = CreateStepInfo("Then the target owns all resources", 4, 5, "");
            var s4_then1 = CreateStepInfo("Active resources belong to target", 1, 2, "4.");
            s4_then1.Parent = s4_then;
            var s4_then2 = CreateStepInfo("Archived resources belong to target", 2, 2, "4.");
            s4_then2.Parent = s4_then;
            var s4_then2a = CreateStepInfo("Read-only archives are transferred", 1, 2, "4.2.");
            s4_then2a.Parent = s4_then2;
            var s4_then2b = CreateStepInfo("Mutable archives are transferred", 2, 2, "4.2.");
            s4_then2b.Parent = s4_then2;
            // AND (flat)
            var s4_thenAnd = CreateStepInfo("And the original owner has no resources", 5, 5, "");

            _notifier.Notify(new ScenarioStarting(eventTime, s4));
            _notifier.Notify(new StepStarting(eventTime, s4_given));
            _notifier.Notify(new StepStarting(eventTime, s4_given1));
            _notifier.Notify(new StepFinished(eventTime, PassedResult(s4_given1)));
            _notifier.Notify(new StepStarting(eventTime, s4_given2));
            _notifier.Notify(new StepStarting(eventTime, s4_given2a));
            _notifier.Notify(new StepFinished(eventTime, PassedResult(s4_given2a)));
            _notifier.Notify(new StepStarting(eventTime, s4_given2b));
            _notifier.Notify(new StepFinished(eventTime, PassedResult(s4_given2b)));
            _notifier.Notify(new StepFinished(eventTime, PassedResult(s4_given2)));
            _notifier.Notify(new StepFinished(eventTime, PassedResult(s4_given)));
            _notifier.Notify(new StepStarting(eventTime, s4_givenAnd));
            _notifier.Notify(new StepFinished(eventTime, PassedResult(s4_givenAnd)));
            _notifier.Notify(new StepStarting(eventTime, s4_when));
            _notifier.Notify(new StepStarting(eventTime, s4_when1));
            _notifier.Notify(new StepFinished(eventTime, PassedResult(s4_when1)));
            _notifier.Notify(new StepStarting(eventTime, s4_when2));
            _notifier.Notify(new StepFinished(eventTime, PassedResult(s4_when2)));
            _notifier.Notify(new StepFinished(eventTime, PassedResult(s4_when)));
            _notifier.Notify(new StepStarting(eventTime, s4_then));
            _notifier.Notify(new StepStarting(eventTime, s4_then1));
            _notifier.Notify(new StepFinished(eventTime, PassedResult(s4_then1)));
            _notifier.Notify(new StepStarting(eventTime, s4_then2));
            _notifier.Notify(new StepStarting(eventTime, s4_then2a));
            _notifier.Notify(new StepFinished(eventTime, PassedResult(s4_then2a)));
            _notifier.Notify(new StepStarting(eventTime, s4_then2b));
            _notifier.Notify(new StepFinished(eventTime, PassedResult(s4_then2b)));
            _notifier.Notify(new StepFinished(eventTime, PassedResult(s4_then2)));
            _notifier.Notify(new StepFinished(eventTime, PassedResult(s4_then)));
            _notifier.Notify(new StepStarting(eventTime, s4_thenAnd));
            _notifier.Notify(new StepFinished(eventTime, PassedResult(s4_thenAnd)));
            _notifier.Notify(new ScenarioFinished(eventTime, PassedScenarioResult("Transfer user ownership")));

            _notifier.Notify(new FeatureFinished(eventTime, featureResult));

            var expected = NormalizeNewlines("""
                FEATURE:  [Epic-5] User management
                          Covers user lifecycle operations

                SCENARIO: List users

                    Given the system has users (STEP 1)
                    And the user has permissions (STEP 2)
                    When I request the user list (STEP 3)
                    Then all users are returned (STEP 4)
                    And the response contains user details (STEP 5)

                SCENARIO RESULT: Passed after 250ms

                SCENARIO: Create user

                    Given valid registration data (STEP 1)
                    When the user is created (STEP 2)
                        The API is called (STEP 2.1)
                        The response is parsed (STEP 2.2)
                        The user is stored (STEP 2.3)
                    Then the user appears in the system (STEP 3)

                SCENARIO RESULT: Passed after 250ms

                SCENARIO: Delete user with cascade

                    Given a user with nested resources (STEP 1)
                    When the user is deleted (STEP 2)
                        The dependencies are resolved (STEP 2.1)
                        The deletion is executed (STEP 2.2)
                            Child resources are removed (STEP 2.2.1)
                            The user record is removed (STEP 2.2.2)
                    Then the user no longer exists (STEP 3)
                    And all nested resources are gone (STEP 4)

                SCENARIO RESULT: Passed after 250ms

                SCENARIO: Transfer user ownership

                    Given an owner with resources (STEP 1)
                        The owner account is loaded (STEP 1.1)
                        The resources are enumerated (STEP 1.2)
                            Active resources are found (STEP 1.2.1)
                            Archived resources are found (STEP 1.2.2)
                    And a target user exists (STEP 2)
                    When ownership is transferred (STEP 3)
                        Permissions are reassigned (STEP 3.1)
                        Notifications are sent (STEP 3.2)
                    Then the target owns all resources (STEP 4)
                        Active resources belong to target (STEP 4.1)
                        Archived resources belong to target (STEP 4.2)
                            Read-only archives are transferred (STEP 4.2.1)
                            Mutable archives are transferred (STEP 4.2.2)
                    And the original owner has no resources (STEP 5)

                SCENARIO RESULT: Passed after 250ms

                FEATURE FINISHED: User management
                """);

            var actual = NormalizeNewlines(string.Join("\n", _captured));
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void It_should_show_failure_in_deeply_nested_Then_step()
        {
            var scenarioInfo = new TestResults.TestScenarioInfo
            {
                Name = TestResults.CreateNameInfo("Transfer user ownership"),
                Labels = Array.Empty<string>(),
                Categories = Array.Empty<string>()
            };

            // GIVEN with nested + double-nested sub-steps (all pass)
            var given = CreateStepInfo("Given an owner with resources", 1, 5, "");
            var given1 = CreateStepInfo("The owner account is loaded", 1, 2, "1.");
            given1.Parent = given;
            var given2 = CreateStepInfo("The resources are enumerated", 2, 2, "1.");
            given2.Parent = given;
            var given2a = CreateStepInfo("Active resources are found", 1, 2, "1.2.");
            given2a.Parent = given2;
            var given2b = CreateStepInfo("Archived resources are found", 2, 2, "1.2.");
            given2b.Parent = given2;

            // AND (passes)
            var givenAnd = CreateStepInfo("And a target user exists", 2, 5, "");

            // WHEN with nested sub-steps (all pass)
            var when = CreateStepInfo("When ownership is transferred", 3, 5, "");
            var when1 = CreateStepInfo("Permissions are reassigned", 1, 2, "3.");
            when1.Parent = when;
            var when2 = CreateStepInfo("Notifications are sent", 2, 2, "3.");
            when2.Parent = when;

            // THEN with nested + double-nested sub-steps (double-nested step fails)
            var then = CreateStepInfo("Then the target owns all resources", 4, 5, "");
            var then1 = CreateStepInfo("Active resources belong to target", 1, 2, "4.");
            then1.Parent = then;
            var then2 = CreateStepInfo("Archived resources belong to target", 2, 2, "4.");
            then2.Parent = then;
            var then2a = CreateStepInfo("Read-only archives are transferred", 1, 2, "4.2.");
            then2a.Parent = then2;
            var then2b = CreateStepInfo("Mutable archives are transferred", 2, 2, "4.2.");
            then2b.Parent = then2;

            // AND (ignored after failure)
            var thenAnd = CreateStepInfo("And the original owner has no resources", 5, 5, "");

            TestResults.TestStepResult Result(TestResults.TestStepInfo info, ExecutionStatus status) => new()
            {
                Info = info,
                Status = status,
                ExecutionTime = new TestResults.TestExecutionTime
                {
                    Start = DateTimeOffset.UtcNow,
                    Duration = TimeSpan.FromMilliseconds(125)
                }
            };

            var scenarioResult = new TestResults.TestScenarioResult
            {
                Info = new TestResults.TestScenarioInfo
                {
                    Name = TestResults.CreateNameInfo("Transfer user ownership"),
                    Labels = Array.Empty<string>(),
                    Categories = Array.Empty<string>()
                },
                Status = ExecutionStatus.Failed,
                StatusDetails = "Step 4.2.2: archive transfer failed",
                ExecutionTime = new TestResults.TestExecutionTime
                {
                    Start = DateTimeOffset.UtcNow,
                    Duration = TimeSpan.FromMilliseconds(500)
                }
            };

            var eventTime = new EventTime();

            _notifier.Notify(new ScenarioStarting(eventTime, scenarioInfo));

            // GIVEN (all pass)
            _notifier.Notify(new StepStarting(eventTime, given));
            _notifier.Notify(new StepStarting(eventTime, given1));
            _notifier.Notify(new StepFinished(eventTime, Result(given1, ExecutionStatus.Passed)));
            _notifier.Notify(new StepStarting(eventTime, given2));
            _notifier.Notify(new StepStarting(eventTime, given2a));
            _notifier.Notify(new StepFinished(eventTime, Result(given2a, ExecutionStatus.Passed)));
            _notifier.Notify(new StepStarting(eventTime, given2b));
            _notifier.Notify(new StepFinished(eventTime, Result(given2b, ExecutionStatus.Passed)));
            _notifier.Notify(new StepFinished(eventTime, Result(given2, ExecutionStatus.Passed)));
            _notifier.Notify(new StepFinished(eventTime, Result(given, ExecutionStatus.Passed)));

            // AND (passes)
            _notifier.Notify(new StepStarting(eventTime, givenAnd));
            _notifier.Notify(new StepFinished(eventTime, Result(givenAnd, ExecutionStatus.Passed)));

            // WHEN (all pass)
            _notifier.Notify(new StepStarting(eventTime, when));
            _notifier.Notify(new StepStarting(eventTime, when1));
            _notifier.Notify(new StepFinished(eventTime, Result(when1, ExecutionStatus.Passed)));
            _notifier.Notify(new StepStarting(eventTime, when2));
            _notifier.Notify(new StepFinished(eventTime, Result(when2, ExecutionStatus.Passed)));
            _notifier.Notify(new StepFinished(eventTime, Result(when, ExecutionStatus.Passed)));

            // THEN (double-nested step 4.2.2 fails, cascades up)
            _notifier.Notify(new StepStarting(eventTime, then));
            _notifier.Notify(new StepStarting(eventTime, then1));
            _notifier.Notify(new StepFinished(eventTime, Result(then1, ExecutionStatus.Passed)));
            _notifier.Notify(new StepStarting(eventTime, then2));
            _notifier.Notify(new StepStarting(eventTime, then2a));
            _notifier.Notify(new StepFinished(eventTime, Result(then2a, ExecutionStatus.Passed)));
            _notifier.Notify(new StepStarting(eventTime, then2b));
            _notifier.Notify(new StepFinished(eventTime, Result(then2b, ExecutionStatus.Failed)));
            _notifier.Notify(new StepFinished(eventTime, Result(then2, ExecutionStatus.Failed)));
            _notifier.Notify(new StepFinished(eventTime, Result(then, ExecutionStatus.Failed)));

            // AND (ignored after failure)
            _notifier.Notify(new StepStarting(eventTime, thenAnd));
            _notifier.Notify(new StepFinished(eventTime, Result(thenAnd, ExecutionStatus.Ignored)));

            _notifier.Notify(new ScenarioFinished(eventTime, scenarioResult));

            var expected = NormalizeNewlines("""
                SCENARIO: Transfer user ownership

                    Given an owner with resources (STEP 1)
                        The owner account is loaded (STEP 1.1)
                        The resources are enumerated (STEP 1.2)
                            Active resources are found (STEP 1.2.1)
                            Archived resources are found (STEP 1.2.2)
                    And a target user exists (STEP 2)
                    When ownership is transferred (STEP 3)
                        Permissions are reassigned (STEP 3.1)
                        Notifications are sent (STEP 3.2)
                    Then the target owns all resources (STEP 4)
                        Active resources belong to target (STEP 4.1)
                        Archived resources belong to target (STEP 4.2)
                            Read-only archives are transferred (STEP 4.2.1)
                            Mutable archives are transferred (STEP 4.2.2)
                              => (Failed after 125ms) (STEP 4.2.2)
                          => (Failed after 125ms) (STEP 4.2)
                      => (Failed after 125ms) (STEP 4)
                    And the original owner has no resources (STEP 5)
                      => (Ignored after 125ms) (STEP 5)

                SCENARIO RESULT: Failed after 500ms
                    Step 4.2.2: archive transfer failed

                """);

            var actual = NormalizeNewlines(string.Join("\n", _captured));
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void It_should_show_failure_in_nested_Given_And_step()
        {
            var scenarioInfo = new TestResults.TestScenarioInfo
            {
                Name = TestResults.CreateNameInfo("Provision team workspace"),
                Labels = Array.Empty<string>(),
                Categories = Array.Empty<string>()
            };

            // GIVEN (passes)
            var given = CreateStepInfo("Given an organization exists", 1, 4, "");

            // AND with nested sub-steps (nested step fails)
            var givenAnd = CreateStepInfo("And the team is configured", 2, 4, "");
            var givenAnd1 = CreateStepInfo("Team roles are assigned", 1, 2, "2.");
            givenAnd1.Parent = givenAnd;
            var givenAnd2 = CreateStepInfo("Team quota is validated", 2, 2, "2.");
            givenAnd2.Parent = givenAnd;
            var givenAnd2a = CreateStepInfo("Storage quota is checked", 1, 2, "2.2.");
            givenAnd2a.Parent = givenAnd2;
            var givenAnd2b = CreateStepInfo("Seat limit is checked", 2, 2, "2.2.");
            givenAnd2b.Parent = givenAnd2;

            // WHEN (ignored after setup failure)
            var when = CreateStepInfo("When the workspace is provisioned", 3, 4, "");

            // THEN (ignored after setup failure)
            var then = CreateStepInfo("Then the workspace is available", 4, 4, "");

            TestResults.TestStepResult Result(TestResults.TestStepInfo info, ExecutionStatus status) => new()
            {
                Info = info,
                Status = status,
                ExecutionTime = new TestResults.TestExecutionTime
                {
                    Start = DateTimeOffset.UtcNow,
                    Duration = TimeSpan.FromMilliseconds(125)
                }
            };

            var scenarioResult = new TestResults.TestScenarioResult
            {
                Info = new TestResults.TestScenarioInfo
                {
                    Name = TestResults.CreateNameInfo("Provision team workspace"),
                    Labels = Array.Empty<string>(),
                    Categories = Array.Empty<string>()
                },
                Status = ExecutionStatus.Failed,
                StatusDetails = "Step 2.2.2: seat limit exceeded",
                ExecutionTime = new TestResults.TestExecutionTime
                {
                    Start = DateTimeOffset.UtcNow,
                    Duration = TimeSpan.FromMilliseconds(300)
                }
            };

            var eventTime = new EventTime();

            _notifier.Notify(new ScenarioStarting(eventTime, scenarioInfo));

            // GIVEN (passes)
            _notifier.Notify(new StepStarting(eventTime, given));
            _notifier.Notify(new StepFinished(eventTime, Result(given, ExecutionStatus.Passed)));

            // AND (nested step 2.2.2 fails, cascades up)
            _notifier.Notify(new StepStarting(eventTime, givenAnd));
            _notifier.Notify(new StepStarting(eventTime, givenAnd1));
            _notifier.Notify(new StepFinished(eventTime, Result(givenAnd1, ExecutionStatus.Passed)));
            _notifier.Notify(new StepStarting(eventTime, givenAnd2));
            _notifier.Notify(new StepStarting(eventTime, givenAnd2a));
            _notifier.Notify(new StepFinished(eventTime, Result(givenAnd2a, ExecutionStatus.Passed)));
            _notifier.Notify(new StepStarting(eventTime, givenAnd2b));
            _notifier.Notify(new StepFinished(eventTime, Result(givenAnd2b, ExecutionStatus.Failed)));
            _notifier.Notify(new StepFinished(eventTime, Result(givenAnd2, ExecutionStatus.Failed)));
            _notifier.Notify(new StepFinished(eventTime, Result(givenAnd, ExecutionStatus.Failed)));

            // WHEN (ignored)
            _notifier.Notify(new StepStarting(eventTime, when));
            _notifier.Notify(new StepFinished(eventTime, Result(when, ExecutionStatus.Ignored)));

            // THEN (ignored)
            _notifier.Notify(new StepStarting(eventTime, then));
            _notifier.Notify(new StepFinished(eventTime, Result(then, ExecutionStatus.Ignored)));

            _notifier.Notify(new ScenarioFinished(eventTime, scenarioResult));

            var expected = NormalizeNewlines("""
                SCENARIO: Provision team workspace

                    Given an organization exists (STEP 1)
                    And the team is configured (STEP 2)
                        Team roles are assigned (STEP 2.1)
                        Team quota is validated (STEP 2.2)
                            Storage quota is checked (STEP 2.2.1)
                            Seat limit is checked (STEP 2.2.2)
                              => (Failed after 125ms) (STEP 2.2.2)
                          => (Failed after 125ms) (STEP 2.2)
                      => (Failed after 125ms) (STEP 2)
                    When the workspace is provisioned (STEP 3)
                      => (Ignored after 125ms) (STEP 3)
                    Then the workspace is available (STEP 4)
                      => (Ignored after 125ms) (STEP 4)

                SCENARIO RESULT: Failed after 300ms
                    Step 2.2.2: seat limit exceeded

                """);

            var actual = NormalizeNewlines(string.Join("\n", _captured));
            Assert.That(actual, Is.EqualTo(expected));
        }

        #endregion

        #region Helpers

        private static TestResults.TestStepInfo CreateStepInfo(string name, int number, int total, string groupPrefix)
        {
            return new TestResults.TestStepInfo
            {
                Name = TestResults.CreateStepName(name, null, name),
                Number = number,
                Total = total,
                GroupPrefix = groupPrefix
            };
        }

        private static TestResults.TestStepResult CreateStepResult(string name, int number, int total, string groupPrefix, ExecutionStatus status)
        {
            return new TestResults.TestStepResult
            {
                Info = CreateStepInfo(name, number, total, groupPrefix),
                Status = status,
                ExecutionTime = new TestResults.TestExecutionTime
                {
                    Start = DateTimeOffset.UtcNow,
                    Duration = TimeSpan.FromMilliseconds(125)
                }
            };
        }

        private static TestResults.TestStepResult CreateStepResultWithParameters()
        {
            var result = CreateStepResult("Given some condition", 1, 3, "", ExecutionStatus.Failed);
            result.Parameters = new IParameterResult[]
            {
                new TestResults.TestParameterResult("table",
                    TestResults.CreateTabularParameterDetails(ParameterVerificationStatus.Failure)
                        .WithKeyColumns("Key")
                        .WithValueColumns("Value1", "Value2")
                        .AddRow(TableRowType.Matching,
                            ParameterVerificationStatus.Success,
                            TestResults.CreateValueResult("1"),
                            TestResults.CreateValueResult("abc"),
                            TestResults.CreateValueResult("some value"))
                        .AddRow(TableRowType.Matching,
                            ParameterVerificationStatus.Failure,
                            TestResults.CreateValueResult("2"),
                            TestResults.CreateValueResult("def"),
                            TestResults.CreateValueResult("value", "val", ParameterVerificationStatus.Failure))
                        .AddRow(TableRowType.Missing,
                            ParameterVerificationStatus.Failure,
                            TestResults.CreateValueResult("3"),
                            TestResults.CreateValueResult("XXX", "<null>", ParameterVerificationStatus.NotProvided),
                            TestResults.CreateValueResult("YYY", "<null>", ParameterVerificationStatus.NotProvided))
                        .AddRow(TableRowType.Surplus,
                            ParameterVerificationStatus.Failure,
                            TestResults.CreateValueResult("4"),
                            TestResults.CreateValueResult("<null>", "XXX", ParameterVerificationStatus.Failure),
                            TestResults.CreateValueResult("<null>", "YYY", ParameterVerificationStatus.Failure))
                ),
                new TestResults.TestParameterResult("tree", CreateTreeParameterResult())
            };
            return result;
        }

        private static IParameterDetails CreateTreeParameterResult()
        {
            var expected = new
            {
                Name = "Bob",
                Surname = "Johnson",
                Items = new[] { false }
            };
            var actual = new
            {
                Name = "Bob",
                LastName = "Johnson",
                Items = new[] { true }
            };
            var tree = Tree.ExpectEquivalent(expected);
            tree.SetActual(actual);
            return tree.Details;
        }

        /// <summary>
        /// Normalizes line endings in raw string literals to match <see cref="Environment.NewLine"/>
        /// so that assertions are platform-independent.
        /// </summary>
        private static string NormalizeNewlines(string text) => text.Replace("\r\n", "\n").Replace("\n", Environment.NewLine);

        #endregion
    }
}
