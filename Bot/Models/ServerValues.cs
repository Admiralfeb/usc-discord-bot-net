namespace UnitedSystemsCooperative.Bot.Models;

public class ServerValues
{
    public const string ConfigName = "ServerValues";
    public string GalNetChannelKey { get; set; }
    public string GalNetTitlesKey { get; set; }
    public ulong JoinChannelId { get; set; }
}