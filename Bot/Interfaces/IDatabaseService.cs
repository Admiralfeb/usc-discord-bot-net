using UnitedSystemsCooperative.Bot.Models;

namespace UnitedSystemsCooperative.Bot.Interfaces;

public interface IDatabaseService
{
    public Task<T> GetValueAsync<T>(string key);
    public Task SetValueAsync<T>(string key, T value);
    public Task<JoinRequest?> GetJoinRequest(string discordUserName);
    public Task SetEmail(string tag, string email);
}