using System.Threading.Tasks;
using Example.Domain.Helpers;
using LightBDD.Framework;
using LightBDD.Framework.Scenarios.Basic;
using LightBDD.Framework.Scenarios.Contextual;
using LightBDD.Framework.Scenarios.Extended;
using NUnit.Framework;

#pragma warning disable 1998

namespace Example.LightBDD.NUnit2.Features
{
    public partial class Customer_journey
    {
        private async Task<CompositeStep> Then_customer_should_receive_order_email()
        {
            return CompositeStep.DefineNew()
                .WithContext<MailBox>()
                .AddAsyncSteps(
                    x => x.Then_customer_should_receive_invoice(),
                    x => x.Then_customer_should_receive_order_confirmation())
                .Build();
        }

        private async Task<CompositeStep> When_customer_pays_for_products_in_basket()
        {
            return CompositeStep.DefineNew()
                .AddSteps(
                    When_customer_requests_to_pay,
                    Then_payment_should_be_successful)
                .Build();
        }

        private async Task<CompositeStep> When_customer_adds_products_to_basket()
        {
            return CompositeStep.DefineNew().AddAsyncSteps(
                    _ => Given_product_is_in_stock("wooden desk"),
                    _ => When_customer_adds_product_to_the_basket("wooden desk"),
                    _ => Then_the_product_addition_should_be_successful())
                .Build();
        }

        private async Task<CompositeStep> Given_customer_is_logged_in()
        {
            return CompositeStep.DefineNew().AddSteps(
                    Given_the_user_is_about_to_login,
                    Given_the_user_entered_valid_login,
                    Given_the_user_entered_valid_password,
                    When_the_user_clicks_login_button,
                    Then_the_login_operation_should_be_successful)
                .Build();
        }

        #region Emails
        private class MailBox
        {
            public async Task Then_customer_should_receive_invoice()
            {
                Assert.Ignore("Not implemented yet");
            }

            public async Task Then_customer_should_receive_order_confirmation()
            {
            }
        }
        #endregion

        #region Basket

        private async Task Then_the_product_addition_should_be_successful()
        {
        }

        private async Task When_customer_adds_product_to_the_basket(string product)
        {
            await LongRunningOperationSimulator.SimulateAsync();
            StepExecution.Current.Bypass("Until proper api is implemented, product is added directly to the DB.");
        }

        private async Task Given_product_is_in_stock(string product)
        {
        }

        #endregion

        #region  Payment
        private void Then_payment_should_be_successful()
        {
        }

        private void When_customer_requests_to_pay()
        {
            LongRunningOperationSimulator.Simulate();
        }
        #endregion

        #region Login steps
        private void Then_the_login_operation_should_be_successful()
        {
        }

        private void When_the_user_clicks_login_button()
        {
            LongRunningOperationSimulator.Simulate();
        }

        private void Given_the_user_entered_valid_password()
        {

        }

        private void Given_the_user_entered_valid_login()
        {
        }

        private void Given_the_user_is_about_to_login()
        {
        }
        #endregion
    }
}