using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using LightBDD.Core.Execution;
using LightBDD.Core.Extensibility.Execution;

namespace LightBDD.TUnit
{
    /// <summary>
    /// Attribute allowing to ignore scenario in declarative way. It can be applied on scenario method or step method as well as feature class.
    /// If applied on scenario, no steps will be executed, but scenario will be included in reports.
    /// If applied on class level, all scenarios in this class will get ignored.
    /// It is recommended to use this attribute in favor of <see cref="SkipAttribute"/>.
    /// </summary>
    public class IgnoreScenarioAttribute : Attribute, IScenarioDecoratorAttribute, IStepDecoratorAttribute
    {
        private readonly string _reason;

        /// <summary>
        /// Default constructor allowing to specify ignore reason.
        /// </summary>
        /// <param name="reason">Ignore reason.</param>
        public IgnoreScenarioAttribute(string reason)
        {
            _reason = reason;
        }
        
        /// <summary>
        /// Order in which extensions should be applied, where instances of lower values would be executed first.
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// Stops execution of current scenario with ignored status.
        /// </summary>
        public Task ExecuteAsync(IScenario scenario, Func<Task> scenarioInvocation)
        {
            Skip.Test(_reason);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Stops execution of current step with ignored status.
        /// </summary>
        public Task ExecuteAsync(IStep step, Func<Task> stepInvocation)
        {
            Skip.Test(_reason);
            return Task.CompletedTask;
        }
    }
}