using LightBDD.Core.Configuration;

namespace LightBDD.Core.Extensibility.Execution
{
    /// <summary>
    /// Attribute interface allowing to enhance step execution with additional logic.
    /// Decorators will be executed in order specified by <see cref="Order"/> property, after globally registered step decorators with <see cref="ExecutionExtensionsConfiguration"/>.
    /// </summary>
    public interface IStepDecoratorAttribute : IStepDecorator
    {
        /// <summary>
        /// Order in which decorators should be applied, where instances of lower values will be executed first.
        /// </summary>
        int Order { get; set; }
    }
}