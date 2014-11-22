using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
        /// Returns implementation specific scenario categories or empty collection if no categories are provided.
        /// If test class is annotated with [Category] attribute, it's content is used as scenario category.
        /// </summary>
        /// <param name="scenarioMethod">Scenario method to analyze.</param>
        /// <returns>Scenario categories or empty collection.</returns>
        protected override IEnumerable<string> GetImplementationSpecificScenarioCategories(MethodBase scenarioMethod)
        {
            return ExtractAttributes<TestCategoryAttribute>(scenarioMethod).SelectMany(a => a.TestCategories);
        }
    }
}