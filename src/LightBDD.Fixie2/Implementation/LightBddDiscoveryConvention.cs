using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Fixie;
using LightBDD.Core.Extensibility;

namespace LightBDD.Fixie2.Implementation
{
    internal class LightBddDiscoveryConvention : IDiscovery
    {
        private string[] _categoriesToInclude = new string[0];

        /// <summary>
        /// Runs only tests annotated with one of category specified by <paramref name="categoriesToInclude"/>.
        ///
        /// If <paramref name="categoriesToInclude"/> is empty, all tests will be executed.
        /// </summary>
        public void IncludeCategories(IEnumerable<string> categoriesToInclude)
        {
            if (categoriesToInclude == null)
                throw new ArgumentNullException(nameof(categoriesToInclude));
            _categoriesToInclude = categoriesToInclude.ToArray();
        }

        private bool HasAnyCategory(MethodInfo method)
        {
            return !_categoriesToInclude.Any() || GetCategories(method).Any(_categoriesToInclude.Contains);
        }

        private IEnumerable<string> GetCategories(MethodInfo method)
        {
            return method.GetCustomAttributes(true)
                .Concat(method.DeclaringType?.GetCustomAttributes() ?? Enumerable.Empty<Attribute>())
                .OfType<IScenarioCategoryAttribute>()
                .Select(a => a.Category);
        }

        public IEnumerable<Type> TestClasses(IEnumerable<Type> concreteClasses)
            => concreteClasses.Where(x => x.Has<FeatureFixtureAttribute>());

        public IEnumerable<MethodInfo> TestMethods(IEnumerable<MethodInfo> publicMethods)
            => publicMethods.Where(x => x.Has<ScenarioAttribute>() && HasAnyCategory(x));
    }
}