using GroupFinder.Application.Common;

namespace GroupFinder.Infrastructure.Auth.ValueObjects;

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

    /// <summary>
    /// Creates a new instance of <see cref="BattleNetUserInfo"/> if the provided values are valid.
    /// </summary>
    /// <param name="id">The unique Battle.net identifier of the user. Cannot be null, empty, or whitespace.</param>
    /// <param name="battleTag">The BattleTag of the user. Cannot be null, empty, or whitespace.</param>
    /// <param name="region">The region of the user. Must be a supported region.</param>
    /// <returns>A <see cref="Result{T}"/> containing the <see cref="BattleNetUserInfo"/> instance if successful, or an error message if validation fails.</returns>
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

