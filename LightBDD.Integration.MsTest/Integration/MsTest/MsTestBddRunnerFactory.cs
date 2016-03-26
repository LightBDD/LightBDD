using LightBDD.Core.Extensibility;

namespace LightBDD.Integration.MsTest
{
    public class MsTestBddRunnerFactory : BddRunnerFactory
    {
        public static MsTestBddRunnerFactory Instance { get; } = new MsTestBddRunnerFactory();

        private MsTestBddRunnerFactory() : base(new MsTestIntegrationContext())
        {
        }
    }
}