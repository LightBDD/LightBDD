using LightBDD.Framework;
using LightBDD.Framework.Scenarios;
using LightBDD.MsTest3;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Example.LightBDD.MsTest3.Features
{
    /// <summary>
    /// This feature class shows presents an usage of extended scenario syntax that allows to call parameterized steps.
    /// It also presents usage of LabelAttribute, ScenarioCategoryAttribute and FeatureDescriptionAttribute.
    /// 
    /// More information on extended syntax can be found here: https://github.com/LightBDD/LightBDD/wiki/Scenario-Steps-Definition#extended-scenarios
    /// while attribute usage is described here: https://github.com/LightBDD/LightBDD/wiki/Tests-Structure-and-Conventions
    /// </summary>
    [TestClass]
    [FeatureDescription(
@"In order to pay for products
As a customer
I want to receive invoice for bought items")]
    [Label("Story-2")]
    public partial class Invoice_feature
    {
        [Scenario]
        [Label("Ticket-4")]
        [ScenarioCategory(Categories.Sales)]
        public void Receiving_invoice_for_products()
        {
            Runner.RunScenario(
                _ => Given_product_is_available_in_product_storage("wooden desk"),
                _ => Given_product_is_available_in_product_storage("wooden shelf"),
                _ => When_customer_buys_product("wooden desk"),
                _ => When_customer_buys_product("wooden shelf"),
                _ => Then_an_invoice_should_be_sent_to_the_customer(),
                _ => Then_the_invoice_should_contain_product_with_price_of_AMOUNT("wooden desk", 62),
                _ => Then_the_invoice_should_contain_product_with_price_of_AMOUNT("wooden shelf", 37));
        }
    }
}