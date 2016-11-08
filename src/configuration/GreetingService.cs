using System.Linq;
using Microsoft.Extensions.Configuration;

public class GreeringService : IGreetingService
{
    private IConfiguration Configuration { get;set; }

    public GreeringService(IConfiguration configuration)
    {
        Configuration = configuration;
    }
    public string Greet(string route)
    {
            var routeMessage = Configuration.AsEnumerable()
                .FirstOrDefault(r => r.Key == route)
                .Value;
            
            var defaultMessage = Configuration.AsEnumerable()
                .FirstOrDefault(r => r.Key == "default")
                .Value;

            return (routeMessage != null) ? routeMessage : defaultMessage;
    }
}