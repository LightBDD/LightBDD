using System.Diagnostics;

namespace LightBDD.Naming
{
    /// <summary>
    /// Class providing step name decorator instances.
    /// </summary>
    [DebuggerStepThrough]
    public static class StepNameDecorators
    {
        /// <summary>
        /// Default step name decorator, where:
        /// * step type is returned intact if not null, or string.Empty is returned,
        /// * parameter value is returned intact if not null, or string.Empty is returned.
        /// </summary>
        public static readonly IStepNameDecorator Default = new DefaultStepNameDecorator();
    }
}