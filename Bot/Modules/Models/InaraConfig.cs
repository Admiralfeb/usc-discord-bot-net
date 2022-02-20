namespace UnitedSystemsCooperative.Bot.Modules.Models;

// Disable nullable warning.
#pragma warning disable CS8618

public class InaraConfig
{
    public const string ConfigName = "InaraConfig";
    public string Token { get; set; }
    public string ApiUrl { get; set; }
}
