namespace LightBDD.Core.Extensibility.Results
{
    /// <summary>
    /// Default step result descriptor, bringing no additional meaning for the step.
    /// </summary>
    public class DefaultStepResultDescriptor : IStepResultDescriptor
    {
        /// <summary>
        /// Instance that should be used if <see cref="DefaultStepResultDescriptor"/> is expected to be returned.
        /// </summary>
        public static readonly IStepResultDescriptor Instance = new DefaultStepResultDescriptor();
        private DefaultStepResultDescriptor() { }
    }
}