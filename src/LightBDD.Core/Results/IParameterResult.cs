using System.Collections.Generic;

namespace LightBDD.Core.Results
{
    /// <summary>
    /// TODO
    /// </summary>
    public interface IParameterResult
    {
        /// <summary>
        /// TODO
        /// </summary>
        string Name { get; }
        /// <summary>
        /// TODO
        /// </summary>
        bool IsContainer { get; }
        /// <summary>
        /// TODO
        /// </summary>
        IValueResult Value { get; }
        /// <summary>
        /// TODO
        /// </summary>
        IEnumerable<IParameterResult> Items { get; }
    }
}