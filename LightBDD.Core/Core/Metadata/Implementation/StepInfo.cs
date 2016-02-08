namespace LightBDD.Core.Metadata.Implementation
{
    internal class StepInfo : IStepInfo
    {
        public StepInfo(IStepNameInfo name, int number)
        {
            Name = name;
            Number = number;
        }

        public int Number { get; private set; }
        public IStepNameInfo Name { get; private set; }

        public void UpdateName(INameParameterInfo[] parameters)
        {
            Name = StepNameInfo.WithUpdatedParameters(Name, parameters);
        }
    }
}