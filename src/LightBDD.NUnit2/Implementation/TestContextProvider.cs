using System;
using System.Reflection;
using LightBDD.Framework.ExecutionContext;

namespace LightBDD.NUnit2.Implementation
{
    internal class TestContextProvider : MarshalByRefObject
    {
        private static readonly AsyncLocalContext<TestContextProvider> Provider = new AsyncLocalContext<TestContextProvider>();
        public MethodInfo TestMethod { get; }

        public static TestContextProvider Current => Provider.Value;

        public static void Initialize(MethodInfo testMethod)
        {
            Provider.Value = new TestContextProvider(testMethod);
        }

        public static void Clear()
        {
            Provider.Value = null;
        }

        private TestContextProvider(MethodInfo testMethod)
        {
            TestMethod = testMethod;
        }
    }
}