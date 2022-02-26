using Discord;
using Discord.WebSocket;

namespace UnitedSystemsCooperative.Bot;

public class BotSocketClient : DiscordSocketClient
{
    public BotSocketClient() : base()
    {

    }

    public new virtual async Task LoginAsync(TokenType tokenType, string token, bool validateToken = true)
    {
        await base.LoginAsync(tokenType, token, validateToken);
    }

    public override async Task StartAsync()
    {
        await base.StartAsync();
    }
}
