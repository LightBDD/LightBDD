using LightBDD.Core.Configuration;

namespace LightBDD.Core.Extensibility.Execution
{
    /// <summary>
    /// Attribute interface allowing to enhance step execution with additional logic.
    /// Decorators will be executed in order specified by <see cref="IOrderedAttribute.Order"/> property, after globally registered step decorators with <see cref="ServiceCollectionExtensions.ConfigureStepDecorators"/>.
    /// </summary>
    public interface IStepDecoratorAttribute : IStepDecorator, IOrderedAttribute
    {
    }
}