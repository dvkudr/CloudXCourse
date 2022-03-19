using System.Collections.Concurrent;
using ProductService;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var products = new ConcurrentDictionary<string, Product>();

app
    .MapPut("/product/{ID}/add", (string id) =>
    {
        var product = new Product{ Id = id };
        if (products.TryAdd(id, product))
        {
            return Results.CreatedAtRoute("get_product", new { id });
        }

        return Results.Conflict();
    });

app
    .MapGet("/product/{ID}", (string id) =>
    {
        if (products.TryGetValue(id, out var product))
        {
            return Results.Ok(product);
        }

        return Results.NotFound();
    })
    .WithName("get_product");

app
    .MapGet("/healthcheck", () => Results.Ok());

app.Run();
