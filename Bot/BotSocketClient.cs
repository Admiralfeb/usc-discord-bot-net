using Discord;
using Discord.WebSocket;
using UnitedSystemsCooperative.Bot.Interfaces;

namespace UnitedSystemsCooperative.Bot;

public class BotSocketClient : DiscordSocketClient
{
    public BotSocketClient() : base()
    {

    }

    public new async Task LoginAsync(TokenType tokenType, string token, bool validateToken = true)
    {
        await base.LoginAsync(tokenType, token, validateToken);
    }

    public override async Task StartAsync()
    {
        await base.StartAsync();
    }
}
