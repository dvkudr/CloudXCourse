namespace ProductService.Products;

public interface IProductRepository
{
    Product Get(string id);

    bool Create(Product product);
}
