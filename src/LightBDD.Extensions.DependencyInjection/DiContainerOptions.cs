using System;

namespace LightBDD.Extensions.DependencyInjection
{
    /// <summary>
    /// Configuration options for Dependency Injection Container.
    /// </summary>
    public class DiContainerOptions
    {
        /// <summary>
        /// Specifies if the provider should be disposed by LightBDD after tests are finished - <c>true</c> by default.
        /// </summary>
        public DiContainerOptions TakeOwnership(bool takeOwnership)
        {
            ShouldTakeOwnership = takeOwnership;
            return this;
        }

        /// <summary>
        /// Configures scenario scope nesting.<br/>
        /// If <c>true</c>, composite steps executed within scenario will run in the nested scope. All instances resolved in that scope will get disposed upon composite step finish, but composite steps will receive different instances of types registered as scoped than scenario.<br/>
        /// If <c>false</c>, composite steps executed within scenario will run in the scenario scope. The lifetime of all instances resolved from the composite steps will match scenario lifetime and composite steps as well as parent scenario will receive the same instances of types registered as scoped.<br/>
        /// Default setting: <c>false</c>
        /// </summary>
        public DiContainerOptions EnableScopeNestingWithinScenarios(bool enabled)
        {
            ShouldEnableScopeNestingWithinScenarios = enabled;
            return this;
        }

        /// <summary>
        /// Specifies if scope nesting within scenarios should be enabled or not. If enabled, composite steps would run in independent, nested scope. If disabled, the composite steps will share the scope with parent scenario.
        /// </summary>
        public bool ShouldEnableScopeNestingWithinScenarios { get; private set; } = false;

        /// <summary>
        /// Specifies if <see cref="IServiceProvider"/> should be disposed by LightBDD after tests are finished.
        /// </summary>
        public bool ShouldTakeOwnership { get; private set; } = true;
    }
}