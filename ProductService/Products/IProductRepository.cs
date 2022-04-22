namespace ProductService.Products;

public interface IProductRepository
{
    Task<Product> Get(string id, CancellationToken cancellationToken = default);

    Task<bool> Create(Product product, CancellationToken cancellationToken = default);
}
