using System;
using LightBDD.Core.Results;

namespace LightBDD.Core.Execution
{
    /// <summary>
    /// Exception describing intention to bypass currently executed stage (step or scenario) resulting with the current stage status being set to <see cref="ExecutionStatus.Bypassed"/>, but continue execution of the parent stages (steps or scenario).
    /// </summary>
    public class BypassException : Exception
    {
        /// <summary>
        /// Constructor allowing to specify bypass reason.
        /// </summary>
        /// <param name="reason">Bypass reason.</param>
        public BypassException(string reason) : base(reason) { }
    }
}