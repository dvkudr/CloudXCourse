using System.Diagnostics;
using OpenTelemetry;
using OpenTelemetry.Contrib.Extensions.AWSXRay.Trace;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using ProductService.Products;

AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen();

builder.Services.AddProductRepository(
    builder.Configuration.GetSection("ProductService:ProductRepository"),
    builder.Configuration.GetSection("ConnectionStrings"));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    Sdk.CreateTracerProviderBuilder()
        .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("product-service").AddTelemetrySdk())
        .AddXRayTraceId()
        .AddAWSInstrumentation()
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddSqlClientInstrumentation(
            options =>
            {
                options.SetDbStatementForStoredProcedure = true;
                options.SetDbStatementForText = true;
            })
        .AddOtlpExporter(options =>
        {
            options.Endpoint = new Uri(Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT"));
        })
        .Build();

    Sdk.SetDefaultTextMapPropagator(new AWSXRayPropagator());
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty;
    });
}

app
    .MapPut("/product/{ID}/add", async (string id, IProductRepository productRepository) =>
    {
        var product = new Product{ Id = id };
        return await productRepository.Create(product)
            ? Results.CreatedAtRoute("get_product", new { id })
            : Results.Conflict();
    })
    .WithName("create_product");

app
    .MapGet("/product/{ID}", async (string id, IProductRepository productRepository) =>
    {
        var product = await productRepository.Get(id);
        return product != null
            ? Results.Ok(product)
            : Results.NotFound();
    })
    .WithName("get_product");

app
    .MapGet("/healthcheck", () => Results.Ok());

app.Run();
