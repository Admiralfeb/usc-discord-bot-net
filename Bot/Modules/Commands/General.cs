using Discord;
using Discord.Interactions;

namespace UnitedSystemsCooperative.Bot.Modules.Commands;

public class GeneralCommandsModule : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("caniscthere", "Can I supercruise there?")]
    public async Task CanIScThere()
    {
        var builder = new ComponentBuilder()
            .WithButton("Click Me to find out fi you can supercruise there.",
                style: ButtonStyle.Link,
                url: "https://caniflytothenextstarinelitedangero.us/");

        await RespondAsync("Click the button below.", components: builder.Build());
    }

    [SlashCommand("heresy", "Stop your heresy")]
    public async Task Heresy()
    {
        await RespondAsync("https://tenor.com/view/cease-your-heresy-warhammer-40k-gif-19005947");
    }
}