namespace LightBDD.Core.Dependencies
{
    /// <summary>
    /// Container and dependency registration lifetime scope.
    /// </summary>
    public class LifetimeScope
    {
        /// <summary>
        /// Global lifetime used for objects shared across all nested scopes (like singletons).
        /// </summary>
        public static readonly LifetimeScope Global = new LifetimeScope("#global");
        /// <summary>
        /// Scenario lifetime used for objects shared across nested scoped of given scenario, but instantiated independently between scenarios.
        /// </summary>
        public static readonly LifetimeScope Scenario = new LifetimeScope("#scenario");
        /// <summary>
        /// Local lifetime used for objects shared within the current scope but not shared across the scopes.
        /// </summary>
        public static readonly LifetimeScope Local = new LifetimeScope("#local");
        /// <summary>
        /// Transient lifetime used for objects that are always instantiated upon request.
        /// </summary>
        public static readonly LifetimeScope Transient = new LifetimeScope("#transient");
        /// <summary>
        /// Custom lifetime where objects are shared within the scope of the same name and nested scopes.
        /// </summary>
        public static LifetimeScope Named(string name) => new LifetimeScope($"@{name}");
        /// <summary>
        /// Scope identifier
        /// </summary>
        public string Id { get; }

        private LifetimeScope(string id)
        {
            Id = id;
        }

        /// <summary>Determines whether the specified object is equal to the current object.</summary>
        protected bool Equals(LifetimeScope other)
        {
            return Id == other.Id;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((LifetimeScope)obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return (Id != null ? Id.GetHashCode() : 0);
        }
    }
}