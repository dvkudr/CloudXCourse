namespace ProductService.Products;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddProductRepository(this IServiceCollection services, IConfigurationSection configuration)
    {
        var implementation = configuration["Implementation"];
        switch (implementation)
        {
            case "InMemory":
                services
                    .AddSingleton<IProductRepository>(new InMemoryProductRepository());
                break;
            case "DynamoDB":
                var dynamoDbConfig = configuration.GetSection("DynamoDB");
                var serviceUrl = dynamoDbConfig?["ServiceUrl"];
                services
                    .AddSingleton<IProductRepository>(new DynamoDbProductRepository(serviceUrl));
                break;
        }

        return services;
    }
}