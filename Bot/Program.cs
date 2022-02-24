
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UnitedSystemsCooperative.Bot.Models;
using UnitedSystemsCooperative.Bot.Modules.Commands;

namespace UnitedSystemsCooperative.Bot;

class Program
{
    static void Main(string[] _)
    {
        IConfiguration config = new ConfigurationBuilder()
        .AddEnvironmentVariables(prefix: "DC_")
        .AddJsonFile("appsettings.local.json", optional: true)
        .AddJsonFile("appsettings.json", optional: false)
        .Build();

        RunAsync(config).GetAwaiter().GetResult();
    }

    static async Task RunAsync(IConfiguration configuration)
    {
        using var services = ConfigureServices(configuration);

        var client = services.GetRequiredService<DiscordSocketClient>();
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

    static Task LogAsync(LogMessage msg)
    {
        Console.WriteLine(msg.ToString());
        return Task.CompletedTask;
    }

    static ServiceProvider ConfigureServices(IConfiguration configuration)
    {
        var services = new ServiceCollection();
        services.AddSingleton(configuration);
        services.AddSingleton<DiscordSocketClient>();
        services.AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()));
        services.AddSingleton<CommandHandler>();
        services.AddSingleton<BotEventHandler>();

        services.AddHttpClient<InaraCommandModule>(client =>
        {
            client.BaseAddress = new Uri("https://inara.cz/inapi/v1");
        });
        services.Configure<InaraConfig>(configuration.GetSection(InaraConfig.ConfigName));
        services.Configure<List<Rank>>(configuration.GetSection("ranks"));

        return services.BuildServiceProvider();
    }
}
