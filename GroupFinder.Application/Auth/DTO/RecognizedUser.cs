namespace GroupFinder.Application.Auth.DTO;

public sealed class RecognizedUser(Guid id, string battleTag, string region)
{
    public Guid Id { get; } = id;
    public string BattleTag { get; } = battleTag;
    public string Region { get; } = region;
}