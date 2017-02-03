using System.Reflection;
using System.Threading;

namespace LightBDD.MsTest.Implementation
{
    internal class TestMethodInfoProvider
    {
        private static readonly AsyncLocal<MethodInfo> _testMethod = new AsyncLocal<MethodInfo>();
        public static MethodInfo TestMethod { get { return _testMethod.Value; } set { _testMethod.Value = value; } }
    }
}