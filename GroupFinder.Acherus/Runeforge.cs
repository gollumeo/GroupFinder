using GroupFinder.Acherus.Contracts;
using GroupFinder.Acherus.Modules;
using Microsoft.AspNetCore.Builder;

namespace GroupFinder.Acherus;

public static class Runeforge
{
    public static async Task Inscribe(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        foreach (var rune in Frostmourne())
            rune.Extract(builder.Services, builder.Configuration);
        
        var app = builder.Build();

        foreach (var rune in Frostmourne())
            rune.CarveWith(app);

        await app.RunAsync();
    }

    private static IEnumerable<IRune> Frostmourne()
    {
        yield return new HttpRune();
        yield return new OpenApiRune();
        yield return new AuthRune();
    }
}