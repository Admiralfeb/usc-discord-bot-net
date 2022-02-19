
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Discord;
using Discord.Interactions;

using Discord.WebSocket;


namespace UnitedSystemsCooperative.Bot;

class Program
{
    static void Main(string[] _)
    {
        IConfiguration config = new ConfigurationBuilder()
        .AddEnvironmentVariables(prefix: "DC_")
        .AddJsonFile("appsettings.local.json", optional: true)
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

        client.Ready += async () =>
        {
            if (IsDebug())
                await commands.RegisterCommandsToGuildAsync(configuration.GetValue<ulong>("testGuild"), true);
            else
                await commands.RegisterCommandsGloballyAsync(true);
        };

        await services.GetRequiredService<CommandHandler>().InitializeAsync();

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
    => new ServiceCollection()
    .AddSingleton(configuration)
    .AddSingleton<DiscordSocketClient>()
    .AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()))
    .AddSingleton<CommandHandler>()
    .BuildServiceProvider();

    static bool IsDebug()
    {
#if DEBUG
        return true;
#else
                return false;
#endif
    }
}
