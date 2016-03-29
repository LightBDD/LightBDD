using System.Collections.Generic;
using System.Threading;
using LightBDD.Core.Execution.Results;
using LightBDD.Core.Formatting;
using LightBDD.Core.Metadata;

namespace LightBDD.Core.Notification
{
    public abstract class ParallelProgressNotifier : IProgressNotifier
    {
        private readonly ProgressManager _progressManager;

        protected ParallelProgressNotifier(ProgressManager progressManager)
        {
            _progressManager = progressManager;
        }

        public virtual void NotifyFeatureStart(IFeatureInfo feature)
        {
            NotifyProgress($"FEATURE: {FormatLabels(feature.Labels)}{feature.Name}{FormatDescription(feature.Description)}");
        }

        public virtual void NotifyFeatureFinished(IFeatureResult feature)
        {
            NotifyProgress($"FEATURE FINISHED: {feature.Info.Name}");
        }

        public virtual void NotifyScenarioStart(IScenarioInfo scenario)
        {
            _progressManager.StartNewScenario();
            NotifyProgress($"SCENARIO: {FormatLabels(scenario.Labels)}{scenario.Name}");
        }

        public virtual void NotifyScenarioFinished(IScenarioResult scenario)
        {
            _progressManager.CaptureScenarioResult(scenario.Status);
            var scenarioText = scenario.ExecutionTime != null
                ? $"  SCENARIO RESULT: {scenario.Status} after {scenario.ExecutionTime.Duration.FormatPretty()}"
                : $"  SCENARIO RESULT: {scenario.Status}";

            var scenarioDetails = !string.IsNullOrWhiteSpace(scenario.StatusDetails)
                ? $"\n    {scenario.StatusDetails.Replace("\n", "\n    ")}"
                : string.Empty;

            NotifyProgress(scenarioText + scenarioDetails);
            _progressManager.FinishScenario();
        }

        public virtual void NotifyStepStart(IStepInfo step)
        {
            NotifyProgress($"  STEP {step.Number}/{step.Total}: {step.Name}...");
        }

        public virtual void NotifyStepFinished(IStepResult step)
        {
            NotifyProgress($"  STEP {step.Info.Number}/{step.Info.Total}: {step.Info.Name} ({step.Status} after {step.ExecutionTime.Duration.FormatPretty()})");
        }

        public virtual void NotifyStepComment(IStepInfo step, string comment)
        {
            NotifyProgress($"  STEP {step.Number}/{step.Total}: /* {comment} */");
        }

        protected void NotifyProgress(string message)
        {
            var progress = _progressManager.GetProgress();
            var header = $"Fi={progress.FinishedScenarios:D3},Fa={progress.FailedScenarios:D3},Pe={progress.PendingScenarios:D3} #{_progressManager.CurrentScenarioNo,3}> ";

            Notify(header + message.Replace("\n", "\n" + new string(' ', header.Length)));
        }

        protected abstract void Notify(string message);

        private static string FormatDescription(string description)
        {
            return string.IsNullOrWhiteSpace(description)
                ? string.Empty
                : $"\n  {description.Replace("\n", "\n  ")}";
        }

        private static string FormatLabels(IEnumerable<string> labels)
        {
            var joinedLabels = string.Join("][", labels);
            if (joinedLabels != string.Empty)
                joinedLabels = "[" + joinedLabels + "] ";
            return joinedLabels;
        }

        #region Inner classes
        public struct ProgressState
        {
            public ProgressState(int finishedScenarios, int pendingScenarios, int failedScenarios)
            {
                FinishedScenarios = finishedScenarios;
                PendingScenarios = pendingScenarios;
                FailedScenarios = failedScenarios;
            }

            public int FinishedScenarios { get; }
            public int PendingScenarios { get; }
            public int FailedScenarios { get; }
        }

        public class ProgressManager
        {
            public static readonly ProgressManager Instance = new ProgressManager();
            private readonly AsyncLocal<int?> _currentScenario = new AsyncLocal<int?>();
            private readonly object _sync = new object();
            private int _totalScenarios;
            private int _finishedScenarios;
            private int _pendingScenarios;
            private int _failedScenarios;

            public int? CurrentScenarioNo => _currentScenario.Value;
            public ProgressState GetProgress()
            {
                lock (_sync)
                    return new ProgressState(_finishedScenarios, _pendingScenarios, _failedScenarios);
            }

            public void StartNewScenario()
            {
                _currentScenario.Value = Interlocked.Increment(ref _totalScenarios);
                lock (_sync)
                    ++_pendingScenarios;
            }

            public void CaptureScenarioResult(ExecutionStatus scenarioStatus)
            {
                lock (_sync)
                {
                    ++_finishedScenarios;
                    if (scenarioStatus == ExecutionStatus.Failed)
                        ++_failedScenarios;
                    --_pendingScenarios;
                }
            }

            public void FinishScenario()
            {
                _currentScenario.Value = null;
            }
        }
        #endregion
    }
}