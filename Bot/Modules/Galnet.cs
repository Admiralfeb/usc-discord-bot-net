using System.Net.Http.Json;
using System.Text.RegularExpressions;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using UnitedSystemsCooperative.Bot.Interfaces;
using UnitedSystemsCooperative.Bot.Models;
using UnitedSystemsCooperative.Bot.Utils;

namespace UnitedSystemsCooperative.Bot.Modules;

public class GalnetModule
{
    private readonly HttpClient _http;
    private readonly IDatabaseService _db;
    private readonly string _galnetApi;
    private readonly DiscordSocketClient _client;
    private CancellationTokenSource? cancellationTokenSource;
    public GalnetModule(IDatabaseService db, IConfiguration config, HttpClient http, DiscordSocketClient client)
    {
        _db = db;
        _galnetApi = config.GetValue<string>("galnetApi");
        _http = http;
        _client = client;
    }

    public async Task InitializeAsync()
    {
        try
        {
            cancellationTokenSource = new CancellationTokenSource();
            await UtilityMethods.PeriodicAsync(PollGalnet, TimeSpan.FromMinutes(10), cancellationTokenSource.Token);
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Galnet Module disabled");
            cancellationTokenSource = null;
        }
    }

    public void Stop()
    {
        Console.WriteLine("Galnet Module disable requested. May take up to 10 minutes to process.");
        cancellationTokenSource?.Cancel();
    }

    private async Task PollGalnet()
    {
        var watchGalnet = await _db.GetValueAsync<DatabaseItem>("watchGalnet");
        if (watchGalnet.Value == "true")
        {
            var newArticles = await GetNewArticles();
            if (newArticles.Any())
            {
                var channelId = (await _db.GetValueAsync<DatabaseItem>("galnetChannel")).Value;
                if (string.IsNullOrEmpty(channelId))
                {
                    throw new Exception("no channel");
                }
                SocketNewsChannel channel = (SocketNewsChannel)await _client.GetChannelAsync(ulong.Parse(channelId));

                foreach (var article in newArticles)
                {
                    var embed = new EmbedBuilder()
                    .WithTitle(article.Title)
                    .WithDescription(article.Content)
                    .WithFooter(article.Date)
                    .Build();
                    await channel.SendMessageAsync(embed: embed);
                }
            }
            else
            {
                Console.WriteLine("No further articles.");
            }
        }

    }

    private async Task<IEnumerable<GalnetArticle>> GetNewArticles()
    {
        var response = await _http.GetFromJsonAsync<IEnumerable<GalnetArticle>>(_galnetApi);

        if (response == null)
        {
            return new List<GalnetArticle>();
        }

        Regex unicodeBreak = new(@"/<br \/>/g");
        Regex newline = new(@"/\n/g");
        Regex lastNewLine = new(@"/\n$/");

        var processedData = response.Select(article =>
        {
            var updatedContent = article.Content
            .Replace("<p>", "")
            .Replace("</p>", "");

            updatedContent = unicodeBreak.Replace(updatedContent, "");
            updatedContent = lastNewLine.Replace(updatedContent, "");
            updatedContent = newline.Replace(updatedContent, "\n> ");

            article.Content = updatedContent;
            return article;
        });

        List<string> titles = (await _db.GetValueAsync<DatabaseItemArray>("galnetTitles")).Value ?? new List<string>();
        var newArticles = processedData.Where(x => string.IsNullOrEmpty(titles.FirstOrDefault(y => y == x.Title)));
        var newtitles = new DatabaseItemArray()
        {
            Key = "galnetTitles",
            Value = newArticles.Select(x => x.Title).ToList()
        };
        await _db.SetValueAsync("galnetTitles", newtitles);

        return newArticles;
    }
}
