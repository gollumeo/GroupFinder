using GroupFinder.Acherus.Contracts;
using GroupFinder.Application.Auth.Contracts;
using GroupFinder.Infrastructure.Auth;
using GroupFinder.Infrastructure.Auth.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GroupFinder.Acherus.Modules;

public class AuthRune : IRune
{
    public void Extract(IServiceCollection services, IConfiguration config)
    {
        services.Configure<BattleNetOAuthOptions>(
            config.GetSection("BattleNetOAuth")
        );

        services.AddHttpClient<BattleNetAuthProxy>();
        services.AddScoped<IExternalAuthentication, BattleNetAuthProxy>();
    }

    public void CarveWith(WebApplication app)
    {
        // No middleware to boot here, yet
    }
}