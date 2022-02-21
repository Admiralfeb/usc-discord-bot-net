using UnitedSystemsCooperative.Bot.Models;

namespace UnitedSystemsCooperative.Bot.Interfaces;

public interface IDatabaseService
{
    public Task<T> GetValueAsync<T>(string key) where T : DatabaseItemBase;
    public Task SetValueAsync<T>(string key, T value) where T : DatabaseItemBase;
    public Task<JoinRequest> GetJoinRequest(string discordUserName);
    public Task SetEmail(string tag, string email);
}
