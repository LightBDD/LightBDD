namespace LightBDD.Core.Metadata.Implementation
{
    internal class StepTypeNameInfo : IStepTypeNameInfo
    {
        public StepTypeNameInfo(string name, string originalName)
        {
            Name = name;
            OriginalName = originalName;
        }

        public string Name { get; }
        public string OriginalName { get; }

        public override string ToString()
        {
            return Name;
        }
    }
}