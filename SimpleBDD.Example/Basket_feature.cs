using NUnit.Framework;

namespace SimpleBDD.Example
{
	[TestFixture]
	[Description(
		@"In order to buy products
As a customer
I want to add products to basket")]
	public partial class Basket_feature
	{
		[Test]
		public void No_product_in_stock()
		{
			Runner.RunScenario(
				Given_product_is_out_of_stock,
				When_customer_adds_it_to_basket,
				Then_product_addition_is_unsuccessful,
				Then_basket_does_not_contain_product);
		}

		[Test]
		public void Successful_addition()
		{
			Runner.RunScenario(
				Given_product_is_in_stock,
				When_customer_adds_it_to_basket,
				Then_product_addition_is_successful,
				Then_basket_contains_product,
				Then_product_is_removed_from_stock);
		}
	}
}