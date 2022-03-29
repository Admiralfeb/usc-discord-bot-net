using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using UnitedSystemsCooperative.Bot.Interfaces;
using UnitedSystemsCooperative.Bot.Models;
using UnitedSystemsCooperative.Bot.Models.Exceptions;
using UnitedSystemsCooperative.Bot.Utils;

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
                        await UtilityMethods.SetRole(user, roles, "Fleet Member");
                        await UtilityMethods.SetRole(user, roles, "Cadet");
                        break;
                    case CmdrType.Ambassador:
                        await UtilityMethods.SetRole(user, roles, "Ambassador");
                        break;
                    case CmdrType.Guest:
                        await UtilityMethods.SetRole(user, roles, "Guest");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(platform), platform, null);
                }

                if (dissociateRole != null)
                    await user.RemoveRoleAsync(dissociateRole);

                switch (platform)
                {
                    case PlatformType.PC:
                        await UtilityMethods.SetRole(user, roles, "PC");
                        break;
                    case PlatformType.Xbox:
                        await UtilityMethods.SetRole(user, roles, "Xbox");
                        break;
                    case PlatformType.PlayStation:
                        await UtilityMethods.SetRole(user, roles, "Playstation");
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

            await UtilityMethods.RemoveRole(user, roles, "Ambassador");
            await UtilityMethods.RemoveRole(user, roles, "Guest");

            await UtilityMethods.SetRole(user, roles, "Fleet Member");
            await UtilityMethods.SetRole(user, roles, "Cadet");

            await ModifyOriginalResponseAsync(x =>
                x.Content = "User has been moved to Cadet and Fleet Member. Please update Cmdr Dashboard");
        }
    }

    [RequireContext(ContextType.Guild)]
    [Group("gankers", "Controls the ganker listing.")]
    public class GankerListSubCommands : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IDatabaseService _db;

        public GankerListSubCommands(IDatabaseService db)
        {
            _db = db;
        }

        [SlashCommand("add", "Add a ganker to the list")]
        public async Task AddGanker(string cmdrName)
        {
            await DeferAsync(true);
            var gankerList = await GetGankerList();

            if (gankerList.Any(x => x.Equals(cmdrName, StringComparison.OrdinalIgnoreCase)))
            {
                await ModifyOriginalResponseAsync(x => x.Content = "Ganker is already in list");
                return;
            }

            gankerList.Add(cmdrName);
            await SetGankerList(gankerList);

            try
            {
                await UpdateGankerMessage(gankerList);
            }
            catch (ChannelNotFoundException)
            {
                await ModifyOriginalResponseAsync(x => x.Content = "Channel could not be found.");
                return;
            }
            catch (MessageNotFoundException)
            {
                await ModifyOriginalResponseAsync(x => x.Content = "Message could not be found.");
                return;
            }
            catch (Exception)
            {
                await ModifyOriginalResponseAsync(x =>
                    x.Content = "An error occurred when trying to update the Ganker message");
                return;
            }

            await ModifyOriginalResponseAsync(x => x.Content = "Successfully updated ganker list");
        }

        [SlashCommand("delete", "Remove a ganker from the list")]
        public async Task DeleteGanker(string cmdrName)
        {
            await DeferAsync(true);
            var gankerList = await GetGankerList();

            var ganker = gankerList.FirstOrDefault(x => x.Equals(cmdrName, StringComparison.OrdinalIgnoreCase));

            if (string.IsNullOrEmpty(ganker))
            {
                await ModifyOriginalResponseAsync(x => x.Content = "Ganker is not present in list.");
                return;
            }

            gankerList.Remove(ganker);
            await SetGankerList(gankerList);

            try
            {
                await UpdateGankerMessage(gankerList);
            }
            catch (ChannelNotFoundException)
            {
                await ModifyOriginalResponseAsync(x => x.Content = "Channel could not be found.");
                return;
            }
            catch (MessageNotFoundException)
            {
                await ModifyOriginalResponseAsync(x => x.Content = "Message could not be found.");
                return;
            }
            catch (Exception)
            {
                await ModifyOriginalResponseAsync(x =>
                    x.Content = "An error occurred when trying to update the Ganker message");
                return;
            }

            await ModifyOriginalResponseAsync(x => x.Content = "Successfully updated ganker list");
        }

        [SlashCommand("update", "Updates the ganker list from the database")]
        public async Task UpdateGankerList()
        {
            await DeferAsync(true);
            var gankerList = await GetGankerList();

            try
            {
                await UpdateGankerMessage(gankerList);
            }
            catch (ChannelNotFoundException)
            {
                await ModifyOriginalResponseAsync(x => x.Content = "Channel could not be found.");
                return;
            }
            catch (MessageNotFoundException)
            {
                await ModifyOriginalResponseAsync(x => x.Content = "Message could not be found.");
                return;
            }
            catch (Exception)
            {
                await ModifyOriginalResponseAsync(x =>
                    x.Content = "An error occurred when trying to update the Ganker message");
                return;
            }

            await ModifyOriginalResponseAsync(x => x.Content = "Successfully updated ganker list");
        }

        private async Task<List<string>> GetGankerList()
        {
            return (await _db.GetValueAsync<DatabaseItemArray<string>>("gankers")).Value;
        }

        private async Task SetGankerList(IEnumerable<string> gankers)
        {
            await _db.SetValueAsync("gankers", new DatabaseItemArray() {Key = "gankers", Value = gankers.ToList()});
        }

        private async Task UpdateGankerMessage(IEnumerable<string> gankerList)
        {
            var embed = new EmbedBuilder()
                .WithTitle("Known Gankers")
                .WithDescription(string.Join("\n", gankerList.Reverse()))
                .Build();

            var channelId = (await _db.GetValueAsync<DatabaseItem>("gank_report_channel")).Value;
            var messageId = (await _db.GetValueAsync<DatabaseItem>("gank_report_message")).Value;
            if (Context.Guild.GetChannel(ulong.Parse(channelId)) is not SocketTextChannel channel)
                throw new ChannelNotFoundException();

            if (await channel.GetMessageAsync(ulong.Parse(messageId)) is not SocketUserMessage message)
                throw new MessageNotFoundException();

            await message.ModifyAsync(x => x.Embeds = new[] {embed});
        }
    }
}

public enum CmdrType
{
    Member,
    Ambassador,
    Guest
}