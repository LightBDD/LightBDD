using Example.LightBDD.MsTest3.Features.Contexts;
using LightBDD.Framework;
using LightBDD.Framework.Scenarios;
using LightBDD.MsTest3;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Example.LightBDD.MsTest3.Features
{
    /// <summary>
    /// This feature class describes usage of contextual scenarios and extended syntax.
    /// With contextual scenarios it is possible to share state between all steps, which is useful in the situation where
    /// sharing state via feature class fields is not desired.
    /// 
    /// To use contextual scenarios, the Runner.WithContext() method has to be used and the context object can be then accessed by step with lambda parameter.
    /// The scenario below shows the recommended usage of context, where given-when-then steps are declared on the context class (it is however not enforced rule).
    /// 
    /// The lambda parameter name is used here to determine the step type (GIVEN/WHEN/THEN/etc).
    /// 
    /// More information on contextual scenarios can be found here: https://github.com/LightBDD/LightBDD/wiki/Scenario-Steps-Definition#contextual-scenarios
    /// </summary>
    [TestClass]
    [FeatureDescription(
@"In order to deliver products to customer effectively
As a spedition manager
I want to dispatch products to customer as soon as the payment is finalized")]
    [Label("Story-3")]
    public class Product_spedition_feature : FeatureFixture
    {
        [Scenario]
        [Label("Ticket-5")]
        [ScenarioCategory(Categories.Sales)]
        [ScenarioCategory(Categories.Delivery)]
        public void Should_dispatch_product_after_payment_is_finalized()
        {
            Runner.WithContext<SpeditionContext>().RunScenario(
                given => given.There_is_an_active_customer_with_id("ABC-123"),
                and => and.The_customer_has_product_in_basket("wooden shelf"),
                and => and.The_customer_has_product_in_basket("wooden desk"),
                when => when.The_customer_payment_finalizes(),
                then => then.Product_should_be_dispatched_to_the_customer("wooden shelf"),
                and => and.Product_should_be_dispatched_to_the_customer("wooden desk"));
        }
    }
}