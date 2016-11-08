using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

public class Startup
{
    public void Configure(IApplicationBuilder app)
    {
        var routeBuilder = new RouteBuilder(app);

        routeBuilder.MapGet("", context => context.Response.WriteAsync("Hello from root!"));
        routeBuilder.MapGet("hello", context => context.Response.WriteAsync("Hello from /hello"));
        routeBuilder.MapGet("hello/{name}", context => 
        {
            var response = $"Hello, {context.GetRouteValue("name")}!";
            return context.Response.WriteAsync(response);
        });

        routeBuilder.MapGet("square/{number:int}", context =>
        {
            int number = Convert.ToInt32(context.GetRouteValue("number"));
            return context.Response.WriteAsync($"The square of {number} is {number * number}");
        });

        routeBuilder.MapPost("post/{parameter}", context =>
        {
            var response = $"Posting with {context.GetRouteValue("parameter")} as parameter";
            return context.Response.WriteAsync(response);
        });

        app.UseRouter(routeBuilder.Build());
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddRouting();
    }
}