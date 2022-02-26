using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;

namespace UnitedSystemsCooperative.Bot;

public class BotEventHandler
{
    private readonly BotSocketClient _client;
    private readonly InteractionService _commands;
    private readonly IServiceProvider _services;
    private readonly IConfiguration _configuration;


    public BotEventHandler(
        BotSocketClient client,
        InteractionService commands,
        IServiceProvider services,
        IConfiguration configuration)
    {
        _client = client;
        _commands = commands;
        _services = services;
        _configuration = configuration;
    }

    public void Initialize()
    {
        _client.Ready += OnReady;
        _client.UserJoined += OnGuildMemberAdd;
        _client.UserLeft += OnGuildMemberRemove;
    }

    public async Task OnReady()
    {
        if (IsDebug())
        {
            await _commands.RegisterCommandsToGuildAsync(_configuration.GetValue<ulong>("testGuild"), true);
        }
        else
        {
            await _commands.RegisterCommandsGloballyAsync(true);
        }
    }

    public async Task OnGuildMemberAdd(SocketUser user)
    {

    }

    public async Task OnGuildMemberRemove(SocketGuild guild, SocketUser user)
    {
        SocketGuildUser guildUser = (SocketGuildUser)user;
        IReadOnlyCollection<SocketRole> roles = guildUser.Roles;

    }

    static bool IsDebug()
    {
#if DEBUG
        return true;
#else
        return false;
#endif
    }
}
