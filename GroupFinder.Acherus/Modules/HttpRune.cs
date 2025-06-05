using GroupFinder.Acherus.Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GroupFinder.Acherus.Modules;

public class HttpRune : IRune
{
    public void Extract(IServiceCollection services, IConfiguration config)
    {
        services.AddControllers();
    }

    public void CarveWith(WebApplication app)
    {
        app.MapControllers();
        app.UseAuthorization();
    }
}