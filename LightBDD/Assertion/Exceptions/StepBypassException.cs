using System;

namespace LightBDD.Assertion.Exceptions
{
    /// <summary>
    /// Step bypass exception used to mark step bypassed
    /// </summary>
    public class StepBypassException : Exception
    {
        /// <summary>
        /// Constructor allowing to specify bypass reason.
        /// </summary>
        /// <param name="reason">Bypass reason.</param>
        public StepBypassException(string reason) : base(reason) { }
    }
}