using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;

namespace ProductService.Products;

internal class DynamoDbProductRepository : IProductRepository
{
    private readonly AmazonDynamoDBClient _client;
    private readonly string _tableName = "products";

    public DynamoDbProductRepository(string serviceUrl)
    {
        AmazonDynamoDBConfig clientConfig = new AmazonDynamoDBConfig();
        if (!string.IsNullOrEmpty(serviceUrl))
        {
            clientConfig.ServiceURL = "http://localhost:8000";
        }

        _client = new AmazonDynamoDBClient(clientConfig); 
    }

    public async Task<Product> Get(string id, CancellationToken cancellationToken)
    {
        Table products = Table.LoadTable(_client, _tableName);
        
        GetItemOperationConfig config = new GetItemOperationConfig
        {
            AttributesToGet = new List<string> { "Id" },
            ConsistentRead = true
        };
        Document document = await products.GetItemAsync(id, config, cancellationToken);
        return document != null
            ? new Product { Id = document["Id"] }
            : null;
    }

    public async Task<bool> Create(Product product, CancellationToken cancellationToken)
    {
        Table products = Table.LoadTable(_client, _tableName);

        Document document = new Document();
        document["Id"] = product.Id;

        await products.PutItemAsync(document, cancellationToken);

        return true;
    }
}
