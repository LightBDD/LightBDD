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

        public static StepNameInfo WithUpdatedParameters(IStepNameInfo nameInfo, IReadOnlyList<INameParameterInfo> parameters)
        {
            if (nameInfo.Parameters.Count() != parameters.Count)
                throw new ArgumentException("StepNameInfo cannot be updated with different number of parameters");
            return new StepNameInfo(nameInfo.StepTypeName, nameInfo.NameFormat, parameters);
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