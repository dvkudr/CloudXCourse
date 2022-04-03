using System.Collections.Concurrent;
using OpenTelemetry;
using OpenTelemetry.Contrib.Extensions.AWSXRay.Trace;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using ProductService;

var builder = WebApplication.CreateBuilder(args);

AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

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

var app = builder.Build();

var products = new ConcurrentDictionary<string, Product>();

app
    .MapPut("/product/{ID}/add", (string id) =>
    {
        /*FindPrimeNumber.Run(100000);*/
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
        /*FindPrimeNumber.Run(100000);*/
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
