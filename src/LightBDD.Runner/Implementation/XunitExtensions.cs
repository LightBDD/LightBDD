using System.Reflection;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace LightBDD.XUnit2.Implementation;

internal static class XunitExtensions
{
    public static Assembly ToRuntimeAssembly(this IAssemblyInfo assemblyInfo) =>
        ((ReflectionAssemblyInfo)assemblyInfo).Assembly;
}