using System;
using System.Collections.Generic;
using LightBDD.Notification.Jsonl.Events;
using LightBDD.Notification.Jsonl.Models;

namespace LightBDD.Reporting.Progressive.UI.Models
{
    internal class ScenarioModel : IScenarioModel
    {
        private readonly List<StepModel> _steps = new List<StepModel>();
        private readonly ScenarioDiscovered _meta;
        private readonly NameInfo _name;
        private ScenarioStarting _start;
        private ScenarioFinished _finish;

        public ScenarioModel(ScenarioDiscovered meta)
        {
            _meta = meta;
            _name = new NameInfo(meta.Name);
        }

        public Guid Id => _meta.Id;
        public Guid FeatureId => _meta.ParentId;
        public IReadOnlyList<string> Categories => _meta.Categories;
        public IReadOnlyList<string> Labels => _meta.Labels;
        public INameInfo Name => _name;
        public ExecutionStatus Status => _finish?.Status ?? (_start != null ? ExecutionStatus.Running : ExecutionStatus.NotRun);
        public string StatusDetails => _finish?.StatusDetails;
        public TimeSpan? ExecutionTime => (_start != null && _finish != null) ? _finish.Time - _start.Time : null;
        public IReadOnlyList<IStepModel> Steps => _steps;

        public event Action OnChange;

        public void AddStep(StepModel sub)
        {
            _steps.Add(sub);
            OnChange?.Invoke();
        }

        public void OnStart(ScenarioStarting start)
        {
            _start = start;
            OnChange?.Invoke();
        }

        public void OnFinish(ScenarioFinished finish)
        {
            _finish = finish; 
            OnChange?.Invoke();
        }
    }
}