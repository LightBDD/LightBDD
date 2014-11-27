using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LightBDD
{
    [DebuggerStepThrough]
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
        /// <param name="member">Scenario method or feature test class to analyze.</param>
        /// <returns>Scenario categories or empty collection.</returns>
        protected override IEnumerable<string> GetImplementationSpecificScenarioCategories(MemberInfo member)
        {
            return ExtractAttributes<TestCategoryAttribute>(member).SelectMany(a => a.TestCategories);
        }
    }
}