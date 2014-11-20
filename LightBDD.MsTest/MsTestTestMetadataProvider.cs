using System;
using System.Collections.Generic;
using System.Linq;

namespace LightBDD
{
    internal class MsTestTestMetadataProvider : TestMetadataProvider
    {
        public static readonly TestMetadataProvider Instance = new MsTestTestMetadataProvider();

        /// <summary>
        /// Always returns null - MsTest framework does not have dedicated description attribute for test classes.
        /// </summary>
        /// <param name="testClass">Class to analyze.</param>
        /// <returns>Null.</returns>
        protected override string GetImplementationSpecificFeatureDescription(Type testClass)
        {
            return null;
        }

        /// <summary>
        /// Always returns empty collection - MsTest framework does not have dedicated category attribute for test classes.
        /// </summary>
        /// <param name="testClass">Class to analyze.</param>
        /// <returns>Empty collection.</returns>
        protected override IEnumerable<string> GetImplementationSpecificFeatureCategories(Type testClass)
        {
            return Enumerable.Empty<string>();
        }
    }
}