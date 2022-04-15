using System.Collections.Concurrent;
using OpenTelemetry;
using OpenTelemetry.Contrib.Extensions.AWSXRay.Trace;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using ProductService;

AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

var products = new ConcurrentDictionary<string, Product>();

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    Sdk.CreateTracerProviderBuilder()
        .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("product-service").AddTelemetrySdk())
        .AddXRayTraceId()
        .AddAWSInstrumentation()
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
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
