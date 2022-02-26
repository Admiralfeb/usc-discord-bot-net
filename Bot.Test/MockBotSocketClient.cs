using System.Threading.Tasks;
using Discord;
using UnitedSystemsCooperative.Bot;

namespace UnitedSystemsCooperative.Bot.Test;

public class TestBotSocketClient : BotSocketClient
{
    public TestBotSocketClient() : base() { }

    public override async Task LoginAsync(TokenType tokenType, string token, bool validateToken = true)
    {
        await Task.Run(() =>
        {
            // do nothing
        });
    }

    public override async Task StartAsync()
    {
        await base.StartAsync();
    }
}
