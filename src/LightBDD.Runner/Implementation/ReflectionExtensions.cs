using System.Reflection;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace LightBDD.Runner.Implementation;

internal static class ReflectionExtensions
{
    public static Assembly ToRuntimeAssembly(this IAssemblyInfo assemblyInfo) => ((ReflectionAssemblyInfo)assemblyInfo).Assembly;

    public static T[]? ToNullIfEmpty<T>(this T[]? collection) => collection?.Length == 0 ? null : collection;
}