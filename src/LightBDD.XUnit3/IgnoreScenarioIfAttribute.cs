using System;
using System.Threading.Tasks;
using LightBDD.Core.Execution;
using LightBDD.Core.Extensibility.Execution;
using LightBDD.Framework;
using LightBDD.XUnit3.Implementation.Customization;

namespace LightBDD.XUnit3
{
    /// <summary>
    /// Scenario decorator attribute that conditionally ignores the decorated scenario based on a boolean property
    /// from the fixture's <see cref="IIgnorable{T}.IgnoreSettings"/> object.
    /// <para>
    /// When the setting evaluates to <c>true</c>, the scenario is ignored by throwing an <c>IgnoreException</c>,
    /// causing xUnit v3 to mark the test as skipped. Otherwise, the scenario executes normally.
    /// </para>
    /// <para>
    /// The test fixture must implement <see cref="IIgnorable{T}"/> for this attribute to take effect.
    /// If the fixture does not implement the interface, the scenario always executes normally.
    /// </para>
    /// <para>
    /// This is a generic attribute class. To use it, derive a non-generic class that specifies the settings type:
    /// <code>
    /// public class IgnoreScenarioIfAttribute : IgnoreScenarioIfAttribute&lt;MySettings&gt;
    /// {
    ///     public IgnoreScenarioIfAttribute(string settingName, params string[] reasons) : base(settingName, reasons) { }
    /// }
    /// </code>
    /// </para>
    /// </summary>
    /// <typeparam name="T">The type of the settings object exposed by <see cref="IIgnorable{T}"/>.</typeparam>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class IgnoreScenarioIfAttribute<T> : Attribute, IScenarioDecoratorAttribute where T : class
    {
        private readonly string _settingName;
        private readonly string[] _reasons;

        /// <summary>
        /// Initializes the attribute with the name of a boolean property on <typeparamref name="T"/>
        /// and the reasons to report if the scenario is ignored.
        /// </summary>
        /// <param name="settingName">The name of a boolean property on <typeparamref name="T"/> to evaluate. Use <c>nameof(...)</c> for compile-time safety.</param>
        /// <param name="reasons">One or more reasons to include in the ignore message.</param>
        public IgnoreScenarioIfAttribute(string settingName, params string[] reasons)
        {
            _settingName = settingName;
            _reasons = reasons;
        }

        /// <inheritdoc />
        public Task ExecuteAsync(IScenario scenario, Func<Task> scenarioInvocation)
        {
            var settings = (scenario.Fixture as IIgnorable<T>)?.IgnoreSettings;
            if (settings != null)
            {
                var property = typeof(T).GetProperty(_settingName);
                if (property?.GetValue(settings) is true)
                    throw new IgnoreException(string.Join("; ", _reasons));
            }
            return scenarioInvocation();
        }

        /// <summary>
        /// Order in which extensions should be applied, where instances of lower values would be executed first.
        /// Default value is <c>1</c>.
        /// </summary>
        public int Order { get; set; } = 1;
    }
}
