using System;
using System.Threading.Tasks;
using LightBDD.Core.Execution;
using LightBDD.Core.Extensibility.Execution;
using LightBDD.Core.Metadata;
using LightBDD.Framework;
using LightBDD.XUnit2.Implementation.Customization;
using Xunit;

namespace LightBDD.XUnit2
{
    /// <summary>
    /// Attribute allowing to ignore scenario in declarative way. It can be applied on scenario method or step method.
    /// If applied on scenario, no steps will be executed, but scenario will be included in reports.
    /// It is recommended to use this attribute in favor of <see cref="ScenarioAttribute.Skip"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class IgnoreScenarioAttribute : Attribute, IScenarioExtensionAttribute, IStepExtensionAttribute
    {
        /// <summary>
        /// Default constructor allowing to specify ignore reason.
        /// </summary>
        /// <param name="reason">Ignore reason.</param>
        public IgnoreScenarioAttribute(string reason)
        {
            Reason = reason;
        }

        /// <summary>
        /// Ignore reason.
        /// </summary>
        public string Reason { get; }

        /// <summary>
        /// Order in which extensions should be applied, where instances of lower values would be executed first.
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// Stops execution of current scenario with ignored status.
        /// </summary>
        public Task ExecuteAsync(IScenario scenario, Func<Task> scenarioInvocation)
        {
            throw new IgnoreException(Reason);
        }

        /// <summary>
        /// Stops execution of current step and scenario with ignored status.
        /// </summary>
        public Task ExecuteAsync(IStep step, Func<Task> stepInvocation)
        {
            throw new IgnoreException(Reason);
        }
    }
}