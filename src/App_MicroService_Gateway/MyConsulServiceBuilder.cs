using Consul;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Ocelot.Logging;
using Ocelot.Provider.Consul;
using Ocelot.Provider.Consul.Interfaces;

namespace App_MicroService_Gateway;

public class MyConsulServiceBuilder : DefaultConsulServiceBuilder
{
    public MyConsulServiceBuilder(Func<ConsulRegistryConfiguration> configurationFactory, IConsulClientFactory clientFactory, IOcelotLoggerFactory loggerFactory)
        : base(configurationFactory, clientFactory, loggerFactory) { }
    // I want to use the agent service IP address as the downstream hostname
    protected override string GetDownstreamHost(ServiceEntry entry, Node node) => entry.Service.Address;
}

public class SwaggerAggregatorMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IHttpClientFactory _httpClientFactory;

    public SwaggerAggregatorMiddleware(RequestDelegate next, IHttpClientFactory httpClientFactory)
    {
        _next = next;
        _httpClientFactory = httpClientFactory;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path.StartsWithSegments("/swagger/product"))
        {
            var client = _httpClientFactory.CreateClient();
            var productSwagger = await client.GetStringAsync("https://localhost:7293/swagger/v1/swagger.json");
            // var orderSwagger = await client.GetStringAsync("http://localhost:5002/swagger/v1/swagger.json");

            var productSwaggerDoc = JsonConvert.DeserializeObject<OpenApiDocument>(productSwagger);
            // // var orderSwaggerDoc = JsonConvert.DeserializeObject<OpenApiDocument>(orderSwagger);

            // Combine the Swagger documents
            var combinedSwaggerDoc = new OpenApiDocument
            {
                Info = new OpenApiInfo
                {
                    Title = "Combined API",
                    Version = "v1"
                },
                Paths = new OpenApiPaths()
            };

            foreach (var path in productSwaggerDoc!.Paths)
            {
                combinedSwaggerDoc.Paths.Add(path.Key, path.Value);
            }

            // foreach (var path in orderSwaggerDoc.Paths)
            // {
            //     combinedSwaggerDoc.Paths.Add(path.Key, path.Value);
            // }

            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonConvert.SerializeObject(combinedSwaggerDoc));
        }
        else
        {
            await _next(context);
        }
    }
}