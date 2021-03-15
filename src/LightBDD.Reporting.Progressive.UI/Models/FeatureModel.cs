using System;
using System.Collections.Generic;
using LightBDD.Notification.Jsonl.Events;
using LightBDD.Notification.Jsonl.Models;

namespace LightBDD.Reporting.Progressive.UI.Models
{
    internal class FeatureModel : IFeatureModel
    {
        private readonly List<IScenarioModel> _scenarios = new List<IScenarioModel>();
        private readonly FeatureDiscovered _meta;
        private readonly NameInfo _name;
        private FeatureStarting _start;
        private FeatureFinished _finish;

        public FeatureModel(FeatureDiscovered meta)
        {
            _meta = meta;
            _name = new NameInfo(_meta.Name);
        }

        public Guid Id => _meta.Id;
        public string Description => _meta.Description;
        public IReadOnlyList<string> Labels => _meta.Labels;
        public INameInfo Name => _name;
        public IReadOnlyList<IScenarioModel> Scenarios => _scenarios;

        public ExecutionStatus Status => _finish?.Status ?? (_start != null ? ExecutionStatus.Running : ExecutionStatus.NotRun);
        public TimeSpan? ExecutionTime => (_start != null && _finish != null) ? _finish.Time - _start.Time : null;

        public event Action OnChange;

        public void AddScenario(IScenarioModel scenario)
        {
            _scenarios.Add(scenario);
            OnChange?.Invoke();
        }


        public void OnStart(FeatureStarting start)
        {
            _start = start;
            OnChange?.Invoke();
        }

        public void OnFinish(FeatureFinished finish)
        {
            _finish = finish;
            OnChange?.Invoke();
        }
    }
}