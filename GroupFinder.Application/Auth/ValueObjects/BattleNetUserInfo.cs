using GroupFinder.Application.Common;

namespace GroupFinder.Application.Auth.ValueObjects;

public sealed class BattleNetUserInfo
{
    public string Id { get; }
    public string BattleTag { get; }
    public string Region { get; }
    
    private static readonly HashSet<string> SupportedRegions = [ "eu" ];

    private BattleNetUserInfo(string id, string battleTag, string region)
    {
        Id = id;
        BattleTag = battleTag;
        Region = region;
    }

    public static Result<BattleNetUserInfo> Create(string id, string battleTag, string region)
    {
        if (string.IsNullOrWhiteSpace(id))
            return Result<BattleNetUserInfo>.Failure("BattleNetUserInfo: id is required.");
        
        if (string.IsNullOrWhiteSpace(battleTag))
            return Result<BattleNetUserInfo>.Failure("BattleNetUserInfo: battleTag is required.");
        
        if (string.IsNullOrWhiteSpace(region))
            return Result<BattleNetUserInfo>.Failure("BattleNetUserInfo: region is required.");
        
        // ReSharper disable once ConvertIfStatementToReturnStatement
        if (!SupportedRegions.Contains(region))
            return Result<BattleNetUserInfo>.Failure($"BattleNetUserInfo: region {region} is not supported.");
        
        return Result<BattleNetUserInfo>.Success(new BattleNetUserInfo(id, battleTag, region));
    }
}

