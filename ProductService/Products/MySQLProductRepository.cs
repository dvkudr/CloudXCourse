using MySql.Data.MySqlClient;

namespace ProductService.Products;

internal class MySqlProductRepository : IProductRepository
{
    readonly MySqlConnection _connection;

    public MySqlProductRepository(string connectionString)
    {
        _connection = new MySqlConnection(connectionString);
        _connection.Open();
    }

    public async Task<Product> Get(string id, CancellationToken cancellationToken = default)
    {
        var commandText = $"SELECT id FROM products WHERE id={id}";
        await using var cmd = new MySqlCommand(commandText, _connection);

        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);

        if (await reader.ReadAsync(cancellationToken))
        {
            return new Product
            {
                Id = reader[0].ToString()
            };
        }

        return null;
    }

    public async Task<bool> Create(Product product, CancellationToken cancellationToken = default)
    {
        var existingProduct = await Get(product.Id, cancellationToken);
        if (existingProduct != null)
        {
            return false;
        }

        var commandText = $"INSERT INTO products (id) VALUES ({product.Id})";
        await using var cmd = new MySqlCommand(commandText, _connection);
        await cmd.ExecuteNonQueryAsync(cancellationToken);
        return true;
    }
}
