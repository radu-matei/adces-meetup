using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public class Startup
{
    public void Configure(IApplicationBuilder app, IGreetingService greetingService)
    {
        var routeBuilder = new RouteBuilder(app);

        routeBuilder.MapGet("{route}", context => 
        {
            var route = context.GetRouteValue("route").ToString();
            return context.Response.WriteAsync(greetingService.Greet(route));
        });

        app.UseRouter(routeBuilder.Build());
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddRouting();

        services.Add(new ServiceDescriptor(typeof(IConfiguration), 
                     provider => new ConfigurationBuilder()
                                    .SetBasePath(Directory.GetCurrentDirectory())
                                    .AddJsonFile("greetings.json", 
                                                 optional: false, 
                                                 reloadOnChange: true)
                                    .Build(), 
                     ServiceLifetime.Singleton));

        services.AddTransient<IGreetingService, GreeringService>();
    }
}