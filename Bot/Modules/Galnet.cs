using System.Net.Http.Json;
using System.Text.RegularExpressions;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using UnitedSystemsCooperative.Bot.Interfaces;
using UnitedSystemsCooperative.Bot.Models;
using UnitedSystemsCooperative.Bot.Utils;

namespace UnitedSystemsCooperative.Bot.Modules;

public class GalnetModule
{
    private readonly HttpClient _http;
    private readonly IDatabaseService _db;
    private readonly string _galnetApi;
    private readonly string _galnetChannelKey;
    private readonly string _galnetTitleKey;
    private readonly DiscordSocketClient _client;
    private CancellationTokenSource? _cancellationTokenSource;

    public GalnetModule(IDatabaseService db, IConfiguration config, IOptions<ServerValues> serverValuesOptions,
        HttpClient http, DiscordSocketClient client)
    {
        _db = db;
        _galnetApi = config.GetValue<string>("galnetApi");
        var serverValues = serverValuesOptions.Value;
        _galnetChannelKey = serverValues.GalNetChannelKey;
        _galnetTitleKey = serverValues.GalNetTitlesKey;
        _http = http;
        _client = client;
    }

    public async Task InitializeAsync()
    {
        try
        {
            _cancellationTokenSource = new CancellationTokenSource();
            await UtilityMethods.PeriodicAsync(PollGalnet, TimeSpan.FromMinutes(10), _cancellationTokenSource.Token);
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Galnet Module disabled");
            _cancellationTokenSource = null;
        }
    }

    public void Stop()
    {
        Console.WriteLine("Galnet Module disable requested. May take up to 10 minutes to process.");
        _cancellationTokenSource?.Cancel();
    }

    private async Task PollGalnet()
    {
        Console.WriteLine("Galnet Started");
        var shouldWatchGalNet = await _db.GetValueAsync<bool>("watchGalnet");
        if (shouldWatchGalNet)
        {
            var newArticles = await GetNewArticles();
            if (!newArticles.Any())
            {
                Console.WriteLine("No further articles.");
            }
            else
            {
                var channelId = await _db.GetValueAsync<string>(_galnetChannelKey);
                if (string.IsNullOrEmpty(channelId)) throw new Exception("no channel");

                var channel = (SocketTextChannel) await _client.GetChannelAsync(ulong.Parse(channelId));

                foreach (var article in newArticles)
                {
                    var embed = new EmbedBuilder()
                        .WithTitle(article.Title)
                        .WithDescription(article.Content)
                        .WithFooter(article.Date)
                        .Build();
                    await channel.SendMessageAsync(embed: embed);
                    Console.WriteLine($"Posted Galnet article: '{article.Title}'");
                }
            }
        }
    }

    private async Task<List<GalnetArticle>> GetNewArticles()
    {
        var response = await _http.GetFromJsonAsync<IEnumerable<GalnetArticle>>(_galnetApi);

        if (response == null) return new List<GalnetArticle>();

        Regex unicodeBreak = new(@"<br \/>");
        Regex newline = new(@"\n");
        Regex lastNewLine = new(@"\n$");

        var processedData = response.Select(article =>
        {
            var updatedContent = article.Content
                .Replace("<p>", "")
                .Replace("</p>", "");

            updatedContent = newline.Replace(updatedContent, "\n> ");
            updatedContent = lastNewLine.Replace(updatedContent, "");
            updatedContent = unicodeBreak.Replace(updatedContent, "\n");

            article.Content = updatedContent;
            return article;
        });

        var titles = await _db.GetValueAsync<List<string>>(_galnetTitleKey);
        var newArticles = processedData.Where(x => string.IsNullOrEmpty(titles.FirstOrDefault(y => y == x.Title)))
            .ToList();
        var newTitles = newArticles.Select(x => x.Title).ToList();
        titles.AddRange(newTitles);
        await _db.SetValueAsync(_galnetTitleKey, titles);

        return newArticles;
    }
}