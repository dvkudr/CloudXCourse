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
                    .AddSingleton<IProductRepository, InMemoryProductRepository>();
                break;
        }

        return services;
    }
}
