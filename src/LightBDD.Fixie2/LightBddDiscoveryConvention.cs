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
        /// <summary>
        /// Constructor describing the LightBDD discovery conventions, accepting additional arguments.
        /// Currently, LightBDD recognizes the arguments of format <c>category:MyCategory</c> and uses them for running only the tests marked with such categories.
        /// </summary>
        protected LightBddDiscoveryConvention(string[] args)
        {
            if (args == null)
                throw new ArgumentNullException(nameof(args));

            var categoriesToInclude = GetCategoriesToInclude(args);
            Classes.Where(x => x.Has<FeatureFixtureAttribute>());
            Methods.Where(x => x.Has<ScenarioAttribute>() && HasAnyCategory(x, categoriesToInclude));
            Parameters.Add<ScenarioCaseProvider>();
        }

        private static string[] GetCategoriesToInclude(string[] args)
        {
            var categoryPrefix = "category:";

            var categoriesToInclude = args
                .Where(x => x.StartsWith(categoryPrefix, StringComparison.OrdinalIgnoreCase))
                .Select(x => x.Substring(categoryPrefix.Length))
                .ToArray();

            return categoriesToInclude;
        }

        /// <summary>
        /// Default constructor describing the LightBDD discovery conventions, if no additional arguments are needed.
        /// </summary>
        protected LightBddDiscoveryConvention() : this(new string[0]) { }

        private bool HasAnyCategory(MethodInfo method, string[] categoriesToInclude)
        {
            return !categoriesToInclude.Any() || GetCategories(method).Any(categoriesToInclude.Contains);
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