namespace LightBDD.Results
{
    /// <summary>
    /// Interface describing step parameter.
    /// </summary>
    public interface IStepParameter
    {
        /// <summary>
        /// True if parameter value has been evaluated.
        /// </summary>
        bool IsEvaluated { get; }
        /// <summary>
        /// Formatted parameter value (with InvariantCulture), or &lt;?&gt; if parameter has not been evaluated.
        /// </summary>
        string FormattedValue { get; }
    }
}