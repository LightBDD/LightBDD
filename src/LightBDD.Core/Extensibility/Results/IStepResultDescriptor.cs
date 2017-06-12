namespace LightBDD.Core.Extensibility.Results
{
    /// <summary>
    /// A marker interface describing step execution result.
    /// Each step executed by LightBDD engine is expected to return instance implementing this interface.
    /// LightBDD.Core offers few types that implement this interface, enhancing step method with additional behavior.
    /// </summary>
    public interface IStepResultDescriptor
    {
    }
}