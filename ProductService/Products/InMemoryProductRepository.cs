using System.Collections.Concurrent;

namespace ProductService.Products;

internal class InMemoryProductRepository : IProductRepository
{
    private readonly ConcurrentDictionary<string, Product> _products = new ();

    public Task<Product> Get(string id, CancellationToken cancellationToken)
    {
        var result = _products.TryGetValue(id, out var product)
            ? product
            : null;

        return Task.FromResult(result);
    }

    public Task<bool> Create(Product product, CancellationToken cancellationToken)
    {
        return Task.FromResult(_products.TryAdd(product.Id, product));
    }
}
