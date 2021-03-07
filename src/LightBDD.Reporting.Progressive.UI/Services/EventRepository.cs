using System;
using System.Collections.Generic;
using LightBDD.Notification.Jsonl.Events;
using LightBDD.Reporting.Progressive.UI.Models;

namespace LightBDD.Reporting.Progressive.UI.Services
{
    public class EventRepository
    {
        private readonly Dictionary<Type, Action<Event>> _handlers = new Dictionary<Type, Action<Event>>();
        private readonly Dictionary<Guid, FeatureModel> _features = new Dictionary<Guid, FeatureModel>();
        private readonly Dictionary<Guid, ScenarioModel> _scenarios = new Dictionary<Guid, ScenarioModel>();
        private readonly Dictionary<Guid, StepModel> _steps = new Dictionary<Guid, StepModel>();
        public Action OnUpdate { get; set; }

        public IEnumerable<IFeatureModel> Features => _features.Values;
        public IEnumerable<IScenarioModel> Scenarios => _scenarios.Values;

        public EventRepository()
        {
            Register<FeatureDiscovered>(OnFeatureDiscovered);
            Register<ScenarioDiscovered>(OnScenarioDiscovered);
            Register<StepDiscovered>(OnStepDiscovered);
        }

        private void OnStepDiscovered(StepDiscovered sd)
        {
            var model = new StepModel(sd);
            _steps.Add(sd.Id, model);

            if (_scenarios.TryGetValue(model.ParentId, out var scenario))
                scenario.AddStep(model);
            else _steps[model.ParentId].AddSubStep(model);
        }

        private void OnScenarioDiscovered(ScenarioDiscovered sd)
        {
            var model = new ScenarioModel(sd);
            _scenarios.Add(sd.Id, model);
            _features[model.FeatureId].AddScenario(model);
        }

        public void Add(Event e)
        {
            if (!_handlers.TryGetValue(e.GetType(), out var handler))
                return;
            handler.Invoke(e);
            OnUpdate?.Invoke();
        }

        private void OnFeatureDiscovered(FeatureDiscovered fd)
        {
            _features.Add(fd.Id, new FeatureModel(fd));
        }

        private void Register<T>(Action<T> handle) where T : Event => _handlers.Add(typeof(T), e => handle.Invoke((T)e));
    }
}