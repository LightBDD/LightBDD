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
    public class DefaultProgressNotifier_tests
    {
        private Queue<string> _captured;
        private DefaultProgressNotifier _notifier;

        private void Notify(string message)
        {
            _captured.Enqueue(message);
        }

        [SetUp]
        public void SetUp()
        {
            _captured = new Queue<string>();
            _notifier = new DefaultProgressNotifier(Notify);
        }

        [Test]
        public void It_should_capture_meaningful_information()
        {
            var featureInfo = Fake.Object<TestResults.TestFeatureInfo>();
            var scenarioInfo = Fake.Object<TestResults.TestScenarioInfo>();
            var stepInfo = Fake.Object<TestResults.TestStepInfo>();
            var stepResult = Fake.Object<TestResults.TestStepResult>();
            stepResult.Parameters = new IParameterResult[]
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
                            TestResults.CreateValueResult("<null>", "XXX",
                                ParameterVerificationStatus.Failure),
                            TestResults.CreateValueResult("<null>", "YYY",
                                ParameterVerificationStatus.Failure))
                ),
                new TestResults.TestParameterResult("tree",CreateTreeParameterResult())
            };
            var scenarioResult = Fake.Object<TestResults.TestScenarioResult>();
            scenarioResult.Status = ExecutionStatus.Passed;

            var featureResult = Fake.Object<TestResults.TestFeatureResult>();
            var comment = Fake.String();
            var attachment = new FileAttachment(Fake.String(), Fake.String(), Fake.String());

            var eventTime = new EventTime();
            _notifier.Notify(new FeatureStarting(eventTime, featureInfo));
            _notifier.Notify(new ScenarioStarting(eventTime, scenarioInfo));
            _notifier.Notify(new StepStarting(eventTime, stepInfo));
            _notifier.Notify(new StepCommented(eventTime, stepInfo, comment));
            _notifier.Notify(new StepFileAttached(eventTime, stepInfo, attachment));
            _notifier.Notify(new StepFinished(eventTime, stepResult));
            _notifier.Notify(new ScenarioFinished(eventTime, scenarioResult));
            _notifier.Notify(new FeatureFinished(eventTime, featureResult));

            var expectedTable = @"    table:
    +-+---+----------+----------+
    |#|Key|Value1    |Value2    |
    +-+---+----------+----------+
    |=|1  |abc       |some value|
    |!|2  |def       |val/value |
    |-|3  |<null>/XXX|<null>/YYY|
    |+|4  |XXX/<null>|YYY/<null>|
    +-+---+----------+----------+"
                .Replace("\r", "")
                .Replace("\n", Environment.NewLine);

            var expectedTree = @"    tree:
    = $: <object>
    = $.Items: <array:1>
    ! $.Items[0]: False/True
    = $.Name: Bob
    ! $.Surname: Johnson/<none>
    ! $.LastName: <none>/Johnson"
                .Replace("\r", "")
                .Replace("\n", Environment.NewLine);

            var expected = new[]
            {
                $"FEATURE: [{string.Join("][", featureInfo.Labels)}] {featureInfo.Name}{Environment.NewLine}  {featureInfo.Description}",
                $"SCENARIO: [{string.Join("][", scenarioInfo.Labels)}] {scenarioInfo.Name}{Environment.NewLine}  {scenarioInfo.Description}",
                $"  STEP {stepInfo.GroupPrefix}{stepInfo.Number}/{stepInfo.GroupPrefix}{stepInfo.Total}: {stepInfo.Name}...",
                $"  STEP {stepInfo.GroupPrefix}{stepInfo.Number}/{stepInfo.GroupPrefix}{stepInfo.Total}: /* {comment} */",
                $"  STEP {stepInfo.GroupPrefix}{stepInfo.Number}/{stepInfo.GroupPrefix}{stepInfo.Total}: 🔗{attachment.Name}: {attachment.FilePath}",
                $"  STEP {stepResult.Info.GroupPrefix}{stepResult.Info.Number}/{stepResult.Info.GroupPrefix}{stepResult.Info.Total}: {stepResult.Info.Name} ({stepResult.Status} after {stepResult.ExecutionTime.Duration.FormatPretty()}){Environment.NewLine}{expectedTable}{Environment.NewLine}{expectedTree}",
                $"  SCENARIO RESULT: {scenarioResult.Status} after {scenarioResult.ExecutionTime.Duration.FormatPretty()}{Environment.NewLine}    {scenarioResult.StatusDetails}",
                $"FEATURE FINISHED: {featureResult.Info.Name}"
            };

            Assert.That(_captured.ToArray(), Is.EqualTo(expected));
        }

        private IParameterDetails CreateTreeParameterResult()
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

        [Test]
        public void NotifyFeatureStart_should_omit_description_if_not_provided()
        {
            var featureInfo = Fake.Object<TestResults.TestFeatureInfo>();
            featureInfo.Description = null;
            _notifier.Notify(new FeatureStarting(new EventTime(), featureInfo));

            Assert.That(_captured.Single(), Is.EqualTo($"FEATURE: [{string.Join("][", featureInfo.Labels)}] {featureInfo.Name}"));
        }

        [Test]
        public void NotifyFeatureStart_should_omit_labels_if_not_provided()
        {
            var featureInfo = Fake.Object<TestResults.TestFeatureInfo>();
            featureInfo.Labels = Array.Empty<string>();
            _notifier.Notify(new FeatureStarting(new EventTime(), featureInfo));

            var expected = $"FEATURE: {featureInfo.Name}{Environment.NewLine}  {featureInfo.Description}";
            Assert.That(_captured.Single(), Is.EqualTo(expected));
        }

        [Test]
        public void NotifyScenarioStart_should_omit_labels_if_not_provided()
        {
            var scenarioInfo = Fake.Object<TestResults.TestScenarioInfo>();
            scenarioInfo.Labels = Array.Empty<string>();
            _notifier.Notify(new ScenarioStarting(new EventTime(), scenarioInfo));

            var expected = $"SCENARIO: {scenarioInfo.Name}{Environment.NewLine}  {scenarioInfo.Description}";
            Assert.That(_captured.Single(), Is.EqualTo(expected));
        }

        [Test]
        public void NotifyScenarioFinished_should_omit_execution_time_if_not_provided()
        {
            var scenarioInfo = Fake.Object<TestResults.TestScenarioInfo>();
            scenarioInfo.Labels = Array.Empty<string>();
            var scenarioResult = Fake.Object<TestResults.TestScenarioResult>();
            scenarioResult.Info = scenarioInfo;
            scenarioResult.Status = ExecutionStatus.Passed;
            scenarioResult.ExecutionTime = null;

            _notifier.Notify(new ScenarioStarting(new EventTime(), scenarioInfo));
            _notifier.Notify(new ScenarioFinished(new EventTime(), scenarioResult));

            var expected = new[]
            {
                $"SCENARIO: {scenarioInfo.Name}{Environment.NewLine}  {scenarioInfo.Description}",
                $"  SCENARIO RESULT: {scenarioResult.Status}{Environment.NewLine}    {scenarioResult.StatusDetails}"
            };
            Assert.That(_captured.ToArray(), Is.EqualTo(expected));
        }

        [Test]
        public void NotifyScenarioFinished_should_omit_status_details_if_not_provided()
        {
            var scenarioInfo = Fake.Object<TestResults.TestScenarioInfo>();
            scenarioInfo.Labels = Array.Empty<string>();
            var scenarioResult = Fake.Object<TestResults.TestScenarioResult>();
            scenarioResult.Info = scenarioInfo;
            scenarioResult.Status = ExecutionStatus.Passed;
            scenarioResult.StatusDetails = null;

            _notifier.Notify(new ScenarioStarting(new EventTime(), scenarioInfo));
            _notifier.Notify(new ScenarioFinished(new EventTime(), scenarioResult));

            var expected = new[]
            {
                $"SCENARIO: {scenarioInfo.Name}{Environment.NewLine}  {scenarioInfo.Description}",
                $"  SCENARIO RESULT: {scenarioResult.Status} after {scenarioResult.ExecutionTime.Duration.FormatPretty()}"
            };
            Assert.That(_captured.ToArray(), Is.EqualTo(expected));
        }

        #region Step start customizations

        [Test]
        public void NotifyStepStart_should_use_prefix_numbering_by_default()
        {
            var stepInfo = CreateStepInfo("Given some condition", 1, 3, "");
            _notifier.Notify(new StepStarting(new EventTime(), stepInfo));

            Assert.That(_captured.Single(), Is.EqualTo("  STEP 1/3: Given some condition..."));
        }

        [Test]
        public void NotifyStepStart_with_suffix_numbering()
        {
            _notifier.StepWordAndStepNumberOnStart = StepWordAndStepNumberBehaviour.IncludeAsSuffix;
            var stepInfo = CreateStepInfo("Given some condition", 1, 3, "");
            _notifier.Notify(new StepStarting(new EventTime(), stepInfo));

            Assert.That(_captured.Single(), Is.EqualTo("  Given some condition... (STEP 1/3)"));
        }

        [Test]
        public void NotifyStepStart_with_excluded_numbering()
        {
            _notifier.StepWordAndStepNumberOnStart = StepWordAndStepNumberBehaviour.Exclude;
            var stepInfo = CreateStepInfo("Given some condition", 1, 3, "");
            _notifier.Notify(new StepStarting(new EventTime(), stepInfo));

            Assert.That(_captured.Single(), Is.EqualTo("  Given some condition..."));
        }

        [Test]
        public void NotifyStepStart_without_ellipsis()
        {
            _notifier.IncludeEllipsisAfterStep = false;
            var stepInfo = CreateStepInfo("Given some condition", 1, 3, "");
            _notifier.Notify(new StepStarting(new EventTime(), stepInfo));

            Assert.That(_captured.Single(), Is.EqualTo("  STEP 1/3: Given some condition"));
        }

        [Test]
        public void NotifyStepStart_without_final_step_count()
        {
            _notifier.ShowFinalStepWithEachStep = false;
            var stepInfo = CreateStepInfo("Given some condition", 2, 5, "");
            _notifier.Notify(new StepStarting(new EventTime(), stepInfo));

            Assert.That(_captured.Single(), Is.EqualTo("  STEP 2: Given some condition..."));
        }

        [Test]
        public void NotifyStepStart_should_include_group_prefix()
        {
            var stepInfo = CreateStepInfo("Given some condition", 2, 5, "1.");
            _notifier.Notify(new StepStarting(new EventTime(), stepInfo));

            Assert.That(_captured.Single(), Is.EqualTo("  STEP 1.2/1.5: Given some condition..."));
        }

        [Test]
        public void NotifyStepStart_without_final_step_count_and_with_group_prefix()
        {
            _notifier.ShowFinalStepWithEachStep = false;
            var stepInfo = CreateStepInfo("Given some condition", 2, 5, "1.");
            _notifier.Notify(new StepStarting(new EventTime(), stepInfo));

            Assert.That(_captured.Single(), Is.EqualTo("  STEP 1.2: Given some condition..."));
        }

        [Test]
        public void NotifyStepStart_suffix_without_ellipsis_and_without_final_step()
        {
            _notifier.StepWordAndStepNumberOnStart = StepWordAndStepNumberBehaviour.IncludeAsSuffix;
            _notifier.IncludeEllipsisAfterStep = false;
            _notifier.ShowFinalStepWithEachStep = false;
            var stepInfo = CreateStepInfo("Given some condition", 1, 3, "");
            _notifier.Notify(new StepStarting(new EventTime(), stepInfo));

            Assert.That(_captured.Single(), Is.EqualTo("  Given some condition (STEP 1)"));
        }

        #endregion

        #region Step finish customizations

        [Test]
        public void NotifyStepFinished_should_include_step_name_and_prefix_by_default()
        {
            var stepResult = CreateStepResult("Given some condition", 1, 3, "", ExecutionStatus.Passed);
            _notifier.Notify(new StepFinished(new EventTime(), stepResult));

            Assert.That(_captured.Single(), Is.EqualTo("  STEP 1/3: Given some condition (Passed after 125ms)"));
        }

        [Test]
        public void NotifyStepFinished_should_suppress_passed_when_WriteSuccessMessageForBasicSteps_is_false()
        {
            _notifier.WriteSuccessMessageForBasicSteps = false;
            var stepResult = CreateStepResult("Given some condition", 1, 3, "", ExecutionStatus.Passed);
            _notifier.Notify(new StepFinished(new EventTime(), stepResult));

            Assert.That(_captured, Is.Empty);
        }

        [Test]
        public void NotifyStepFinished_should_emit_failed_even_when_WriteSuccessMessageForBasicSteps_is_false()
        {
            _notifier.WriteSuccessMessageForBasicSteps = false;
            var stepResult = CreateStepResult("Given some condition", 1, 3, "", ExecutionStatus.Failed);
            _notifier.Notify(new StepFinished(new EventTime(), stepResult));

            Assert.That(_captured.Single(), Does.Contain("Failed after 125ms"));
        }

        [Test]
        public void NotifyStepFinished_should_emit_ignored_even_when_WriteSuccessMessageForBasicSteps_is_false()
        {
            _notifier.WriteSuccessMessageForBasicSteps = false;
            var stepResult = CreateStepResult("Given some condition", 1, 3, "", ExecutionStatus.Ignored);
            _notifier.Notify(new StepFinished(new EventTime(), stepResult));

            Assert.That(_captured.Single(), Does.Contain("Ignored after 125ms"));
        }

        [Test]
        public void NotifyStepFinished_should_emit_bypassed_even_when_WriteSuccessMessageForBasicSteps_is_false()
        {
            _notifier.WriteSuccessMessageForBasicSteps = false;
            var stepResult = CreateStepResult("Given some condition", 1, 3, "", ExecutionStatus.Bypassed);
            _notifier.Notify(new StepFinished(new EventTime(), stepResult));

            Assert.That(_captured.Single(), Does.Contain("Bypassed after 125ms"));
        }

        [Test]
        public void NotifyStepFinished_with_suffix_numbering()
        {
            _notifier.StepWordAndStepNumberOnFinish = StepWordAndStepNumberBehaviour.IncludeAsSuffix;
            var stepResult = CreateStepResult("Given some condition", 1, 3, "", ExecutionStatus.Failed);
            _notifier.Notify(new StepFinished(new EventTime(), stepResult));

            Assert.That(_captured.Single(), Is.EqualTo("  Given some condition  => (Failed after 125ms) (STEP 1/3)"));
        }

        [Test]
        public void NotifyStepFinished_with_excluded_numbering()
        {
            _notifier.StepWordAndStepNumberOnFinish = StepWordAndStepNumberBehaviour.Exclude;
            var stepResult = CreateStepResult("Given some condition", 1, 3, "", ExecutionStatus.Failed);
            _notifier.Notify(new StepFinished(new EventTime(), stepResult));

            Assert.That(_captured.Single(), Is.EqualTo("  Given some condition  => (Failed after 125ms)"));
        }

        [Test]
        public void NotifyStepFinished_without_step_name()
        {
            _notifier.IncludeStepNameOnFinish = false;
            var stepResult = CreateStepResult("Given some condition", 1, 3, "", ExecutionStatus.Failed);
            _notifier.Notify(new StepFinished(new EventTime(), stepResult));

            Assert.That(_captured.Single(), Is.EqualTo("  STEP 1/3: (Failed after 125ms)"));
        }

        [Test]
        public void NotifyStepFinished_without_step_name_and_suffix_numbering()
        {
            _notifier.IncludeStepNameOnFinish = false;
            _notifier.StepWordAndStepNumberOnFinish = StepWordAndStepNumberBehaviour.IncludeAsSuffix;
            var stepResult = CreateStepResult("Given some condition", 1, 3, "", ExecutionStatus.Failed);
            _notifier.Notify(new StepFinished(new EventTime(), stepResult));

            Assert.That(_captured.Single(), Is.EqualTo("    => (Failed after 125ms) (STEP 1/3)"));
        }

        [Test]
        public void NotifyStepFinished_should_report_tabular_and_tree_parameters()
        {
            var stepResult = CreateStepResultWithParameters();
            _notifier.Notify(new StepFinished(new EventTime(), stepResult));

            var expectedTable = @"    table:
    +-+---+----------+----------+
    |#|Key|Value1    |Value2    |
    +-+---+----------+----------+
    |=|1  |abc       |some value|
    |!|2  |def       |val/value |
    |-|3  |<null>/XXX|<null>/YYY|
    |+|4  |XXX/<null>|YYY/<null>|
    +-+---+----------+----------+"
                .Replace("\r", "")
                .Replace("\n", Environment.NewLine);

            var expectedTree = @"    tree:
    = $: <object>
    = $.Items: <array:1>
    ! $.Items[0]: False/True
    = $.Name: Bob
    ! $.Surname: Johnson/<none>
    ! $.LastName: <none>/Johnson"
                .Replace("\r", "")
                .Replace("\n", Environment.NewLine);

            var fullMessage = _captured.Single();

            Assert.That(fullMessage, Does.StartWith("  STEP 1/3: Given some condition (Failed after 125ms)"));
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

            var fullMessage = _captured.Single();
            Assert.That(fullMessage, Does.Not.Contain("Passed after"));
            Assert.That(fullMessage, Does.Contain("table:"));
            Assert.That(fullMessage, Does.Contain("tree:"));
        }

        #endregion

        #region Step comment and file attachment customizations

        [Test]
        public void NotifyStepComment_should_use_prefix_style_by_default()
        {
            var stepInfo = CreateStepInfo("Given some condition", 1, 3, "");
            _notifier.Notify(new StepCommented(new EventTime(), stepInfo, "some comment"));

            Assert.That(_captured.Single(), Is.EqualTo("  STEP 1/3: /* some comment */"));
        }

        [Test]
        public void NotifyStepComment_should_use_arrow_style_when_suffix_numbering()
        {
            _notifier.StepWordAndStepNumberOnStart = StepWordAndStepNumberBehaviour.IncludeAsSuffix;
            var stepInfo = CreateStepInfo("Given some condition", 1, 3, "");
            _notifier.Notify(new StepCommented(new EventTime(), stepInfo, "some comment"));

            Assert.That(_captured.Single(), Is.EqualTo("    => /* some comment */"));
        }

        [Test]
        public void NotifyStepFileAttached_should_use_prefix_style_by_default()
        {
            var stepInfo = CreateStepInfo("Given some condition", 1, 3, "");
            var attachment = new FileAttachment("screenshot", "/path/to/file.png", "file.png");
            _notifier.Notify(new StepFileAttached(new EventTime(), stepInfo, attachment));

            Assert.That(_captured.Single(), Is.EqualTo("  STEP 1/3: \U0001F517screenshot: /path/to/file.png"));
        }

        [Test]
        public void NotifyStepFileAttached_should_use_arrow_style_when_suffix_numbering()
        {
            _notifier.StepWordAndStepNumberOnStart = StepWordAndStepNumberBehaviour.IncludeAsSuffix;
            var stepInfo = CreateStepInfo("Given some condition", 1, 3, "");
            var attachment = new FileAttachment("screenshot", "/path/to/file.png", "file.png");
            _notifier.Notify(new StepFileAttached(new EventTime(), stepInfo, attachment));

            Assert.That(_captured.Single(), Is.EqualTo("    => \U0001F517screenshot: /path/to/file.png"));
        }

        #endregion

        #region Blank line customizations

        [Test]
        public void NotifyFeatureStart_should_not_write_blank_line_by_default()
        {
            var featureInfo = new TestResults.TestFeatureInfo
            {
                Name = TestResults.CreateNameInfo("Shopping cart"),
                Labels = new[] { "Story-1" },
                Description = null
            };
            _notifier.Notify(new FeatureStarting(new EventTime(), featureInfo));

            Assert.That(_captured.ToArray(), Is.EqualTo(new[] { "FEATURE: [Story-1] Shopping cart" }));
        }

        [Test]
        public void NotifyFeatureStart_should_write_blank_line_when_configured()
        {
            _notifier.WriteBlankLineAfterFeatureStart = true;
            var featureInfo = new TestResults.TestFeatureInfo
            {
                Name = TestResults.CreateNameInfo("Shopping cart"),
                Labels = new[] { "Story-1" },
                Description = null
            };
            _notifier.Notify(new FeatureStarting(new EventTime(), featureInfo));

            Assert.That(_captured.ToArray(), Is.EqualTo(new[] { "FEATURE: [Story-1] Shopping cart", "" }));
        }

        [Test]
        public void NotifyScenarioStart_should_not_write_blank_line_by_default()
        {
            var scenarioInfo = new TestResults.TestScenarioInfo
            {
                Name = TestResults.CreateNameInfo("Adding items"),
                Labels = new[] { "Ticket-42" },
                Categories = Array.Empty<string>(),
                Description = null
            };
            _notifier.Notify(new ScenarioStarting(new EventTime(), scenarioInfo));

            Assert.That(_captured.ToArray(), Is.EqualTo(new[] { "SCENARIO: [Ticket-42] Adding items" }));
        }

        [Test]
        public void NotifyScenarioStart_should_write_blank_line_when_configured()
        {
            _notifier.WriteBlankLineAfterScenarioStart = true;
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
        public void NotifyScenarioFinished_should_not_write_blank_lines_by_default()
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

            Assert.That(_captured.ToArray(), Is.EqualTo(new[] { "  SCENARIO RESULT: Passed after 250ms" }));
        }

        [Test]
        public void NotifyScenarioFinished_should_write_blank_lines_when_configured()
        {
            _notifier.WriteBlankLinesAroundScenarioFinish = true;
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

            Assert.That(_captured.ToArray(), Is.EqualTo(new[] { "", "  SCENARIO RESULT: Passed after 250ms", "" }));
        }

        #endregion

        #region IndentLength customizations

        [Test]
        public void Steps_should_use_flat_two_space_indent_by_default()
        {
            var stepInfo = CreateStepInfo("Given some condition", 1, 3, "");
            _notifier.Notify(new StepStarting(new EventTime(), stepInfo));

            Assert.That(_captured.Single(), Does.StartWith("  STEP"));
        }

        [Test]
        public void Steps_should_use_hierarchical_indent_when_IndentLength_is_positive()
        {
            _notifier.IndentLength = 4;
            _notifier.StepWordAndStepNumberOnStart = StepWordAndStepNumberBehaviour.IncludeAsSuffix;
            _notifier.IncludeEllipsisAfterStep = false;
            var stepInfo = CreateStepInfo("Given some condition", 1, 3, "");
            _notifier.Notify(new StepStarting(new EventTime(), stepInfo));

            // IndentLength=4 => base indent is 4 spaces
            Assert.That(_captured.Single(), Is.EqualTo("    Given some condition (STEP 1/3)"));
        }

        [Test]
        public void Sub_steps_should_be_further_indented_when_IndentLength_is_positive()
        {
            _notifier.IndentLength = 4;
            _notifier.StepWordAndStepNumberOnStart = StepWordAndStepNumberBehaviour.IncludeAsSuffix;
            _notifier.IncludeEllipsisAfterStep = false;
            _notifier.ShowFinalStepWithEachStep = false;

            var parentStep = CreateStepInfo("Given a parent step", 1, 3, "");
            var childStep = CreateStepInfo("And a child step", 1, 2, "1.");
            childStep.Parent = parentStep;

            _notifier.Notify(new StepStarting(new EventTime(), childStep));

            // Parent level (4) + base level (4) = 8 spaces
            Assert.That(_captured.Single(), Is.EqualTo("        And a child step (STEP 1.1)"));
        }

        [Test]
        public void Deeply_nested_steps_should_increase_indentation()
        {
            _notifier.IndentLength = 4;
            _notifier.StepWordAndStepNumberOnStart = StepWordAndStepNumberBehaviour.IncludeAsSuffix;
            _notifier.IncludeEllipsisAfterStep = false;
            _notifier.ShowFinalStepWithEachStep = false;

            var grandparent = CreateStepInfo("Given a grandparent", 1, 3, "");
            var parent = CreateStepInfo("And a parent", 1, 2, "1.");
            parent.Parent = grandparent;
            var child = CreateStepInfo("And a child", 1, 1, "1.1.");
            child.Parent = parent;

            _notifier.Notify(new StepStarting(new EventTime(), child));

            // grandparent (4) + parent (4) + base (4) = 12 spaces
            Assert.That(_captured.Single(), Does.StartWith("            And a child"));
        }

        [Test]
        public void Flat_indent_should_not_increase_for_nested_steps()
        {
            // Default IndentLength=0, so nested steps still get flat "  " indent
            var parentStep = CreateStepInfo("Given a parent step", 1, 3, "");
            var childStep = CreateStepInfo("And a child step", 1, 2, "1.");
            childStep.Parent = parentStep;

            _notifier.Notify(new StepStarting(new EventTime(), parentStep));
            _notifier.Notify(new StepStarting(new EventTime(), childStep));

            Assert.That(_captured.ElementAt(0), Does.StartWith("  STEP 1/3:"));
            Assert.That(_captured.ElementAt(1), Does.StartWith("  STEP 1.1/1.2:"));
        }

        [Test]
        public void FeatureStart_should_use_double_space_pad_when_IndentLength_is_positive()
        {
            _notifier.IndentLength = 4;
            var featureInfo = new TestResults.TestFeatureInfo
            {
                Name = TestResults.CreateNameInfo("Shopping cart"),
                Labels = new[] { "Story-1" },
                Description = null
            };
            _notifier.Notify(new FeatureStarting(new EventTime(), featureInfo));

            // IndentLength > 0 uses "FEATURE:  " (two spaces)
            Assert.That(_captured.Single(), Is.EqualTo("FEATURE:  [Story-1] Shopping cart"));
        }

        [Test]
        public void FeatureStart_should_use_single_space_pad_when_IndentLength_is_zero()
        {
            var featureInfo = new TestResults.TestFeatureInfo
            {
                Name = TestResults.CreateNameInfo("Shopping cart"),
                Labels = new[] { "Story-1" },
                Description = null
            };
            _notifier.Notify(new FeatureStarting(new EventTime(), featureInfo));

            // Default IndentLength=0 uses "FEATURE: " (one space)
            Assert.That(_captured.Single(), Is.EqualTo("FEATURE: [Story-1] Shopping cart"));
        }

        [Test]
        public void ScenarioResult_should_have_two_space_indent_when_IndentLength_is_zero()
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

            Assert.That(_captured.Single(), Does.StartWith("  SCENARIO RESULT:"));
        }

        [Test]
        public void ScenarioResult_should_have_no_indent_when_IndentLength_is_positive()
        {
            _notifier.IndentLength = 4;
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

            Assert.That(_captured.Single(), Is.EqualTo("SCENARIO RESULT: Passed after 250ms"));
        }

        [Test]
        public void Description_should_use_extended_indent_when_IndentLength_is_positive()
        {
            _notifier.IndentLength = 4;
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
            Assert.That(_captured.Single(), Is.EqualTo(expected));
        }

        [Test]
        public void Description_should_use_two_space_indent_when_IndentLength_is_zero()
        {
            var featureInfo = new TestResults.TestFeatureInfo
            {
                Name = TestResults.CreateNameInfo("Shopping cart"),
                Labels = Array.Empty<string>(),
                Description = "As a customer I want to manage my cart"
            };
            _notifier.Notify(new FeatureStarting(new EventTime(), featureInfo));

            var expected = $"FEATURE: Shopping cart{Environment.NewLine}  As a customer I want to manage my cart";
            Assert.That(_captured.Single(), Is.EqualTo(expected));
        }

        #endregion

        #region Full integration: configured like SimpleIndentedProgressNotifier

        [Test]
        public void It_should_match_SimpleIndentedProgressNotifier_when_configured_equivalently()
        {
            // Configure DefaultProgressNotifier to match SimpleIndentedProgressNotifier defaults
            _notifier.WriteSuccessMessageForBasicSteps = false;
            _notifier.ShowFinalStepWithEachStep = false;
            _notifier.IndentLength = 4;
            _notifier.StepWordAndStepNumberOnStart = StepWordAndStepNumberBehaviour.IncludeAsSuffix;
            _notifier.StepWordAndStepNumberOnFinish = StepWordAndStepNumberBehaviour.IncludeAsSuffix;
            _notifier.IncludeStepNameOnFinish = false;
            _notifier.IncludeEllipsisAfterStep = false;
            _notifier.WriteBlankLineAfterFeatureStart = true;
            _notifier.WriteBlankLineAfterScenarioStart = true;
            _notifier.WriteBlankLinesAroundScenarioFinish = true;

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

        private TestResults.TestStepResult CreateStepResultWithParameters()
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

        private static string NormalizeNewlines(string text) => text.Replace("\r\n", "\n").Replace("\n", Environment.NewLine);

        #endregion
    }
}