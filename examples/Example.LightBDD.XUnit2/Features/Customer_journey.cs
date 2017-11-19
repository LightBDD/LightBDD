using System.Threading.Tasks;
using LightBDD.Framework;
using LightBDD.Framework.Scenarios.Basic;
using LightBDD.Framework.Scenarios.Contextual;
using LightBDD.Framework.Scenarios.Extended;
using LightBDD.XUnit2;

#pragma warning disable 1998

namespace Example.LightBDD.XUnit2.Features
{
    /// <summary>
    /// This feature class shows how to create asynchronous scenario from composite steps using basic syntax.
    /// 
    /// More information on asynchronous scenarios can be found here: https://github.com/LightBDD/LightBDD/wiki/Scenario-Steps-Definition#asynchronous-scenario-step-execution
    /// The basic scenario syntax is described here: https://github.com/LightBDD/LightBDD/wiki/Scenario-Steps-Definition#basic-scenarios
    /// The composite steps are described here: https://github.com/LightBDD/LightBDD/wiki/Composite-Steps-Definition
    /// </summary>
    [FeatureDescription(
@"In order to receive a product
As an application user
I want to go through entire customer journey")]
    [Label("Story-6")]
    public partial class Customer_journey : FeatureFixture
    {
        /// <summary>
        /// This scenario uses basic syntax for asynchronous steps (it uses RunScenarioAsync() method),
        /// however all of the used steps are composites, where each composite is constructed in different way and use synchronous or asynchronous sub-steps.
        /// </summary>
        [Scenario]
        [Label("Ticket-12")]
        public async Task Ordering_products()
        {
            await Runner.RunScenarioAsync(
                Given_customer_is_logged_in,
                When_customer_adds_products_to_basket,
                When_customer_pays_for_products_in_basket,
                Then_customer_should_receive_order_email);
        }

        /// <summary>
        /// This step shows that it is possible to create a contextual step, where context will be shared between all the sub-steps,
        /// and then use extended syntax to call some asynchronous sub-steps.
        /// </summary>
        private async Task<CompositeStep> Then_customer_should_receive_order_email()
        {
            return CompositeStep.DefineNew()
                .WithContext<MailBox>()
                .AddAsyncSteps(
                    x => x.Then_customer_should_receive_invoice(),
                    x => x.Then_customer_should_receive_order_confirmation())
                .Build();
        }

        /// <summary>
        /// This one, on the other hand uses basic syntax and uses synchronous sub-steps. 
        /// </summary>
        private async Task<CompositeStep> When_customer_pays_for_products_in_basket()
        {
            return CompositeStep.DefineNew()
                .AddSteps(
                    When_customer_requests_to_pay,
                    Then_payment_should_be_successful)
                .Build();
        }

        /// <summary>
        /// Here, the composite is made of asynchronous steps that are called with extended syntax. 
        /// </summary>
        private async Task<CompositeStep> When_customer_adds_products_to_basket()
        {
            return CompositeStep.DefineNew()
                .AddAsyncSteps(
                    _ => Given_product_is_in_stock("wooden desk"),
                    _ => When_customer_adds_product_to_the_basket("wooden desk"),
                    _ => Then_the_product_addition_should_be_successful())
                .Build();
        }

        /// <summary>
        /// Finally, this composite uses basic syntax and synchronous steps. 
        /// </summary>
        private async Task<CompositeStep> Given_customer_is_logged_in()
        {
            return CompositeStep.DefineNew()
                .AddSteps(
                    Given_the_user_is_about_to_login,
                    Given_the_user_entered_valid_login,
                    Given_the_user_entered_valid_password,
                    When_the_user_clicks_login_button,
                    Then_the_login_operation_should_be_successful)
                .Build();
        }
    }
}
