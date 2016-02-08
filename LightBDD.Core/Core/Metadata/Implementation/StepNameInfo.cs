using System;
using System.Linq;

namespace LightBDD.Core.Metadata.Implementation
{
    internal class StepNameInfo : NameInfo, IStepNameInfo
    {
        public StepNameInfo(string stepTypeName, string nameFormat, INameParameterInfo[] parameters)
            : base(nameFormat, parameters)
        {
            StepTypeName = stepTypeName;
        }

        public static StepNameInfo WithUpdatedParameters(IStepNameInfo nameInfo, INameParameterInfo[] parameters)
        {
            if (nameInfo.Parameters.Count() != parameters.Length)
                throw new ArgumentException("StepNameInfo cannot be updated with different number of parameters");
            return new StepNameInfo(nameInfo.StepTypeName, nameInfo.NameFormat, parameters);
        }

        public string StepTypeName { get; private set; }

        public override string ToString()
        {
            return (StepTypeName != null)
                ? string.Format("{0} {1}", StepTypeName, base.ToString())
                : base.ToString();
        }
    }
}