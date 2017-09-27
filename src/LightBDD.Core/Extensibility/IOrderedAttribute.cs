namespace LightBDD.Core.Extensibility
{
    public interface IOrderedAttribute
    {
        /// <summary>
        /// Order in which attributes should be applied, where instances of lower values will be applied first.
        /// </summary>
        int Order { get; }
    }
}