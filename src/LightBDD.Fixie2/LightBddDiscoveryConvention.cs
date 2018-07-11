using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Fixie;
using LightBDD.Core.Extensibility;
using LightBDD.Fixie2.Implementation;

namespace LightBDD.Fixie2
{
    /// <summary>
    /// Class describing LightBDD discovery convention.
    /// In order to use LightBDD with Fixie, the project containing LightBDD scenarios should contain also class deriving from <see cref="LightBddDiscoveryConvention"/>.
    ///
    /// Example showing how to initialize LightBDD in Fixie framework:
    /// <example>
    /// class WithLightBddDiscovery: LightBddDiscoveryConvention { }
    /// class ConfiguredLightBddScope: LightBddScope { }
    /// </example>
    /// </summary>
    public abstract class LightBddDiscoveryConvention : Discovery
    {
        private string[] _categoriesToInclude = new string[0];
        /// <summary>
        /// Constructor describing the LightBDD discovery conventions.
        /// </summary>
        protected LightBddDiscoveryConvention()
        {
            Classes.Where(x => x.Has<FeatureFixtureAttribute>());
            Methods.Where(x => x.Has<ScenarioAttribute>() && HasAnyCategory(x));
            Parameters.Add<ScenarioCaseProvider>();
        }

        /// <summary>
        /// Runs only tests annotated with one of category specified by <paramref name="categoriesToInclude"/>.
        ///
        /// If <paramref name="categoriesToInclude"/> is empty, all tests will be executed.
        /// </summary>
        protected void IncludeCategories(IEnumerable<string> categoriesToInclude)
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
    }
}