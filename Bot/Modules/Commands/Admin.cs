using Discord;
using Discord.Interactions;
using UnitedSystemsCooperative.Bot.Models;

namespace UnitedSystemsCooperative.Bot.Modules.Commands;

[Group("admin", "Bot's admin functions")]
public class AdminCommandModule : InteractionModuleBase<SocketInteractionContext>
{
    [Group("auth_users", "Controls the authorized admin users.")]
    public class AuthUsersSubCommands : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("add", "Adds an admin user.")]
        public async Task AddAdminUser(IUser user)
        {
            await RespondAsync($"Cannot add {user.Username}. AddAdminUser is not yet implemented.");
        }

        [SlashCommand("delete", "Delete an admin user.")]
        public async Task DeleteAdminUser(IUser user)
        {
            await RespondAsync($"Cannot delete {user.Username}. DeleteAdminUser is not yet implemented.");
        }

        [SlashCommand("list", "List admin users")]
        public async Task ListAdminUsers()
        {
            await RespondAsync($"Cannot list admin users. ListAdminUsers is not implemented");
        }
    }

    [Group("setup_member", "Setup a new member")]
    public class SetupUserSubCommands : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("finalize", "Finalize user after bot has started process")]
        public async Task FinalizeUser(IUser user)
        {
            await RespondAsync("FinalizeUser not yet implemented");
        }

        [SlashCommand("manual", "Setup a user manually")]
        public async Task SetupUserManually(IUser user,
            [Summary(description: "nickname - WITHOUT 'CMDR'")] string cmdrName, CmdrType type, PlatformType platform)
        {
            await RespondAsync("SetupUserManually not yet implemented.");
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