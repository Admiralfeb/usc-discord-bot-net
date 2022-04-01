using Discord;
using Discord.Interactions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using UnitedSystemsCooperative.Bot.Interfaces;
using UnitedSystemsCooperative.Bot.Models;
using UnitedSystemsCooperative.Bot.Modules;
using UnitedSystemsCooperative.Bot.Modules.Commands;
using UnitedSystemsCooperative.Bot.Modules.Events;
using UnitedSystemsCooperative.Bot.Services;

namespace UnitedSystemsCooperative.Bot;

internal static class Program
{
    private static void Main(string[] _)
    {
        IConfiguration config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.local.json", true)
#if DEV
            .AddJsonFile("appsettings.dev.json", false)
#else
            .AddJsonFile("appsettings.prod.json", false)
#endif
            .AddJsonFile("appsettings.json", false)
            .AddEnvironmentVariables()
            .Build();

        RunAsync(config).GetAwaiter().GetResult();
    }

    private static async Task RunAsync(IConfiguration configuration)
    {
        await using var services = ConfigureServices(configuration);

        var client = services.GetRequiredService<BotSocketClient>();
        var commands = services.GetRequiredService<InteractionService>();

        client.Log += LogAsync;
        commands.Log += LogAsync;

        await services.GetRequiredService<CommandHandler>().InitializeAsync();
        services.GetRequiredService<BotEventHandler>().Initialize();

        var botToken = configuration.GetValue<string>("BotToken");
        await client.LoginAsync(TokenType.Bot, botToken);
        await client.StartAsync();

        await Task.Delay(Timeout.Infinite);
    }

    private static Task LogAsync(LogMessage msg)
    {
        Console.WriteLine(msg.ToString());
        return Task.CompletedTask;
    }

    private static ServiceProvider ConfigureServices(IConfiguration configuration)
    {
        var services = new ServiceCollection();
        services.Configure<InaraConfig>(configuration.GetSection(InaraConfig.ConfigName));
        services.Configure<List<Rank>>(configuration.GetSection("ranks"));
        services.AddLogging();

        services.AddSingleton(configuration);
        services.AddSingleton<BotSocketClient>();
        services.AddSingleton(x => new InteractionService(x.GetRequiredService<BotSocketClient>()));
        services.AddSingleton<CommandHandler>();
        services.AddSingleton<EducationCommandModule>();
        services.AddSingleton<BotEventHandler>();
        services.AddSingleton<IDatabaseService, MongoDbService>();

        services.AddHttpClient<InaraCommandModule>(client => new HttpClient
            {BaseAddress = new Uri(configuration["InaraConfig:ApiUrl"])});
        services.AddHttpClient<GalnetModule>();
        services.AddHttpClient();

        return services.BuildServiceProvider();
    }
}