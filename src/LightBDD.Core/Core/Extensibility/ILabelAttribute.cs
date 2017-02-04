namespace LightBDD.Core.Extensibility
{
    /// <summary>
    /// Label attribute interface that can be applied on feature test class or scenario method.
    /// May be used to link feature/scenario with external tools by storing ticket number.
    /// Multiple labels per item are supported.
    /// </summary>
    public interface ILabelAttribute
    {
        /// <summary>
        /// Specified label.
        /// </summary>
        string Label { get; }
    }
}