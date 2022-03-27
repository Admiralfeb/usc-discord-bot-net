using System.Text;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using UnitedSystemsCooperative.Bot.Interfaces;
using UnitedSystemsCooperative.Bot.Utils;

namespace UnitedSystemsCooperative.Bot.Modules.Events;

public partial class BotEventHandler
{
    private readonly BotSocketClient _client;
    private readonly InteractionService _commands;
    private readonly IServiceProvider _services;
    private readonly IConfiguration _configuration;
    private readonly IDatabaseService _db;


    public BotEventHandler(
        BotSocketClient client,
        InteractionService commands,
        IServiceProvider services,
        IConfiguration configuration,
        IDatabaseService db)
    {
        _client = client;
        _commands = commands;
        _services = services;
        _configuration = configuration;
        _db = db;
    }

    public void Initialize()
    {
        _client.Ready += OnReady;
        _client.UserJoined += OnGuildMemberAdd;
        _client.UserLeft += OnGuildMemberRemove;
        _client.MessageReceived += OnMessageReceived;
    }

    public async Task OnReady()
    {
        if (IsDebug())
            await _commands.RegisterCommandsToGuildAsync(_configuration.GetValue<ulong>("testGuild"), true);
        else
            await _commands.RegisterCommandsGloballyAsync(true);
    }

    public async Task OnGuildMemberAdd(SocketGuildUser user)
    {
        var roles = user.Guild.Roles;
        await UtilityMethods.SetRole(user, roles, "Dissociate Member");
        await UtilityMethods.SetRole(user, roles, "New Member");

        var joinChannel = user.Guild.GetChannel(708038933132476537) as SocketTextChannel;
        var joinRequest = await _db.GetJoinRequest(user.ToString());
        if (joinRequest != null)
            await UtilityMethods.AutoSetupMember(user, joinRequest, joinChannel);
        else
            await RequestJoinRequest(user, joinChannel);
    }

    public async Task OnGuildMemberRemove(SocketGuild guild, SocketUser user)
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

        var joinChannel = guild.GetTextChannel(708038933132476537);
        if (joinChannel != null)
            await joinChannel.SendMessageAsync(embed: embed);
    }

    public async Task OnMessageReceived(SocketMessage message)
    {
        if (message is not SocketUserMessage)
            return;
        if (message.Channel.Id != 708038933132476537)
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