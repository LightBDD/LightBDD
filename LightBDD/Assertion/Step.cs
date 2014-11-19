using LightBDD.Assertion.Exceptions;

namespace LightBDD.Assertion
{
    /// <summary>
    /// TBD
    /// </summary>
    public static class Step
    {
        /// <summary>
        /// TBD
        /// </summary>
        /// <param name="reason"></param>
        /// <exception cref="StepBypassException"></exception>
        public static void Bypass(string reason)
        {
            throw new StepBypassException(reason);
        }
    }
}
