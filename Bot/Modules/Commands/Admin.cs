using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using UnitedSystemsCooperative.Bot.Models;

namespace UnitedSystemsCooperative.Bot.Modules.Commands;

[RequireRole("High Command")]
[Group("admin", "Bot's admin functions")]
public class AdminCommandModule : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("auth_ping", "Pings if you have admin")]
    public async Task AdminPing()
    {
        await RespondAsync("You have admin!", ephemeral: true);
    }

    [RequireContext(ContextType.Guild)]
    [Group("setup_member", "Setup a new member")]
    public class SetupUserSubCommands : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("finalize", "Finalize user after bot has started process")]
        public async Task FinalizeUser(SocketGuildUser user)
        {
            if (user.IsBot)
            {
                await RespondAsync("Bots cannot be finalized.", ephemeral: true);
                return;
            }

            await DeferAsync(true);

            try
            {
                var requestOptions = new RequestOptions {AuditLogReason = "Finalize Member"};

                var roles = user.Guild.Roles;
                var isCadet = user.Roles.Any(x => x.Name.ToUpper() == "CADET");
                if (isCadet)
                {
                    var fleetMemberRole = roles.FirstOrDefault(role => role.Name.ToUpper() == "FLEET MEMBER");
                    await user.AddRoleAsync(fleetMemberRole, requestOptions);
                }

                var disassociateRole = roles.FirstOrDefault(role => role.Name.ToUpper() == "DISSOCIATE MEMBER");
                var newRole = roles.FirstOrDefault(role => role.Name.ToUpper() == "NEW MEMBER");

                if (disassociateRole != null)
                    await user.RemoveRoleAsync(disassociateRole, requestOptions);
                if (newRole != null)
                    await user.RemoveRoleAsync(newRole, requestOptions);

                await ModifyOriginalResponseAsync(x => x.Content = $"{user.DisplayName}'s setup has been finalized.");
            }
            catch (Exception)
            {
                await ModifyOriginalResponseAsync(x => x.Content = "There was an issue finalizing the member.");
            }
        }

        [SlashCommand("manual", "Setup a user manually")]
        public async Task SetupUserManually(SocketGuildUser user,
            [Summary(description: "nickname - WITHOUT 'CMDR'")]
            string cmdrName, CmdrType type, PlatformType platform)
        {
            if (user.IsBot)
            {
                await RespondAsync("Bots cannot be setup in this fashion.", ephemeral: true);
                return;
            }

            await DeferAsync(true);

            // Set Nickname
            await user.ModifyAsync(x => x.Nickname = $"CMDR {cmdrName}");

            var roles = user.Guild.Roles;
            if (roles == null)
            {
                await ModifyOriginalResponseAsync(x => x.Content = $"Roles not found");
                return;
            }

            // Set Roles (member and platform)
            var dissociateRole = roles.FirstOrDefault(x => x.Name.ToUpper() == "DISSOCIATE MEMBER");
            try
            {
                switch (type)
                {
                    case CmdrType.Member:
                        await SetRole(user, roles, "Fleet Member");
                        await SetRole(user, roles, "Cadet");
                        break;
                    case CmdrType.Ambassador:
                        await SetRole(user, roles, "Ambassador");
                        break;
                    case CmdrType.Guest:
                        await SetRole(user, roles, "Guest");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(platform), platform, null);
                }

                if (dissociateRole != null)
                    await user.RemoveRoleAsync(dissociateRole);

                switch (platform)
                {
                    case PlatformType.PC:
                        await SetRole(user, roles, "PC");
                        break;
                    case PlatformType.Xbox:
                        await SetRole(user, roles, "Xbox");
                        break;
                    case PlatformType.PlayStation:
                        await SetRole(user, roles, "Playstation");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(platform), platform, null);
                }
            }
            catch (Exception e)
            {
                await ModifyOriginalResponseAsync(x => x.Content = $"Error: {e.Message}");
                return;
            }

            await ModifyOriginalResponseAsync(x => x.Content = "User setup complete.");
        }

        private static async Task SetRole(SocketGuildUser user, IReadOnlyCollection<SocketRole> roles, string roleName)
        {
            var role = roles.FirstOrDefault(x =>
                string.Equals(x.Name, roleName, StringComparison.CurrentCultureIgnoreCase));
            if (role == null)
                throw new Exception("Role not found");

            await user.AddRoleAsync(role);
        }

        private static async Task RemoveRole(SocketGuildUser user, IReadOnlyCollection<SocketRole> roles,
            string roleName)
        {
            var role = roles.FirstOrDefault(x =>
                string.Equals(x.Name, roleName, StringComparison.CurrentCultureIgnoreCase));
            if (role == null)
                throw new Exception("Role not found");

            await user.RemoveRoleAsync(role);
        }

        [SlashCommand("move_to_member", "Move a ambassador/guest to member")]
        public async Task MoveToMember(SocketGuildUser user)
        {
            if (user.IsBot)
            {
                await RespondAsync("Bots cannot be setup in this fashion.", ephemeral: true);
                return;
            }

            await DeferAsync(true);

            var roles = user.Guild.Roles;
            if (roles == null)
            {
                await ModifyOriginalResponseAsync(x => x.Content = $"Roles not found");
                return;
            }

            await RemoveRole(user, roles, "Ambassador");
            await RemoveRole(user, roles, "Guest");

            await SetRole(user, roles, "Fleet Member");
            await SetRole(user, roles, "Cadet");

            await ModifyOriginalResponseAsync(x =>
                x.Content = "User has been moved to Cadet and Fleet Member. Please update Cmdr Dashboard");
        }
    }

    [Group("gankers", "Controls the ganker listing.")]
    public class GankerListSubCommands : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("add", "Add a ganker to the list")]
        public async Task AddGanker(string cmdrName)
        {
            await RespondAsync("AddGanker is not yet implemented.");
        }

        [SlashCommand("delete", "Remove a ganker from the list")]
        public async Task DeleteGanker(string cmdrName)
        {
            await RespondAsync("DeleteGanker is not yet implemented.");
        }

        [SlashCommand("update", "Updates the ganker list from the database")]
        public async Task UpdateGankerList()
        {
            await RespondAsync("UpdateGankerList is not yet implemented.");
        }
    }
}

public enum CmdrType
{
    Member,
    Ambassador,
    Guest
}