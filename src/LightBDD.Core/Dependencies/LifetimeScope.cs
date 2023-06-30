using System;

namespace LightBDD.Core.Dependencies
{
    /// <summary>
    /// Container lifetime scope.
    /// </summary>
    public sealed class LifetimeScope
    {
        /// <summary>
        /// Global scope created once and affecting all tests.
        /// The global scope should be used by container root.
        /// </summary>
        public static readonly LifetimeScope Global = new("#global");

        /// <summary>
        /// Scenario scope created once per each scenario.
        /// </summary>
        public static readonly LifetimeScope Scenario = new("#scenario");

        /// <summary>
        /// Local scope created for all nested scopes such as composite steps.
        /// </summary>
        public static readonly LifetimeScope Local = new("#local");

        /// <summary>
        /// Scope name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Scope constructor.
        /// </summary>
        /// <param name="name">Scope name.</param>
        public LifetimeScope(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        /// <summary>
        /// Determines if <paramref name="other"/> is equal current scope.
        /// </summary>
        public bool Equals(LifetimeScope other)
        {
            return Name == other?.Name;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((LifetimeScope)obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return (Name != null ? Name.GetHashCode() : 0);
        }

        /// <inheritdoc />
        public override string ToString() => Name;
    }
}