using System.Reflection;
using System.Threading;
using LightBDD.Framework.ExecutionContext;

namespace LightBDD.MsTest2.Implementation
{
    internal class TestMethodInfoProvider
    {
        private static readonly AsyncLocalContext<MethodInfo> _testMethod = new AsyncLocalContext<MethodInfo>();
        public static MethodInfo TestMethod { get { return _testMethod.Value; } set { _testMethod.Value = value; } }
    }
}