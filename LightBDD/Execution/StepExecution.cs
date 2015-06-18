using LightBDD.Execution.Exceptions;
using LightBDD.Execution.Implementation;

namespace LightBDD.Execution
{
    /// <summary>
    /// Step execution class, allowing to control step execution.
    /// </summary>
    public static class StepExecution
    {
        /// <summary>
        /// Bypasses currently executed step and continues execution of current scenario, allowing to execute all remaining steps.
        /// The status of bypassed step would be <c>ResultStatus.Bypassed</c> and the overall status of scenario would be <c>ResultStatus.Bypassed</c>,
        /// unless any further step is failed or ignored.<br/>
        /// 
        /// Scenarios with <c>ResultStatus.Bypassed</c> status are recognized as passed tests in underlying test framework.<br/>
        /// 
        /// The <c>reason</c> argument would be used as step <c>StatusDetails</c>, and it would be aggregated in overall scenario <c>StatusDetails</c> as well.<br/>
        /// 
        /// The Bypass method could be used in situations when:
        /// <list type="bullet">
        /// <item><description>It is not possible to implement given step at the moment (no required API is implemented yet), but all other steps are precise enough to prove that scenario is successful, i.e. situation when scenario checks overall and detailed cost of product and one of price component cannot be retrieved.</description></item>
        /// <item><description>Step implementation does not exists, but it is possible to simulate it, so further steps can be executed, i.e. end-to-end tests, where the middle component does not exist yet.</description></item>
        /// <item><description>The required API is not exposed yet, but it is possible to implement a workaround like direct data insert to database.</description></item>
        /// </list>
        /// </summary>
        /// <param name="reason">Bypass reason</param>
        /// <exception cref="StepBypassException">Bypass exception used to control scenario execution</exception>
        public static void Bypass(string reason)
        {
            throw new StepBypassException(reason);
        }

        /// <summary>
        /// Comments currently executed step with a <c>comment</c> text.
        /// The comment would be included in progress notification, as well as in execution reports.
        /// </summary>
        /// <param name="comment">Comment</param>
        public static void Comment(string comment)
        {
            if (!string.IsNullOrWhiteSpace(comment))
                ExecutionContext.Instance.CurrentStep.Comment(ExecutionContext.Instance, comment);
        }

        /// <summary>
        /// Comments currently executed step with a formatted comment, based on <c>format</c> and <c>args</c> parameters.
        /// The comment would be included in progress notification, as well as in execution reports.
        /// </summary>
        /// <param name="format">Comment format.</param>
        /// <param name="args">Comment format arguments.</param>
        public static void CommentFormat(string format, params object[] args)
        {
            Comment(string.Format(format, args));
        }
    }
}
