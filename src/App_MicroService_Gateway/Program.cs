using App_MicroService_Gateway;
using App_MicroService_Gateway.Configs;
using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.OpenApi.Models;
using Ocelot.Configuration.File;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Consul;
using Ocelot.Provider.Consul.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen(c =>
//         {
//             c.SwaggerDoc("gateway", new OpenApiInfo { Title = "API Gateway", Version = "v1" });
//             // Add any other service-specific Swagger docs here if needed
//         }); builder.Services.AddHttpClient();

builder.Services.AddSwaggerForOcelot(builder.Configuration, p=> {
    p.GenerateDocsForGatewayItSelf = true;
});

builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
builder.Services.AddOcelot(builder.Configuration)
    .AddConsul<MyConsulServiceBuilder>();
//.AddConsul();
var app = builder.Build();
// var configuration = app.Services.GetRequiredService<IConfiguration>();
// var clientFactory = app.Services.GetRequiredService<IHttpClientFactory>();
// var _httpClient = clientFactory.CreateClient("Test");
// app.UseSwagger();
// app.UseSwaggerUI(setup =>
// {
//     setup.RoutePrefix = "docs";
//     foreach (var item in configuration.Get<FileConfiguration>()!.Routes)
//     {
//         var address = item.DownstreamHostAndPorts.First(); // https://localhost:3322
//         var url = $"{item.DownstreamScheme}://{address.Host}:{address.Port}/v1/swagger.json";  //get actual url to service swagger json

//         var jsonEndpoint = $"/{item.Key}/v1/swagger.json";  //service.Key is name of sevice from route, so we got url to json of service through gateway


//         app.Map(jsonEndpoint, p =>
//         {
//             return Task.Run(async () =>
//              {
//                  string content = await _httpClient.GetStringAsync(url);
//                  await p.Response.WriteAsync(content);
//              });
//         });

//         setup.SwaggerEndpoint(jsonEndpoint, item.Key);
//     }
// });

// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// app.UseMiddleware<SwaggerAggregatorMiddleware>();
app.UseSwaggerForOcelotUI().UseOcelot().Wait();

app.Run();
