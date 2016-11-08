Introduction
-------------

> ASP.NET Core is a new open-source and cross-platform framework for building modern cloud based internet connected applications, such as web apps, IoT apps and mobile backends.

> ASP.NET Core apps can run on .NET Core or on the full .NET Framework. It was architected to provide an optimized development framework for apps that are deployed to the cloud or run on-premises. It consists of modular components with minimal overhead, so you retain flexibility while constructing your solutions. You can develop and run your ASP.NET Core apps cross-platform on Windows, Mac and Linux. ASP.NET Core is open source at GitHub.

By now, you might have read a lot of articles about the new ASP.NET Core release, 1.0 (or 1.0.1, 1.1-preview or any other version of Core), so this will not be a step-by-step introductory session about it, but we will try to look at a few components of the framework, see how they work and interact with each other and a few very simple apps to demo these functionalities.

So, a few basic facts about the new ASP.NET Core to get us started:

- ASP .NET Core is a *complete re-write of the 4.6 framework* that came out last year and comes with a *completely new architecture based on .NET Core*

- ASP .NET Core is *no longer based on `System.Web`*. Instead, everything in the framework is modular and comes as NuGet packages which allows you to only include in your application the packages that you will use, resulting in a smaller application footprint and better performance

- it comes with integrated dependency injection, a new request pipeline middleware and the ability to plug in your own web server (IIS - Windows only or Kestrel inside your own process in Windows, macOS and Linux) and run across operating systems with very similar development processes and tools

- *if you have a working ASP.NET 4.x application in production (and you are happy about it), there is no reason to port it to ASP.NET Core (unless you need it to run on Linux servers)*

- it is still a very new framework, with a lot of libraries that have not yet been ported to .NET Core. So don't expect all libraries you used in .NET 4.x to be already present in .NET Core.



Now that we have the basics out of the way, we can get started in talking about the stuff in the title.


> If you want to [get started with the complete basics about ASP.NET Core, you can follow this article](https://radu-matei.github.io/blog/aspnet-core-getting-started/).

Every code sample in this meetup will start from the empty application that is built when you run the `dotnet new` command, without using any template built by Visual Studio or using the `-t web` parameters and will walk you through adding the necessary components. 