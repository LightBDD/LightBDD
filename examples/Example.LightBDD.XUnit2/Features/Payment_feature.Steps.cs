using System.Threading.Tasks;
using Example.Domain.Helpers;
using LightBDD.XUnit2;

namespace Example.LightBDD.XUnit2.Features
{
    public partial class Payment_feature : FeatureFixture
    {
        private async Task Given_customer_has_some_products_in_basket()
        {
            await LongRunningOperationSimulator.SimulateAsync();
        }

        private async Task Given_customer_has_enough_money_to_pay_for_products()
        {
            await LongRunningOperationSimulator.SimulateAsync();
        }

        private async Task When_customer_requests_to_pay()
        {
            await LongRunningOperationSimulator.SimulateAsync();
        }

        private async Task Then_payment_should_be_successful()
        {
            await LongRunningOperationSimulator.SimulateAsync();
        }
    }
}