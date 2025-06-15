using GroupFinder.Acherus.Modules;
using Rituals.Contracts;
using Rituals.Runeforge;

namespace GroupFinder.Api;

public class Runeforge : DormantRuneforge
{
    protected override IEnumerable<IRune> Frostmourne()
    {
        yield return new AuthRune();
    }
}