using System.Text;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using UnitedSystemsCooperative.Bot.Interfaces;
using UnitedSystemsCooperative.Bot.Models;
using UnitedSystemsCooperative.Bot.Utils;

namespace UnitedSystemsCooperative.Bot.Modules.Events;

public partial class BotEventHandler
{
    private readonly DiscordSocketClient _client;
    private readonly InteractionService _commands;
    private readonly IServiceProvider _services;
    private readonly IConfiguration _configuration;
    private readonly IDatabaseService _db;
    private readonly GalnetModule _galnet;
    private readonly ServerValues _serverValues;


    public BotEventHandler(
        DiscordSocketClient client,
        InteractionService commands,
        IServiceProvider services,
        IConfiguration configuration,
        IOptions<ServerValues> serverValueOptions,
        IDatabaseService db,
        GalnetModule galnet)
    {
        _client = client;
        _commands = commands;
        _services = services;
        _configuration = configuration;
        _serverValues = serverValueOptions.Value;
        _db = db;
        _galnet = galnet;
    }

    public void Initialize()
    {
        _client.Ready += OnReady;
        _client.UserJoined += OnUserJoined;
        _client.UserLeft += OnUserLeft;
        // _client.MessageReceived += OnMessageReceived;
    }

    public async Task OnReady()
    {
        if (IsDebug())
            await _commands.RegisterCommandsToGuildAsync(_configuration.GetValue<ulong>("testGuild"), true);
        else
            await _commands.RegisterCommandsGloballyAsync(true);

        await _galnet.InitializeAsync().ConfigureAwait(false);
    }

    public async Task OnUserJoined(SocketGuildUser user)
    {
        var roles = user.Guild.Roles;
        await UtilityMethods.SetRole(user, roles, "Dissociate Member");
        await UtilityMethods.SetRole(user, roles, "New Member");

        var joinChannel = user.Guild.GetChannel(_serverValues.JoinChannelId) as SocketTextChannel;
        var joinRequest = await _db.GetJoinRequest(user.ToString());
        if (joinRequest != null)
            await UtilityMethods.AutoSetupMember(user, joinRequest, joinChannel);
        else
            await RequestJoinRequest(user, joinChannel);
    }

    public async Task OnUserLeft(SocketGuild guild, SocketUser user)
    {
        var guildUser = (SocketGuildUser) user;
        var userRoles = guildUser.Roles.Aggregate(new StringBuilder(), (acc, val) => acc.Append($"{val} ")).ToString();

        var embed = new EmbedBuilder()
            .WithTitle("Member Left")
            .WithDescription($"{user}")
            .AddField("Roles", userRoles)
            .WithFooter(user.ToString())
            .WithCurrentTimestamp()
            .Build();

        var joinChannel = guild.GetTextChannel(_serverValues.JoinChannelId);
        if (joinChannel != null)
            await joinChannel.SendMessageAsync(embed: embed);
    }

    public async Task OnMessageReceived(SocketMessage message)
    {
        if (message is not SocketUserMessage)
            return;
        if (message.Channel.Id != _serverValues.JoinChannelId)
            return;
        if (message.Author.IsWebhook == false && (message.Author as SocketWebhookUser).Username != "Application System")
            return;

        var text = message.Embeds.First().Description ?? string.Empty;
        var userName = text.Remove(text.IndexOf("has")).Trim();
        if (!string.IsNullOrEmpty(userName))
        {
            var guild = (message.Channel as SocketTextChannel).Guild;
            if (guild == null)
                return;
            await guild.DownloadUsersAsync();
            var member =
                guild.Users.FirstOrDefault(x => x.ToString().Equals(userName, StringComparison.OrdinalIgnoreCase));
            if (member == null)
                return;

            var channel = message.Channel as SocketTextChannel;
            var joinRequest = await _db.GetJoinRequest(userName);
            if (joinRequest != null)
                await UtilityMethods.AutoSetupMember(member, joinRequest, channel);
        }
    }

    private static bool IsDebug()
    {
#if DEBUG
        return true;
#else
        return false;
#endif
    }
}