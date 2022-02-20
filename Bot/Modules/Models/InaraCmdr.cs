namespace UnitedSystemsCooperative.Bot.Modules.Models;

// Disable nullable warning.
#pragma warning disable CS8618
public class InaraCmdr
{
    public long UserId { get; set; }
    public string UserName { get; set; }
    public string? CommanderName { get; set; }
    public IEnumerable<CommanderPilotRank> CommanderRanksPilot { get; set; }
    public string PreferredAllegianceName { get; set; }
    public CmdrMainShip? CommanderMainShip { get; set; }
    public CmdrSquadron? CommanderSquadron { get; set; }
    public CmdrSquadron? CommanderWing { get; set; }
    public string? PreferredPowerName { get; set; }
    public string PreferredGameRole { get; set; }
    public string? AvatarImageUrl { get; set; }
    public string InaraUrl { get; set; }
    public IEnumerable<string>? OtherNamesFound { get; set; }
}

public class CommanderPilotRank
{
    public string RankName { get; set; }
    public int RankValue { get; set; }
    public double RankProgress { get; set; }
}

public class CmdrMainShip
{
    public string ShipType { get; set; }
    public string ShipName { get; set; }
    public string ShipIdent { get; set; }
    public string ShipRole { get; set; }
}

public class CmdrSquadron
{
    public int SquadronId { get; set; }
    public string SquadronName { get; set; }
    public int SquadronMembersCount { get; set; }
    public string SquadronMemberRank { get; set; }
    public string InaraUrl { get; set; }
}
