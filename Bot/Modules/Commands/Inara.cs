using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace UnitedSystemsCooperative.Bot.Modules.Commands;

[Group("inara", "Inara Commands")]
public class InaraCommandModule : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("cmdr", "Get cmdr data")]
    public async Task GetCmdrData(string cmdrName)
    {
        IGuildUser user = (SocketGuildUser)Context.User;
        var inaraResponse = GetInaraCmdr(cmdrName, user);
    }

    private async Task GetInaraCmdr(string cmdrName, IGuildUser user)
    {

    }
}
