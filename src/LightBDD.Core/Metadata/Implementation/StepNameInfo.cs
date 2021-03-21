using System;
using System.Collections.Generic;
using System.Linq;
using LightBDD.Core.Formatting.NameDecorators;

namespace LightBDD.Core.Metadata.Implementation
{
    internal class StepNameInfo : NameInfo, IStepNameInfo
    {
        public StepNameInfo(IStepTypeNameInfo stepTypeName, string nameFormat, IReadOnlyList<INameParameterInfo> parameters)
            : base(nameFormat, parameters)
        {
            StepTypeName = stepTypeName;
        }

        public IStepTypeNameInfo StepTypeName { get; }

        public override string ToString()
        {
            return Format(StepNameDecorators.Default);
        }

        public string Format(IStepNameDecorator decorator)
        {
            return StepTypeName != null
                ? $"{decorator.DecorateStepTypeName(StepTypeName)} {base.Format(decorator)}"
                : base.Format(decorator);
        }
    }
}