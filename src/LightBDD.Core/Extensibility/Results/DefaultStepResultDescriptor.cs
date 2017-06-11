namespace LightBDD.Core.Extensibility.Results
{
    public class DefaultStepResultDescriptor : IStepResultDescriptor
    {
        public static readonly IStepResultDescriptor Instance = new DefaultStepResultDescriptor();
    }
}