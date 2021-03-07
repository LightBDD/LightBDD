using System;
using System.Collections.Generic;
using LightBDD.Notification.Jsonl.Events;

namespace LightBDD.Reporting.Progressive.UI.Models
{
    internal class ScenarioModel : IScenarioModel
    {
        private readonly List<StepModel> _steps = new List<StepModel>();
        private readonly ScenarioDiscovered _meta;
        private readonly NameInfo _name;

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

        public IReadOnlyList<IStepModel> Steps => _steps;
        public void AddStep(StepModel sub)
        {
            _steps.Add(sub);
            OnChange?.Invoke();
        }

        public event Action OnChange;
    }
}