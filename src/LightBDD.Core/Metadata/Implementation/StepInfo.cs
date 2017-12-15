using System.Diagnostics;

namespace LightBDD.Core.Metadata.Implementation
{
    [DebuggerStepThrough]
    internal class StepInfo : IStepInfo
    {
        public StepInfo(string runtimeId, IStepNameInfo name, int number, int total, string groupPrefix)
        {
            RuntimeId = runtimeId;
            Name = name;
            Number = number;
            Total = total;
            GroupPrefix = groupPrefix;
        }

        public string GroupPrefix { get; }
        public int Number { get; }
        public int Total { get; }
        public string RuntimeId { get; }
        public IStepNameInfo Name { get; private set; }

        public void UpdateName(INameParameterInfo[] parameters)
        {
            Name = StepNameInfo.WithUpdatedParameters(Name, parameters);
        }

        public override string ToString()
        {
            return $"{GroupPrefix}{Number}/{GroupPrefix}{Total} {Name}";
        }
    }
}