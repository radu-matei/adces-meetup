The Routing Engine
==================

Introduction
------------

> In this document we will deal with the low-level routing middleware for ASP.NET Core and not the MVC Core routing.

> The best place to start learning about routing is [the Official ASP.NET Core Documentation](https://docs.asp.net/en/latest/fundamentals/routing.html).

> Routing is used to map requests to route handlers. Routes are configured when the application starts up, and can extract values from the URL that will be used for request processing. Routing functionality is also responsible for generating links using the defined routes in ASP.NET apps.

Not all web applications need the full MVC / MVC Core framework - some applications might only need a very lightweight package for mapping few predefined routes to some methods - but most of them end up using the entire framework.

Installing the `Routing` package
-----------------------------------------

First of all, we need to add the `Microsoft.AspNetCore.Routing` dependency from NuGet. 

```
      "dependencies": {
        "Microsoft.NETCore.App": {
          "type": "platform",
          "version": "1.0.1"
        },
        "Microsoft.AspNetCore.Server.Kestrel": "1.0.1",
        "Microsoft.AspNetCore.Routing": "1.0.1"
      }
```

This is how the `dependencies` node of `project.json` should look like.


> At the time of writing this article, the latest version for all ASP .NET Core libraries is `1.0.1`. As newer versions are released, check the release notes to see if there are any breaking changes when updating packages.


Adding the `Routing` Service in `Startup`
-----------------------------------------------------

When we discussed [the anatomy of the `Startup` class](https://radu-matei.github.io/blog/aspnet-core-startup/), besides the `Configure` method we have used before, there was also a method called `ConfigureServices` used for configuring services that our application needs.

Since we are going to use Routing, we should add it as a service in `Startup`.

```
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddRouting();
    }
```

Adding and handling custom routes
-------------------------------------------

First of all, in the `Configure` method from `Startup` we need to instantiate a new instance of the `RouteBuilder` class that will allow us to build custom routes and handle them.

```
var routeBuilder = new RouteBuilder(app);
```

We will then use this instance to map requests to to specific paths with our request handlers, allowing us to handle`GET` and `POST` requests from clients.

The way to map a `GET` request is to use the `MapGet` method from `RouteBuilder`. Mapping the application root - http://localhost:5000 is done through:

```
routeBuilder.MapGet("", context => context.Response.WriteAsync("Hello from root!"));
```

Mapping specific paths for `GET` - for example http://localhost:5000/hello is done in the following way.

```
routeBuilder.MapGet("hello", context => context.Response.WriteAsync("Hello from /hello"));
```

We can create paths that contain multiple elements and we can extract the parameters entered when making the request. For example, when requesting on `hello/{name}`, we can extract the parameter `{name}` and use it when constructing the response:

```
routeBuilder.MapGet("hello/{name}", context => context.Response
                                                      .WriteAsync($"Hello, {context.GetRouteValue("name")}"));
```

We can also add constrains on the parameters. For example, let's create a respond for requests coming to the path `/square/{number}`, where `{number}` is an `int` and responds with the square of the number.

```
routeBuilder.MapGet("square/{number:int}", context =>
        {
            int number = Convert.ToInt32(context.GetRouteValue("number"));
            return context.Response.WriteAsync($"The square of {number} is {number * number}");
        });
```

> For a full list of parameter constraints, see this table from [the Official ASP .NET Core Documentation](https://docs.asp.net/en/latest/fundamentals/routing.html#id7).


In order to test wether the routing works properly, open a browser and navigate to your custom route and check if the output is the desired one.

> You can also place some breakpoint inside the custom route handlers and iterate through the handlers step-by-step, watching how the response is formed.

```
http://localhost:5000
http://localhost:5000/hello
http://localhost:5000/hello/John
http://localhost:5000/square/3
```

So far we only created routing mapped for the `GET` method, so we can test the output from a browser tab.
Now we will add routing for a `POST` method (so we will not be able to test it by navigating to the URL in the browser).

```
        routeBuilder.MapPost("post/{parameter}", context =>
        {
            var response = $"Posting with {context.GetRouteValue("parameter")} as parameter";
            return context.Response.WriteAsync(response);
        });
```

Conclusion
----------

In this very simple example, we created a web application in which we defined and handled custom routes. We also saw how to manage `GET` and `POST` requests.

[You can see the completed project here](https://github.com/radu-matei/adces-meetup/tree/master/src/routing).