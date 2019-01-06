using LightBDD.Framework;
using LightBDD.Framework.Scenarios.Basic;
using LightBDD.NUnit3;
using System.Threading.Tasks;

namespace Example.LightBDD.NUnit3.Features
{
    /// <summary>
    /// This feature class presents that it is possible to use basic scenario syntax with step methods of void or async void signature.
    /// Scenarios use here the RunScenarioActionsAsync() method that allows mixing void and async void steps.
    /// 
    /// As using async void methods is generally not recommended practice (and LightBDD offers ways to handle Task methods),
    /// LightBDD properly handles async void steps, waiting for all pending tasks to finish, before proceeding to the next step.
    /// 
    /// Such way of writing tests may be useful when async steps has to be introduced to existing tests that uses only synchronous step methods.
    /// More details can be found here: https://github.com/LightBDD/LightBDD/wiki/Scenario-Steps-Definition#asynchronous-scenario-step-execution
    /// </summary>
    [FeatureDescription(
@"In order to buy products
As a customer
I want to add products to basket")]
    [Label("Story-4")]
    public partial class Basket_feature
    {
        [Scenario]
        [Label("Ticket-6")]
        [ScenarioCategory(Categories.Sales)]
        public async Task No_product_in_stock()
        {
            await Runner.AddSteps(
                    Given_product_is_out_of_stock,
                    When_customer_adds_it_to_the_basket,
                    Then_the_product_addition_should_be_unsuccessful,
                    Then_the_basket_should_not_contain_the_product)
                .RunAsync();
        }

        /// <summary>
        /// This test presents how LightBDD treats tests with Inconclusive / Ignore assertions.
        /// </summary>
        [Scenario]
        [Label("Ticket-7")]
        [ScenarioCategory(Categories.Sales)]
        public async Task Successful_addition()
        {
            await Runner.AddSteps(
                    Given_product_is_in_stock,
                    When_customer_adds_it_to_the_basket,
                    Then_the_product_addition_should_be_successful,
                    Then_the_basket_should_contain_the_product,
                    Then_the_product_should_be_removed_from_stock)
                .RunAsync();
        }
    }
}