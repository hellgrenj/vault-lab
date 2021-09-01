using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
    var response = new OperationResult(true, "Hello world");
    await context.Response.WriteAsJsonAsync(response);
}

public record OperationResult (bool Successful, string Message);
