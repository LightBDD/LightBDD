#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using LightBDD.Core.Extensibility;

namespace LightBDD.Core.Discovery
{
    /// <summary>
    /// Scenario discoverer allowing to discover LightBDD scenario test cases.
    /// </summary>
    public class ScenarioDiscoverer
    {
        /// <summary>
        /// Discovers LightBDD scenario test cases within the specified assembly provided by <paramref name="assembly"/> parameter.
        /// </summary>
        /// <param name="assembly">Assembly to provide scenario test cases for.</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Collection of scenario cases</returns>
        public IEnumerable<ScenarioCase> DiscoverFor(Assembly assembly, CancellationToken cancellationToken = default)
        {
            foreach (var type in assembly.DefinedTypes)
            {
                if (cancellationToken.IsCancellationRequested)
                    yield break;
                foreach (var scenarioCase in DiscoverFor(type, cancellationToken))
                    yield return scenarioCase;
            }
        }

        /// <summary>
        /// Discovers LightBDD scenario test cases within the specified feature fixture type provided by <paramref name="featureFixtureType"/> parameter.
        /// </summary>
        /// <param name="featureFixtureType">Feature fixture type to provide scenario test cases for.</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Collection of scenario cases</returns>
        public IEnumerable<ScenarioCase> DiscoverFor(TypeInfo featureFixtureType, CancellationToken cancellationToken = default)
        {
            if (!IsValid(featureFixtureType))
                yield break;
            foreach (var methodInfo in featureFixtureType.GetMethods(BindingFlags.Instance | BindingFlags.Public))
            {
                if (cancellationToken.IsCancellationRequested)
                    yield break;
                foreach (var scenarioCase in DiscoverFor(featureFixtureType, methodInfo, cancellationToken))
                    yield return scenarioCase;
            }
        }

        /// <summary>
        /// Discovers LightBDD scenario test cases within the specified feature fixture type provided by <paramref name="featureFixtureType"/> parameter and for specified method provided by <paramref name="scenarioMethod"/> parameter.
        /// </summary>
        /// <param name="featureFixtureType">Feature fixture type to provide scenario test cases for.</param>
        /// <param name="scenarioMethod">Method to provide scenario test cases for.</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Collection of scenario cases</returns>
        public IEnumerable<ScenarioCase> DiscoverFor(TypeInfo featureFixtureType, MethodInfo scenarioMethod, CancellationToken cancellationToken)
        {
            if (!IsValid(scenarioMethod))
                yield break;

            if (!scenarioMethod.GetParameters().Any())
            {
                yield return ScenarioCase.CreateParameterless(featureFixtureType, scenarioMethod);
                yield break;
            }

            var hasRuntimeArguments = false;
            foreach (var scenarioCaseSource in scenarioMethod.GetCustomAttributes().OfType<IScenarioCaseSourceAttribute>())
            {
                if (cancellationToken.IsCancellationRequested)
                    yield break;

                if (!scenarioCaseSource.IsResolvableAtDiscovery)
                {
                    hasRuntimeArguments = true;
                    continue;
                }

                foreach (var arguments in scenarioCaseSource.GetCases())
                    yield return ScenarioCase.CreateParameterized(featureFixtureType, scenarioMethod, arguments);
            }

            if (hasRuntimeArguments)
                yield return ScenarioCase.CreateParameterizedAtRuntime(featureFixtureType, scenarioMethod);
        }

        private bool IsValid(MethodInfo methodInfo)
        {
            if (!methodInfo.GetCustomAttributes<Attribute>().Any(attr => attr is IScenarioAttribute))
                return false;
            if (methodInfo.ContainsGenericParameters)
                return false;
            return true;
        }

        private bool IsValid(TypeInfo type) => !type.IsAbstract;
    }
}
