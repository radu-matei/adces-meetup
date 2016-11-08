using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class Startup
{
    public void Configure(IApplicationBuilder app, ITest test)
    {
        app.Run(context =>
        {
            var response = test.DoSomething("startup");
            return context.Response.WriteAsync(response);
        });
    }

    public void ConfigureServices(IServiceCollection services)
    {
        ConfigureJsonServices(services);
    }

    private void ConfigureJsonServices(IServiceCollection services)
    {
        var jsonServices = JObject.Parse(File.ReadAllText("appSettings.json"))["services"];
        var requiredServices = JsonConvert.DeserializeObject<List<Service>>(jsonServices.ToString());

        foreach (var service in requiredServices)
        {
            services.Add(new ServiceDescriptor(serviceType: Type.GetType(service.ServiceType),
                                               implementationType: Type.GetType(service.ImplementationType),
                                               lifetime: service.Lifetime));
        }
    }
}