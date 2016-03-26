using LightBDD.Core.Extensibility;

namespace LightBDD.Integration.XUnit2
{
    public class XUnitBddRunnerFactory : BddRunnerFactory
    {
        public static XUnitBddRunnerFactory Instance { get; } = new XUnitBddRunnerFactory();

        private XUnitBddRunnerFactory() : base(new XUnitIntegrationContext())
        {
        }
    }
}