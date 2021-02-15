using System;

namespace LightBDD.Core.Dependencies
{
    /// <summary>
    /// Instance scope describing how given instance is shared between dependencies resolved within same and nested lifetime scopes.
    /// </summary>
    public class InstanceScope
    {
        /// <summary>
        /// The same instance is returned for requests within the root and nested scopes.
        /// </summary>
        public static readonly InstanceScope Single = new InstanceScope(true, true);
        /// <summary>
        /// The same instance is returned for requests within the given scope, however not shared with nested scopes. Each scope will receive one instance upon request.
        /// </summary>
        public static readonly InstanceScope Local = new InstanceScope(true, false);
        /// <summary>
        /// The new instance is returned upon every request.
        /// </summary>
        public static readonly InstanceScope Transient = new InstanceScope(false, false);
        /// <summary>
        /// The instance is shared within given scenario scope and across all nested scopes, but instantiated independently between scenarios.<br/>
        /// The instance definition will be ignored when resolution request is made outside of the scenario.
        /// </summary>
        public static readonly InstanceScope Scenario = Named(LifetimeScope.Scenario, true, true);

        /// <summary>
        /// Creates the instance scope limited to the specified lifetime scope represented by <paramref name="scopeRestriction"/> parameter and allows configuring sharing options of the instance.
        /// If given container lifetime scope does not match the <paramref name="scopeRestriction"/>, the definition using this InstanceScope will be ignored, however passed down to the nested scopes.
        /// </summary>
        /// <param name="scopeRestriction">Determines the <seealso cref="LifetimeScope"/> that this instance scope is applicable to. If given container lifetime scope does not match the <paramref name="scopeRestriction"/>, the definition using this InstanceScope will be ignored, however passed down to the nested scopes.</param>
        /// <param name="isSharedInstance">Determines if the instance should be shared across requests.</param>
        /// <param name="isSharedWithNestedScopes">Determines if the instance should be shared with nested scopes, assuming the <paramref name="isSharedInstance"/> is true.</param>
        public static InstanceScope Named(LifetimeScope scopeRestriction, bool isSharedInstance = true, bool isSharedWithNestedScopes = true)
        {
            return new InstanceScope(isSharedInstance, isSharedWithNestedScopes, scopeRestriction ?? throw new ArgumentNullException(nameof(scopeRestriction)));
        }

        /// <summary>
        /// Returns true if the instance should be shared between requests made within the given scope.
        /// </summary>
        public bool IsSharedInstance { get; }

        /// <summary>
        /// Returns true if the instance should be shared with the nested lifetime scopes. 
        /// </summary>
        public bool IsSharedWithNestedScopes { get; }

        /// <summary>
        /// <seealso cref="LifetimeScope"/> that given instance definition is applicable from, or <value>null</value> if instance is applicable from the container scope it was registered against.
        /// In most scenarios this value is <value>null</value>, thus the dependency registration is applied on the current container.<br/>
        /// If specified, the dependency registration will become applicable from the first matching container lifetime scope (i.e. current or nested).
        /// </summary>
        public LifetimeScope LifetimeScopeRestriction { get; }

        private InstanceScope(bool isSharedInstance, bool isSharedWithNestedScopes, LifetimeScope scopeRestriction = null)
        {
            IsSharedInstance = isSharedInstance;
            IsSharedWithNestedScopes = isSharedWithNestedScopes;
            LifetimeScopeRestriction = scopeRestriction;
        }
    }
}