using System.Threading.Tasks;
using Example.Domain.Helpers;
using LightBDD.XUnit2;

namespace Example.LightBDD.XUnit2.Features
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
    }
}