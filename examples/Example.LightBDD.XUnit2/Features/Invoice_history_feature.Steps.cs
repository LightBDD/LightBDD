using System.Threading.Tasks;
using LightBDD.Example.Helpers;
using LightBDD.XUnit2;
using Xunit.Abstractions;

namespace LightBDD.Example.AcceptanceTests.XUnit2.Features
{
    public partial class Invoice_history_feature : FeatureFixture
    {
        private async Task Given_invoice(string invoice)
        {
            await LongRunningOperationSimulator.SimulateAsync();
        }

        private void When_I_request_all_historical_invoices()
        {
            LongRunningOperationSimulator.Simulate();
        }

        private void Then_I_should_see_invoices(params string[] invoices)
        {
        }

        public Invoice_history_feature(ITestOutputHelper output) : base(output)
        {
        }
    }
}