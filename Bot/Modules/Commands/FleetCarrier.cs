using Discord.Interactions;

namespace UnitedSystemsCooperative.Bot.Modules.Commands;

[RequireContext(ContextType.Guild)]
[RequireOwner]
[Group("fc", "Fleet Carrier Commands")]
public class FleetCarrierCommandModule : InteractionModuleBase<SocketInteractionContext>
{
    [Group("taxi", "Request or Resolve a FC Taxi Request")]
    public class FCTaxiSubCommands : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("request", "Request a ride from a FC. Must not be a permit locked system")]
        public async Task RequestTaxi(string sourceSystem, string destinationSystem)
        {
        }

        [RequireRole("FC Owner")]
        [ComponentInteraction("taxi-fulfill")]
        public async Task FulfillTaxi()
        {
        }
    }

    [RequireRole("Fleet Member")]
    [SlashCommand("register", "Registers a fleet carrier under your name.")]
    public async Task RegisterCarrier(string carrierName, string carrierId, string? currentSystem = null)
    {
    }

    [RequireRole("FC Owner")]
    [RequireRole("Fleet Member")]
    [Group("owner", "Owner Commands")]
    public class FCOwnerSubCommands : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("update-location",
            "Updates present system of the fleet carrier. If you have multiple carriers, please give carrierId")]
        public async Task UpdateSystem(string system, string? carrierId = null)
        {
        }

        [SlashCommand("update-name", "Updates carrier name. If you have multiple carriers, please give carrierId")]
        public async Task UpdateName(string newName, string? carrierId = null)
        {
        }

        [SlashCommand("delete-carrier",
            "Deletes the carrier from the database. If you have multiple carriers, please give carrierId")]
        public async Task RemoveCarrier(string? carrierId = null)
        {
        }
    }
}