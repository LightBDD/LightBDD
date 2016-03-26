using LightBDD.Core.Extensibility;

namespace LightBDD.Integration.NUnit3
{
    public class NUnit3BddRunnerFactory : BddRunnerFactory
    {
        public static NUnit3BddRunnerFactory Instance { get; } = new NUnit3BddRunnerFactory();

        private NUnit3BddRunnerFactory() : base(new NUnit3IntegrationContext())
        {
        }
    }
}