using System.Text;
using Discord;
using Discord.WebSocket;
using UnitedSystemsCooperative.Bot.Models;

namespace UnitedSystemsCooperative.Bot.Utils;

public static class UtilityMethods
{
    public static async Task PeriodicAsync(Func<Task> action, TimeSpan interval,
        CancellationToken cancellationToken = default)
    {
        using var timer = new PeriodicTimer(interval);
        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await action();
            await timer.WaitForNextTickAsync(cancellationToken);
        }
    }

    public static async Task AutoSetupMember(SocketGuildUser user, JoinRequest joinRequest,
        SocketTextChannel? joinChannel)
    {
        var roles = user.Guild.Roles;

        // Set Nickname
        await user.ModifyAsync(x => x.Nickname = joinRequest.Cmdr);

        // Set Member Type
        var memberRole = joinRequest.Type switch
        {
            "join" => "cadet",
            "guest" => "Guest",
            "ambassador" => "Ambassador",
            _ => throw new Exception("No join type value.")
        };
        await SetRole(user, roles, memberRole);

        // Set Platform
        var platformRole = joinRequest.Platform switch
        {
            PlatformType.PC => "PC",
            PlatformType.Xbox => "Xbox",
            PlatformType.PlayStation => "PS",
            _ => string.Empty
        };
        await SetRole(user, roles, platformRole);

        await RemoveRole(user, roles, "New Member");

        if (joinChannel != null)
        {
            var message = new EmbedBuilder()
                .WithTitle("Automated Setup Complete")
                .WithDescription(new StringBuilder()
                    .AppendLine($"Automated setup complete for {user}")
                    .AppendLine()
                    .AppendLine(
                        "Final manual verification needed. `/admin setup_member finalize` will remove dissociate and add Fleet Member if applicable.")
                    .ToString())
                .Build();
            await joinChannel.SendMessageAsync(embed: message);
        }
    }

    public static async Task SetRole(SocketGuildUser user, IReadOnlyCollection<SocketRole> roles, string roleName)
    {
        var role = roles.FirstOrDefault(x =>
            string.Equals(x.Name, roleName, StringComparison.CurrentCultureIgnoreCase));
        if (role == null)
            throw new Exception("Role not found");

        await user.AddRoleAsync(role);
    }

    public static async Task RemoveRole(SocketGuildUser user, IReadOnlyCollection<SocketRole> roles,
        string roleName)
    {
        var role = roles.FirstOrDefault(x =>
            string.Equals(x.Name, roleName, StringComparison.CurrentCultureIgnoreCase));
        if (role == null)
            throw new Exception("Role not found");

        await user.RemoveRoleAsync(role);
    }
}