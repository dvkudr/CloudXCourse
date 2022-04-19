using System.Collections.Concurrent;

namespace ProductService.Products;

internal class InMemoryProductRepository : IProductRepository
{
    private readonly ConcurrentDictionary<string, Product> _products = new ();

    public Product Get(string id)
    {
        return _products.TryGetValue(id, out var product)
            ? product
            : null;
    }

    public bool Create(Product product)
    {
        return _products.TryAdd(product.Id, product);
    }
}
