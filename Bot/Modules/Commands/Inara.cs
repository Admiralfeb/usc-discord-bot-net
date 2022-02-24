using System.Net.Http.Json;
using System.Text;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Options;
using UnitedSystemsCooperative.Bot.Models;

namespace UnitedSystemsCooperative.Bot.Modules.Commands;

[Group("inara", "Inara Commands")]
public class InaraCommandModule : InteractionModuleBase<SocketInteractionContext>
{
    private readonly HttpClient _httpClient;
    private readonly InaraConfig _config;
    private readonly IEnumerable<Rank> _ranks;
    public InaraCommandModule(HttpClient httpClient, IOptions<InaraConfig> config, IOptions<List<Rank>> ranks) : base()
    {
        _httpClient = httpClient;
        _config = config.Value;
        _ranks = ranks.Value;
    }
    [SlashCommand("cmdr", "Get cmdr data")]
    public async Task GetCmdrData(string cmdrName)
    {
        await DeferAsync();
        IGuildUser user = (SocketGuildUser)Context.User;
        Action<MessageProperties> response = await GetInaraCmdr(cmdrName, user);
        await ModifyOriginalResponseAsync(response);
    }

    private async Task<Action<MessageProperties>> GetInaraCmdr(string cmdrName, IGuildUser user)
    {
        InaraRequestHeader requestHeader = new()
        {
            AppName = "USC Bot",
            AppVersion = "2.0.0",
            IsBeingDeveloped = true,
            ApiKey = _config.Token
        };
        InaraRequestEvent requestEvent = new()
        {
            EventName = "getCommanderProfile",
            EventData = new InaraRequestEventData() { SearchName = cmdrName }
        };

        InaraRequest inaraRequest = new() { Header = requestHeader, Events = new InaraRequestEvent[] { requestEvent } };
        HttpRequestMessage request = new(HttpMethod.Get, _config.ApiUrl);
        request.Content = new StringContent(inaraRequest.ToString(), Encoding.UTF8, "application/json");
        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        var inaraResponse = await response.Content.ReadFromJsonAsync<InaraResponse>();

        if (inaraResponse == null)
            return res => { res.Content = "There was an error"; };

        var eventResponse = inaraResponse.Events.First();
        if (eventResponse.EventStatus == (int)InaraResponseCodes.NotFound)
        {
            Embed embed = new EmbedBuilder()
            .WithTitle("No Profiles Found")
            .WithDescription("No inara profiles were found.")
            .Build();

            return res => { res.Embed = embed; };
        }
        if (eventResponse.EventStatus == (int)InaraResponseCodes.Error)
        {
            Embed embed = new EmbedBuilder()
            .WithTitle("Error")
            .WithDescription("There was an error executing that command.")
            .Build();

            return res => { res.Embed = embed; };
        }

        InaraCmdr cmdr = eventResponse.EventData;

        var embedBuilder = new EmbedBuilder()
            .WithTitle("Inara Profile")
            .WithUrl(cmdr.InaraUrl)
            .WithDescription(cmdr.PreferredGameRole ?? "N/A")
            .WithAuthor(new EmbedAuthorBuilder()
                .WithName(cmdr.CommanderName ?? cmdr.UserName)
                .WithIconUrl(cmdr.AvatarImageUrl)
                .WithUrl(cmdr.InaraUrl))
            .WithFooter($"Retrieved from Inara at the behest of {user}");

        ComponentBuilder componentBuilder = new();
        componentBuilder.WithButton("Inara Profile", style: ButtonStyle.Link, url: cmdr.InaraUrl);

        if (cmdr.AvatarImageUrl != null)
            embedBuilder.WithThumbnailUrl(cmdr.AvatarImageUrl);

        foreach (var rank in cmdr.CommanderRanksPilot)
        {
            string rankName = rank.RankName;
            int rankValue = rank.RankValue;
            var rankSet = _ranks.First(x => x.Name == rankName || x.InaraName == rankName);
            var currentRank = rankSet.Ranks.ElementAt(rankValue);
            var rankProgress =
                currentRank == "Elite V" ||
                currentRank == "King" ||
                currentRank == "Admiral" ?
                    "" :
                    $"- {Math.Round(rank.RankProgress * 100, 2)}%";

            embedBuilder.AddField(rankSet.Name.ToUpper(), $"{currentRank} {rankProgress}", true);
        }

        if (cmdr.CommanderSquadron != null)
        {
            embedBuilder.AddField("Squadron", cmdr.CommanderSquadron.SquadronName);
            componentBuilder.WithButton(
                cmdr.CommanderSquadron.SquadronName,
                style: ButtonStyle.Link,
                url: cmdr.CommanderSquadron.InaraUrl
            );
        }

        if (cmdr.OtherNamesFound != null)
        {
            var othersFoundEmbed = new EmbedBuilder().WithTitle("Other CMDRs found for that name.");
            foreach (var otherName in cmdr.OtherNamesFound)
                othersFoundEmbed.AddField("-", otherName, true);
            return res =>
            {
                res.Embeds = new Embed[] { othersFoundEmbed.Build(), embedBuilder.Build() };
                res.Components = componentBuilder.Build();
            };
        }

        return res => { res.Embed = embedBuilder.Build(); res.Components = componentBuilder.Build(); };
    }

    [SlashCommand("squadron", "Get the inara squad link")]
    public async Task GetInaraSquadLink()
    {
        await RespondAsync("Inara Squadron",
            components: new ComponentBuilder()
                .WithButton("United Systems Cooperative",
                    style: ButtonStyle.Link,
                    url: "https://inara.cz/squadron/7028/"
                )
                .Build());
    }
}
