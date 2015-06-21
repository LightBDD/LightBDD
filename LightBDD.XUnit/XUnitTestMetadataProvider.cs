using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace LightBDD
{
    [DebuggerStepThrough]
    internal class XUnitTestMetadataProvider : TestMetadataProvider
    {
        public static readonly TestMetadataProvider Instance = new XUnitTestMetadataProvider();

        /// <summary>
        /// Returns implementation specific feature description or null if such is not provided.
        /// Current implementation always returns null.
        /// </summary>
        /// <param name="testClass">Class to analyze.</param>
        /// <returns>Feature description or null.</returns>
        protected override string GetImplementationSpecificFeatureDescription(Type testClass)
        {
            return null;
        }

        /// <summary>
        /// Returns implementation specific scenario categories or empty collection if no categories are provided.
        /// Current implementation always returns empty collection.
        /// </summary>
        /// <param name="member">Scenario method or feature test class to analyze.</param>
        /// <returns>Scenario categories or empty collection.</returns>
        protected override IEnumerable<string> GetImplementationSpecificScenarioCategories(MemberInfo member)
        {
            return Enumerable.Empty<string>();
        }
    }
}