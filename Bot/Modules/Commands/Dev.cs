using Discord;
using Discord.Interactions;

namespace UnitedSystemsCooperative.Bot.Modules.Commands;

[Group("dev", "Bot's dev functions")]
public class DevCommandModule : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("bot", "List the bot's github")]
    public async Task ListBotGithub()
    {
        await RespondAsync(
            "Click the button below for the bot's github.",
            components: new ComponentBuilder()
                .WithButton("Click me for the bot's github",
                    style: ButtonStyle.Link,
                    url: "https://github.com/unitedsystemscooperative/usc-discord-bot-net")
                .Build()
        );
    }

    [SlashCommand("website", "List the website's github")]
    public async Task ListWebsiteGithub()
    {
        await RespondAsync(
            "Click the button below for the website's github.",
            components: new ComponentBuilder()
                .WithButton("Click me for the website's github",
                    style: ButtonStyle.Link,
                    url: "https://github.com/unitedsystemscooperative/usc-website")
                .Build()
        );
    }
}