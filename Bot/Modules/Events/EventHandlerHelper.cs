using System.Text;
using Discord;
using Discord.WebSocket;

namespace UnitedSystemsCooperative.Bot.Modules.Events;

// Helpers to keep the other EventHandler partial clear.
public partial class BotEventHandler
{
    private static async Task RequestJoinRequest(SocketGuildUser user, SocketTextChannel? joinChannel)
    {
        var welcomeMessage = new EmbedBuilder()
            .WithTitle("<:phoenixlogoplain:833150738413781062> Welcome to United Systems Cooperative")
            .WithDescription(new StringBuilder()
                .AppendLine(
                    "You have not yet put in a request on unitedsystems.org. As such, the USC Bot cannot perform automated setup.")
                .AppendLine()
                .AppendLine("Please click the button below to fill out the USC form.")
                .AppendLine()
                .AppendLine(
                    "If you have already filled out the form or need assistance, please contact someone in High Command")
                .ToString())
            .Build();

        var componentRow = new ComponentBuilder()
            .WithButton("Fill out the Join Form",
                style: ButtonStyle.Link,
                url: "https://unitedsystems.org/join")
            .Build();

        await user.SendMessageAsync(embed: welcomeMessage, components: componentRow);
        if (joinChannel != null)
        {
            var embed = new EmbedBuilder()
                .WithTitle("Awaiting Join Request")
                .WithDescription($"Awaiting join request from ${user} before performing auto-setup")
                .Build();
            await joinChannel.SendMessageAsync(embed: embed);
        }
    }
}