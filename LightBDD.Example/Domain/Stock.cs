using System.Collections.Generic;
using LightBDD.Example.Helpers;

namespace LightBDD.Example.Domain
{
    public class Stock
    {
        private readonly List<string> _products = new List<string>();

        public IEnumerable<string> Products => _products;

        public void Add(string product)
        {
            _products.Add(product);
        }

        public bool TransferToBasket(Basket basket, string product)
        {
            LongRunningOperationSimulator.Simulate();
            if (!_products.Contains(product) || !_products.Remove(product))
                return false;
            basket.Add(product);
            return true;
        }
    }
}