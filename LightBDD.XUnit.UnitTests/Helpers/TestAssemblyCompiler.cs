using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CSharp;

namespace LightBDD.XUnit.UnitTests.Helpers
{
    internal static class TestAssemblyCompiler
    {
        public static string Compile(string code)
        {
            var parameters = new CompilerParameters()
            {
                OutputAssembly = string.Format("test_{0}.dll", Guid.NewGuid()),
                IncludeDebugInformation = true
            };
            parameters.ReferencedAssemblies.Add("mscorlib.dll");
            parameters.ReferencedAssemblies.Add("System.dll");
            parameters.ReferencedAssemblies.Add("System.Core.dll");
            parameters.ReferencedAssemblies.Add("System.Runtime.dll");
            parameters.ReferencedAssemblies.Add("LightBDD.XUnit.dll");
            parameters.ReferencedAssemblies.Add("xunit.core.dll");
            parameters.ReferencedAssemblies.Add("xunit.assert.dll");

            var compilerOptions = new Dictionary<string, string> { { "CompilerVersion", "v4.0" } };
            var provider = new CSharpCodeProvider(compilerOptions);
            var results = provider.CompileAssemblyFromSource(parameters, code);

            if (results.Errors.Count != 0)
            {
                var errors = results.Errors.Cast<CompilerError>()
                    .Select(error => string.Format("{0}({1},{2}): error {3}: {4}", error.FileName, error.Line, error.Column, error.ErrorNumber, error.ErrorText))
                    .ToArray();

                throw new InvalidOperationException(string.Format("Compilation Failed:{0}{1}", Environment.NewLine, string.Join(Environment.NewLine, errors)));
            }
            return parameters.OutputAssembly;
        }
    }
}