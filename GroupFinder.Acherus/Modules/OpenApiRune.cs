using GroupFinder.Acherus.Contracts;
using GroupFinder.Acherus.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GroupFinder.Acherus.Modules;

public class OpenApiRune : IRune
{
    public void Extract(IServiceCollection services, IConfiguration config)
    {
        services.AddOpenApi();
    }

    public void CarveWith(WebApplication app)
    {
      if (app.Environment.IsDevelopment())
            app.MapOpenApi();
    }
}