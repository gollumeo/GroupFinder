using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace GroupFinder.Acherus.Extensions;

public static class OpenApiExtensions
{
    public static IServiceCollection AddOpenApi(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo()
            {
                Title = "Group Finder API",
                Version = "v1"
            });
        });

        return services;
    }
}