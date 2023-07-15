#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using LightBDD.Core.Extensibility;

namespace LightBDD.Core.Discovery
{
    public class ScenarioDiscoverer
    {
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

        public IEnumerable<ScenarioCase> DiscoverFor(TypeInfo type, CancellationToken cancellationToken = default)
        {
            if (!IsValid(type))
                yield break;
            foreach (var methodInfo in type.GetMethods(BindingFlags.Instance | BindingFlags.Public))
            {
                if (cancellationToken.IsCancellationRequested)
                    yield break;
                if (!IsValid(methodInfo))
                    continue;

                if (!methodInfo.GetParameters().Any())
                {
                    yield return ScenarioCase.CreateParameterless(type, methodInfo);
                    continue;
                }

                var hasRuntimeArguments = false;
                foreach (var scenarioCaseSource in methodInfo.GetCustomAttributes().OfType<IScenarioCaseSourceAttribute>())
                {
                    if (cancellationToken.IsCancellationRequested)
                        yield break;

                    if (!scenarioCaseSource.IsResolvableAtDiscovery)
                    {
                        hasRuntimeArguments = true;
                        continue;
                    }

                    foreach (var arguments in scenarioCaseSource.GetCases())
                        yield return ScenarioCase.CreateParameterized(type, methodInfo, arguments);
                }

                if (hasRuntimeArguments)
                    yield return ScenarioCase.CreateParameterizedAtRuntime(type, methodInfo);
            }
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
