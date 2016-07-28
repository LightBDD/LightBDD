namespace LightBDD.Core.Metadata.Implementation
{
    internal class StepInfo : IStepInfo
    {
        public StepInfo(IStepNameInfo name, int number, int total)
        {
            Name = name;
            Number = number;
            Total = total;
        }

        public int Number { get; }
        public int Total { get; }
        public IStepNameInfo Name { get; private set; }

        public void UpdateName(INameParameterInfo[] parameters)
        {
            Name = StepNameInfo.WithUpdatedParameters(Name, parameters);
        }

        public override string ToString()
        {
            return $"{Number}/{Total} {Name}";
        }
    }
}