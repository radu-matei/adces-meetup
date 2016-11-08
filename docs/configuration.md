The Configuration Engine
========================

Introduction
---------------

In the previous versions of ASP .NET, any configuration setting or parameter you needed was added in `web.config` [(complete description of the old `web.config` file)](http://www.codeproject.com/Articles/301726/Web-config-File-ASP-NET), or added in a separate XML file and referenced in `web.config` (for scenarios like database connection strings, or storing APIs access tokens).

The new configuration system provides support for JSON, XML and INI, while also allowing you to [create your custom configuration provider](https://docs.asp.net/en/latest/fundamentals/configuration.html#custom-config-providers) and for in-memory configuration.

> For a detailed view of ASP .NET Core Configuration system, read the [Official ASP .NET Documentation](https://docs.asp.net/en/latest/fundamentals/configuration.html).

[In the tutorial about Routing](https://radu-matei.github.io/blog/aspnet-core-routing/), we created a very simple web application that used the Routing service to respond to some requests, but they were hardcoded and if we wanted to change the response message, we would have had to recompile the entire application.
> For more information about Routing, check the [ASP .NET Core Routing Tutorial](https://radu-matei.github.io/blog/aspnet-core-routing/).

Let's assume that in our application we want the response messages not to be hardcoded in `Startup` anymore, but stored in a configuration file so we don't have to stop, modify or recompile our application every time the messages or the routes change.

More clearly, we want to map the keys from the JSON file below as routes and the values as the responses we want our application to give, so when a user browses in our application to `/some-route`, if `some-route` is present in the JSON configuration file, then the response will be the value from the file, if else to display a default message.

```
{
    "hi": "Hi!",
    "hello": "Hello!",
    "bye": "Goodbye!",
    "default": "This is default!"
}
```

Also, if we modify the configuration file while the application is running, our application should be able to use the latest configuration. (We don't actually do much here, it's just a constructor parameter we set to `true` or `false`).

There can be any number of defined paths in our configuration file and they can change with any frequency (so that hard-coding them in our application is not an option).

Using the ASP .NET Core JSON Configuration Provider
----------------------------------------------------

As we said earlier, the new ASP .NET implements a JSON configuration provider that allows us to read and use configurations from JSON files (and not only), and we can have strong typing (where we define classes for our configuration settings) or we can access them using directly their key.

> For a complete article on creating [Strongly Typed Configuration Settings in ASP .NET Core, see this article by Rick Strahl](https://weblog.west-wind.com/posts/2016/May/23/Strongly-Typed-Configuration-Settings-in-ASPNET-Core).

To use configuration settings in ASP .NET Core, simply instantiate a `Configuration` object (using a `ConfigurationBuilder`)  and indicate the source of the JSON file.

> At its simplest, `Configuration` is just a collection of sources, which provide the ability to read and write name/value pairs. If a name/value pair is written to Configuration, it is not persisted. This means that the written value will be lost when the sources are read again.

> Developers are not limited to using a single configuration source. In fact several may be set up together such that a default configuration is overridden by settings from another source if they are present.

> [The Official ASP .NET Core Documentation](https://docs.asp.net/en/latest/fundamentals/configuration.html#using-the-built-in-sources)


Because at the time when we write the application we can't know the exact paths, we will not create stronyly-typed configurations but we will take the path from our application an check to see wether that path exists in our configuration file.

Building the configurable Greeting service
------------------------------------------

First of all, [follow all the steps in order to create an ASP .NET Core application with a `Startup` class from this tutorial](https://radu-matei.github.io/blog/aspnet-core-startup/), that means:

- create new app using `dotnet new`
- add the ` "Microsoft.AspNetCore.Server.Kestrel": "1.0.1"` NuGet package
- add an empty `Startup` class


Then, we create a new file, `greetings.json` in the same folder as our `Program.cs`, `Startup.cs` and `project.json` files, where we add our custom routes and messages we want our application to respond with.

```
{
    "hi": "Hi!",
    "hello": "Hello!",
    "bye": "Goodbye!",
    "default": "This is default!"
}
```
Now we need to add another NuGet package, `"Microsoft.Extensions.Configuration.Json": "1.0.1"` that will contain the necessary methods for using JSON files as configuration.

In `Startup` we create a property of type `IConfiguration` where we will keep our configuration files: `public IConfiguration Configuration { get;set; }`

Then, we add a constructor for the `Startup` class that will instantiate a `ConfigurationBuilder` that will actually get the configuration information in our `Configuration` property.

```
    public Startup(IHostingEnvironment env)
    {
        var configurationBuilder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("greetings.json", optional: false, reloadOnChange: true);
        
        Configuration = configurationBuilder.Build();
    }
```

> The constructor has an `IHostingEnvironment` parameter that is used to establish the directory of the `json` configuration file. Since we placed it in the same folder as the other files, we can simply get the current directory: `Directory.GetCurrentDirectory()`.

After we instantiate the `ConfigurationBuilder` we chain two method calls - one for establishing the directory of the configuration file, the other for determing the actual name of the file.

The `.AddJsonFile()` method takes three arguments in this case:

- the name of the file - in our case `greetings.json`
- a `bool` that determines wether this configuration file is optional or not, used to determine the order in which the system searches the files (if there are multiple files) if the same configuration name exists in multiple files.
- a `bool` that specifies what happens if the configuration file is modified while the application is running - `reloadOnChange`
After this, we set our `Configuration` property to what the `configurationBuilder` "builds".


The next step is to add the "`Microsoft.AspNetCore.Routing": "1.0.0"` package in `project.json`, create the `Configure` method in `Startup` and add the routing service.

```
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddRouting();
    }
```

> As you write code in Visual Studio or Visual Studio Code, you can press Ctrl + . (period) to show suggestions for errors (like adding `using` statements for you).


Then, we create the `Configure` method where we will hook up incoming requests and map them with the routes from our configuration file.

```
    public void Configure(IApplicationBuilder app)
    {
        var routeBuilder = new RouteBuilder(app);

        routeBuilder.MapGet("{route}", context => 
        {
            var routeMessage = Configuration.AsEnumerable()
                .FirstOrDefault(r => r.Key == context.GetRouteValue("route")
                .ToString())
                .Value;
            
            var defaultMessage = Configuration.AsEnumerable()
                .FirstOrDefault(r => r.Key == "default")
                .Value;

            var response = (routeMessage != null) ? routeMessage : defaultMessage;

            return context.Response.WriteAsync(response);
        });

        app.UseRouter(routeBuilder.Build());
    }
```

We create a new `RouteBuilder` and map a new GET `route` in [the same way as in this article](https://radu-matei.github.io/blog/aspnet-core-routing/).

```
            var routeMessage = Configuration.AsEnumerable()
                .FirstOrDefault(r => r.Key == context.GetRouteValue("route")
                .ToString())
                .Value;
```

We know that our configuration is now accessible through the `Configuration` property that we populated in the `Startup` constructor, and the configuration settings are an `IEnumerable<KeyValuePair<string, string>>`, that is a collection of key-value pairs of strings, so we can use Linq to search for the key-value pair in our file that has the same key as our path and take the value from that pair.

> For some examples of using Linq with lambdas [check this article from Code Magazine](http://www.codemag.com/article/1001051).

We also search for the default message in our JSON so that if the path does not exist in the file, we have a standard respone.

```
            var defaultMessage = Configuration.AsEnumerable()
                .FirstOrDefault(r => r.Key == "default")
                .Value;
```

Then, depending on wether the route actually exists in our configuration file or not, we return either the message of that specific route, or the default message.

If we run the application and open a browser, we can check if our routing works:

```
http://localhost:5000/hello - This should display Hello!
http://localhost:5000/hi - This should display Hi!
http://localhost:5000/bye - This should display Goodbye!
http://localhost:5000/default or http://localhost:5000/anything-else - This should display This is default!
```

Without closing the application, go to `greeting.json` and either add a new key-value pair, or modify an existing one's value, save the file and navigate to that path.

Normally, you should see that our application was able to load the configuration file without restarting or recompiling.


> Our application works, but there is a lot of logic code in `Startup`, a place for configuration. 
> [In the next article, we will use the dependency injection engine and extract the code from startup, make a service out of it and inject it in our application.](configuration-dependency-injection.md)
