using System;
using System.Collections.Generic;
using LightBDD.Notification.Jsonl.Events;

namespace LightBDD.Reporting.Progressive.UI.Models
{
    internal class FeatureModel : IFeatureModel
    {
        private readonly List<IScenarioModel> _scenarios = new List<IScenarioModel>();
        private readonly FeatureDiscovered _meta;
        private readonly NameInfo _name;

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

        public void AddScenario(IScenarioModel scenario)
        {
            _scenarios.Add(scenario);
            OnChange?.Invoke();
        }

        public event Action OnChange;
    }
}