using GroupFinder.Application.Auth.Contracts;
using GroupFinder.Infrastructure.Auth;
using GroupFinder.Infrastructure.Auth.Contracts;
using GroupFinder.Infrastructure.Auth.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rituals.Contracts;

namespace GroupFinder.Acherus.Modules;

public class AuthRune : IRune
{
    public void Extract(IServiceCollection services, IConfiguration config)
    {
        services.Configure<BattleNetOAuthOptions>(
            config.GetSection("BattleNetOAuth")
        );

        services.AddHttpClient<IBattleNetApi, BattleNetApiClient>();
        services.AddScoped<IExternalAuthentication, BattleNetAuthProxy>();
    }

    public void CarveWith(WebApplication app)
    {
        // No middleware to boot here, yet
    }
}