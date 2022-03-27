using System.Text;
using Discord;
using Discord.Interactions;
using UnitedSystemsCooperative.Bot.Models;

namespace UnitedSystemsCooperative.Bot.Modules.Commands;

[RequireContext(ContextType.Guild)]
[Group("poll", "Handles Polls")]
public class Poll : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("create", "Create a poll")]
    public async Task CreatePoll(string question, string choice1, string choice2, string? choice3 = null,
        string? choice4 = null, string? choice5 = null, string? choice6 = null, string? choice7 = null,
        bool canAbstain = false)
    {
        var messageBuilder = new StringBuilder()
            .AppendLine($":one: {choice1}")
            .AppendLine($":two: {choice2}");
        if (!string.IsNullOrEmpty(choice3)) messageBuilder.AppendLine($":three: {choice3}");
        if (!string.IsNullOrEmpty(choice4)) messageBuilder.AppendLine($":four: {choice4}");
        if (!string.IsNullOrEmpty(choice5)) messageBuilder.AppendLine($":five: {choice5}");
        if (!string.IsNullOrEmpty(choice6)) messageBuilder.AppendLine($":six: {choice6}");
        if (!string.IsNullOrEmpty(choice7)) messageBuilder.AppendLine($":seven: {choice7}");
        if (canAbstain) messageBuilder.AppendLine(":x: Abstain");

        var embed = new EmbedBuilder()
            .WithTitle(question)
            .WithFooter($"Poll by {Context.User}")
            .WithCurrentTimestamp()
            .WithDescription(messageBuilder.ToString())
            .Build();

        await RespondAsync(embed: embed);
        var response = await GetOriginalResponseAsync();

        List<Emoji> emojis = new() {Emoji.Parse(":one:"), Emoji.Parse(":two:")};
        if (!string.IsNullOrEmpty(choice3)) emojis.Add(Emoji.Parse(":three:"));
        if (!string.IsNullOrEmpty(choice4)) emojis.Add(Emoji.Parse(":four:"));
        if (!string.IsNullOrEmpty(choice5)) emojis.Add(Emoji.Parse(":five:"));
        if (!string.IsNullOrEmpty(choice6)) emojis.Add(Emoji.Parse(":six:"));
        if (!string.IsNullOrEmpty(choice7)) emojis.Add(Emoji.Parse(":seven:"));
        if (canAbstain) emojis.Add(Emoji.Parse(":x:"));

        await response.AddReactionsAsync(emojis);
    }

    [SlashCommand("create-yes-no", "Create a simple yes/no poll")]
    public async Task CreateYesNoPoll(string question, bool canAbstain = false)
    {
        var messageBuilder = new StringBuilder()
            .AppendLine($":one: Yay")
            .AppendLine($":two: Nay");
        if (canAbstain) messageBuilder.AppendLine(":x: Abstain");

        var embed = new EmbedBuilder()
            .WithTitle(question)
            .WithFooter($"Poll by {Context.User}")
            .WithCurrentTimestamp()
            .WithDescription(messageBuilder.ToString())
            .Build();

        await RespondAsync(embed: embed);
        var response = await GetOriginalResponseAsync();

        List<Emoji> emojis = new() {Emoji.Parse(":one:"), Emoji.Parse(":two:")};
        if (canAbstain) emojis.Add(Emoji.Parse(":x:"));

        await response.AddReactionsAsync(emojis);
    }

    [SlashCommand("results", "Display Results of poll")]
    public async Task ShowResults(string messageLink)
    {
        var linkSections = messageLink.Trim('/').Split('/');
        if (linkSections.Length != 7)
        {
            await RespondAsync(
                "Link does not appear to be correct. Should be 'https://discord.com/channels/###/###/###' where ### is a long string of numbers.");
            return;
        }

        var channel = Context.Guild.GetTextChannel(ulong.Parse(linkSections[^2]));
        if (channel == null)
        {
            await RespondAsync("Channel cannot be found in the server.");
            return;
        }

        if (!ulong.TryParse(linkSections[^1], out var messageId))
        {
            await RespondAsync("MessageId is not a proper value.");
            return;
        }

        var message = await channel.GetMessageAsync(messageId);
        if (message == null)
        {
            await RespondAsync("Message cannot be found in the channel.");
            return;
        }

        var reactions = message.Reactions;

        var labels = new List<string>();
        var counts = new List<int>();
        foreach (var (key, value) in reactions)
        {
            labels.Add(key.Name);
            // -1 due to the bot voting at least once.
            counts.Add(value.ReactionCount - 1);
        }

        var chart = new QuickChart(labels, counts);

        var chartUrl = chart.GetChartUrl();
        await RespondAsync($"Poll results for {messageLink}\n{chartUrl}");
    }
}