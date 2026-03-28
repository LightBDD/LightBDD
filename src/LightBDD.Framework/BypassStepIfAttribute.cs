using System;
using System.Threading.Tasks;
using LightBDD.Core.Execution;
using LightBDD.Core.ExecutionContext;
using LightBDD.Core.Extensibility.Execution;

namespace LightBDD.Framework
{
    /// <summary>
    /// Step decorator attribute that conditionally bypasses the decorated step based on a boolean property
    /// from the fixture's <see cref="IBypassable{T}.BypassSettings"/> object.
    /// <para>
    /// When the setting evaluates to <c>true</c>, the step is bypassed with the specified reasons.
    /// Otherwise, the step executes normally.
    /// </para>
    /// <para>
    /// The test fixture must implement <see cref="IBypassable{T}"/> for this attribute to take effect.
    /// If the fixture does not implement the interface, the step always executes normally.
    /// </para>
    /// <para>
    /// This is a generic attribute class. To use it, derive a non-generic class that specifies the settings type:
    /// <code>
    /// public class BypassStepIfAttribute : BypassStepIfAttribute&lt;MySettings&gt;
    /// {
    ///     public BypassStepIfAttribute(string settingName, params string[] reasons) : base(settingName, reasons) { }
    /// }
    /// </code>
    /// </para>
    /// </summary>
    /// <typeparam name="T">The type of the settings object exposed by <see cref="IBypassable{T}"/>.</typeparam>
    [AttributeUsage(AttributeTargets.Method)]
    public class BypassStepIfAttribute<T> : Attribute, IStepDecoratorAttribute where T : class
    {
        private readonly string _settingName;
        private readonly string[] _reasons;

        /// <summary>
        /// Initializes the attribute with the name of a boolean property on <typeparamref name="T"/>
        /// and the reasons to report if the step is bypassed.
        /// </summary>
        /// <param name="settingName">The name of a boolean property on <typeparamref name="T"/> to evaluate. Use <c>nameof(...)</c> for compile-time safety.</param>
        /// <param name="reasons">One or more reasons to include in the bypass message.</param>
        public BypassStepIfAttribute(string settingName, params string[] reasons)
        {
            _settingName = settingName;
            _reasons = reasons;
        }

        /// <inheritdoc />
        public Task ExecuteAsync(IStep step, Func<Task> stepInvocation)
        {
            var settings = (ScenarioExecutionContext.CurrentScenario.Fixture as IBypassable<T>)?.BypassSettings;
            if (settings != null)
            {
                var property = typeof(T).GetProperty(_settingName);
                if (property?.GetValue(settings) is true)
                {
                    StepExecution.Current.Bypass(string.Join("; ", _reasons));
                    return Task.CompletedTask;
                }
            }
            return stepInvocation();
        }

        /// <summary>
        /// Order in which extensions should be applied, where instances of lower values would be executed first.
        /// Default value is <c>1</c>.
        /// </summary>
        public int Order { get; set; } = 1;
    }
}
