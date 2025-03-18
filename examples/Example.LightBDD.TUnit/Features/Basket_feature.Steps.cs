using System.Linq;
using System.Threading.Tasks;
using Example.Domain.Domain;
using LightBDD.Framework;
using LightBDD.TUnit;

namespace Example.LightBDD.TUnit.Features
{
    public partial class Basket_feature : FeatureFixture
    {
        private Stock _stock = new Stock();
        private Basket _basket = new Basket();
        private bool _transferResult;
        private static readonly string Product = "product";

        private async Task Given_product_is_in_stock()
        {
            _stock.Add(Product);
            StepExecution.Current.Comment(string.Format("Added '{0}' to the stock", Product));
        }

        private async Task Given_product_is_out_of_stock()
        {
            //It is not added, so it is out of stock
        }

        private async Task When_customer_adds_it_to_the_basket()
        {
            StepExecution.Current.Comment(string.Format("Transferring '{0}' to the basket", Product));
            _transferResult = await _stock.TransferToBasketAsync(_basket, Product);
        }

        private async Task Then_the_product_addition_should_be_successful()
        {
            await Assert.That(_transferResult).IsTrue();
        }

        private async Task Then_the_basket_should_contain_the_product()
        {
            await Assert.That(_basket.Products.Contains(Product)).IsTrue();
        }

        private async Task Then_the_product_addition_should_be_unsuccessful()
        {
            await Assert.That(_transferResult).IsFalse();
        }

        private async Task Then_the_basket_should_not_contain_the_product()
        {
            await Assert.That(_basket.Products.Contains(Product)).IsFalse();
        }

        private async Task Then_the_product_should_be_removed_from_stock()
        {
            Skip.Test("Product removal from stock is not implemented yet");
            await Assert.That(_stock.Products.Contains(Product)).IsFalse();
        }
    }
}