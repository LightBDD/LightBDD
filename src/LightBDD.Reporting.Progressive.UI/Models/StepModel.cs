using System;
using System.Collections.Generic;
using LightBDD.Notification.Jsonl.Events;

namespace LightBDD.Reporting.Progressive.UI.Models
{
    internal class StepModel : IStepModel
    {
        private readonly List<StepModel> _subSteps = new List<StepModel>();
        private readonly StepDiscovered _meta;
        private readonly StepNameInfo _name;

        public StepModel(StepDiscovered meta)
        {
            _meta = meta;
            _name = new StepNameInfo(meta.Name);
        }

        public Guid Id => _meta.Id;
        public Guid ParentId => _meta.ParentId;
        public string GroupPrefix => _meta.GroupPrefix;
        public int Number => _meta.Number;
        public IStepNameInfo Name => _name;
        public IReadOnlyList<IStepModel> SubSteps => _subSteps;
        public void Add(StepModel sub) => _subSteps.Add(sub);
    }
}