using System.Linq;
using LightBDD.Example.Domain;
using LightBDD.Execution;
using Xunit;

namespace LightBDD.Example.AcceptanceTests.XUnit.Features
{
    public partial class Basket_feature : FeatureFixture
    {
        private Stock _stock;
        private Basket _basket;
        private bool _transferResult;

        private void Given_product_is_in_stock()
        {
            var product = "product";
            _stock.Add(product);
            StepExecution.Comment(string.Format("Added '{0}' to the stock", product));
        }

        private void Given_product_is_out_of_stock()
        {
            //It is not added, so it is out of stock
        }

        private void When_customer_adds_it_to_basket()
        {
            var product = "product";
            StepExecution.Comment(string.Format("Transferring '{0}' to the basket", product));
            _transferResult = _stock.TransferToBasket(_basket, product);
        }

        private void Then_product_addition_is_successful()
        {
            Assert.True(_transferResult);
        }

        private void Then_basket_contains_product()
        {
            Assert.True(_basket.Products.Contains("product"));
        }

        private void Then_product_addition_is_unsuccessful()
        {
            Assert.False(_transferResult);
        }

        private void Then_basket_does_not_contain_product()
        {
            Assert.False(_basket.Products.Contains("product"));
        }

        private void Then_product_is_removed_from_stock()
        {
            ScenarioAssert.Ignore("Product removal from stock is not implemented yet");
            Assert.False(_stock.Products.Contains("product"));
        }

        #region Setup/Teardown

        public Basket_feature()
        {
            _stock = new Stock();
            _basket = new Basket();
        }

        #endregion
    }
}