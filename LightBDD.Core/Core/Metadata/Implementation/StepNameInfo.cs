namespace LightBDD.Core.Metadata.Implementation
{
    internal class StepNameInfo : IStepNameInfo
    {
        public StepNameInfo(string stepTypeName, string name)
        {
            StepTypeName = stepTypeName;
            Name = name;
        }

        public string StepTypeName { get; private set; }
        public string Name { get; private set; }

        public override string ToString()
        {
            return (StepTypeName != null) ? string.Format("{0} {1}", StepTypeName, Name) : Name;
        }
    }
}