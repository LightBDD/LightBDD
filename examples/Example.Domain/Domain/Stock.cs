using System.Collections.Generic;
using System.Threading.Tasks;
using Example.Domain.Helpers;

namespace Example.Domain.Domain
{
    public class Stock
    {
        private readonly List<string> _products = new List<string>();

        public IEnumerable<string> Products => _products;

        public void Add(string product)
        {
            _products.Add(product);
        }

        public async Task<bool> TransferToBasketAsync(Basket basket, string product)
        {
            await LongRunningOperationSimulator.SimulateAsync();
            if (!_products.Contains(product) || !_products.Remove(product))
                return false;
            basket.Add(product);
            return true;
        }
    }
}