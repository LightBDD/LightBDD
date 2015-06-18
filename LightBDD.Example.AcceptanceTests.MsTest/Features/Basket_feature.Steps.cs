using System.Linq;
using LightBDD.Example.Domain;
using LightBDD.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LightBDD.Example.AcceptanceTests.MsTest.Features
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
            Assert.IsTrue(_transferResult);
        }

        private void Then_basket_contains_product()
        {
            Assert.IsTrue(_basket.Products.Contains("product"));
        }

        private void Then_product_addition_is_unsuccessful()
        {
            Assert.IsFalse(_transferResult);
        }

        private void Then_basket_does_not_contain_product()
        {
            Assert.IsFalse(_basket.Products.Contains("product"));
        }

        private void Then_product_is_removed_from_stock()
        {
            Assert.Inconclusive("Product removal from stock is not implemented yet");
            Assert.IsFalse(_stock.Products.Contains("product"));
        }

        #region Setup/Teardown

        [TestInitialize]
        public void SetUp()
        {
            _stock = new Stock();
            _basket = new Basket();
        }

        #endregion
    }
}