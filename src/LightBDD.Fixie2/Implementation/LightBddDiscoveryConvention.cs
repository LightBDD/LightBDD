using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Fixie;
using LightBDD.Core.Extensibility;

namespace LightBDD.Fixie3.Implementation
{
    internal class LightBddDiscoveryConvention : IDiscovery
    {
        private readonly string[] _categoriesToInclude;

        public LightBddDiscoveryConvention(string[] category)
        {
            _categoriesToInclude = category ?? throw new ArgumentNullException(nameof(category));
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