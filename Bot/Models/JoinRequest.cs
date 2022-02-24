#pragma warning disable CS8618

namespace UnitedSystemsCooperative.Bot.Models;

public class JoinRequest
{
    public string Type { get; set; }
    public string Cmdr { get; set; }
    public string Discord { get; set; }
    public PlatformType Platform { get; set; }
}
