using LightBDD.Core.Configuration;

namespace LightBDD.Core.Extensibility.Execution
{
    /// <summary>
    /// Attribute interface allowing to enhance scenario execution with additional logic.
    /// Decorators will be executed in order specified by <see cref="IOrderedAttribute.Order"/> property, after globally registered scenario decorators with <see cref="ExecutionExtensionsConfiguration"/>.
    /// </summary>
    public interface IScenarioDecoratorAttribute : IScenarioDecorator, IOrderedAttribute
    {
    }
}