using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GroupFinder.Acherus.Contracts;

public interface IRune
{
    void Extract(IServiceCollection services, IConfiguration config);

    void CarveWith(WebApplication app);
}