using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var jsonString = File.ReadAllText("/vault/secrets/api");
var secrets = JsonSerializer.Deserialize<JsonElement>(jsonString);
var data = secrets.GetProperty("data");
var theSecret = data.GetProperty("pw").ToString();


// configure and start web server
await Host.CreateDefaultBuilder(args)
              .ConfigureLogging(logging =>
              {
                  // Add any 3rd party loggers like NLog or Serilog
              })
              .ConfigureServices((hostContext, services) =>
              {
                  services.AddRouting();
              })
              .ConfigureWebHost(webBuilder =>
              {
                  webBuilder.UseKestrel().ConfigureKestrel(options =>
                  {
                      options.Limits.MinRequestBodyDataRate = null;
                      options.ListenAnyIP(Environment.GetEnvironmentVariable("PORT") != null ? Convert.ToInt32(Environment.GetEnvironmentVariable("PORT")) : 8080);
                  });
                  webBuilder.Configure(app =>
                  {

                      app.UseRouting();
                      app.UseEndpoints(e => Routes(e));
                  });
              })
              .RunConsoleAsync();

IEndpointRouteBuilder Routes(IEndpointRouteBuilder e)
{
    e.MapGet("/", context => IndexHandler(context));
    return e;
}
async Task IndexHandler(HttpContext context)
{
    var response = new OperationResult(true, theSecret);
    await context.Response.WriteAsJsonAsync(response);
}

public record OperationResult (bool Successful, string Message);
