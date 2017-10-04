namespace LightBDD.Core.Extensibility
{
    /// <summary>
    /// Interface defining <see cref="Order"/> property that for attributes that are allowed to be applied multiple times but their order matters.
    /// </summary>
    public interface IOrderedAttribute
    {
        /// <summary>
        /// Order in which attributes should be applied, where instances of lower values will be applied first.
        /// </summary>
        int Order { get; }
    }
}