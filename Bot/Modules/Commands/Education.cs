using System.Text;
using Discord;
using Discord.Interactions;
using Microsoft.Extensions.Options;
using UnitedSystemsCooperative.Bot.Models;

namespace UnitedSystemsCooperative.Bot.Modules.Commands;

[Group("edu", "Educational Information")]
public class EducationCommandModule : InteractionModuleBase<SocketInteractionContext>
{
    private const string INFORMATION_SENT = "Information sent to user.";
    private readonly List<Rank> _ranks;

    public EducationCommandModule(IOptions<List<Rank>> ranks) : base()
    {
        _ranks = ranks.Value;
    }

    [SlashCommand("combat-logging", "What is combat logging?")]
    public async Task EducateOnCombatLogging(IUser? user = null)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine("Combat Logging is improperly exiting gameplay (via menu, alt+F4, pulling a plug, etc) while in combat.");
        stringBuilder.AppendLine("FDev is very strict on this and it can result in a ban.");
        stringBuilder.AppendLine("While FDev does not consider logging to menu as 'Combat Logging', the Elite: Dangerous community does.");
        stringBuilder.AppendLine("Combat Logging is against the rules of USC and can result in a kick or ban.");

        var embedBuilder = new EmbedBuilder()
            .WithTitle("What is Combat Logging (clogging)?")
            .WithDescription(stringBuilder.ToString());

        if (user != null)
        {
            await user.SendMessageAsync(embed: embedBuilder.Build());
            await RespondAsync(INFORMATION_SENT, ephemeral: true);

        }
        else
        {
            await RespondAsync(embed: embedBuilder.Build());
        }
    }

    [Group("engineering", "Engineering things")]
    public class EngineeringSubcommandModule : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("fox-guide", "Fox's Guide to unlock engineers")]
        public async Task EducateOnFoxGuide(IUser? user = null)
        {
            string message = "Find Fox's Guide to unlocking engineers: https://www.reddit.com/r/EliteDangerous/comments/merpky/foxs_comprehensive_guide_to_engineer_unlocking/";
            if (user != null)
            {
                await user.SendMessageAsync(message);
                await RespondAsync(INFORMATION_SENT, ephemeral: true);
            }
            else
                await RespondAsync(message);
        }

        [SlashCommand("inara", "Engineer list on Inara")]
        public async Task EducateOnInaraEngineers(IUser? user = null)
        {
            string message = "Find where and what each engineer does at: https://inara.cz/galaxy-engineers/";
            if (user != null)
            {
                await user.SendMessageAsync(message);
                await RespondAsync(INFORMATION_SENT, ephemeral: true);
            }
            else
                await RespondAsync(message);
        }
    }

    [SlashCommand("fsd-booster", "Link to Exegious' video on how to unlock the Guardian FSD Booster")]
    public async Task EducateOnFsdBoosterUnlock(IUser? user = null)
    {
        string message = "Here's how to unlock the guardian fsd booster: https://youtu.be/J9C9a00-rkQ";
        if (user != null)
        {
            await user.SendMessageAsync(message);
            await RespondAsync(INFORMATION_SENT, ephemeral: true);
        }
        else
            await RespondAsync(message);
    }

    [SlashCommand("neutron", "How to use Neutron Highway")]
    public async Task EducateOnSuperCharging(
        [Summary(description: "Choose what you'd like to know about the Neutron Highway")]
        [Choice("Image Tutorial", "img")]
        [Choice("Spansh's Website", "spansh")]
        string option,
        IUser? user = null)
    {
        string message;
        switch (option)
        {
            case "img":
                message = "https://i.imgur.com/gg6n5VM.jpg";
                break;
            case "spansh":
                message = "Plot neutron highway routes online here: https://www.spansh.co.uk/plotter";
                break;
            default:
                await RespondAsync($"The {option} does not exist", ephemeral: true);
                return;
        }

        if (user != null)
        {
            await user.SendMessageAsync(message);
            await RespondAsync(INFORMATION_SENT, ephemeral: true);
        }
        else
            await RespondAsync(message);
    }

    [SlashCommand("promotions", "How do I get promoted in the squad?")]
    public async Task EducateOnSquadPromotions(IUser? user = null)
    {
        var embed = new EmbedBuilder()
            .WithTitle("How do I get promoted?")
            .AddField("Ensign", "You must have spent at least one week in squad and you've joined the squad in-game and in Discord")
            .AddField("Lieutenant", "Ensign requirements + You've joined the Inara squad and have a good understanding of the game/engineers")
            .AddField("Lt. Commander", "Lieutenant + joined the mentorship mission on Inara and have been approved by High Command")
            .AddField("Captain +", "Selected from the Lt. Cmdrs and offered the role of High Command")
            .Build();

        if (user != null)
        {
            await user.SendMessageAsync(embed: embed);
            await RespondAsync(INFORMATION_SENT, ephemeral: true);
        }
        else
            await RespondAsync(embed: embed);

    }

    [SlashCommand("ranks", "What are the Pilot's Federation or Navy Ranks?")]
    public async Task EducateOnRanks(
        [Summary(description:"Rank to see")]
        [Choice("Combat - Ship Only", "combat")]
        [Choice("Trade - Ship Only", "trade")]
        [Choice("Exploration - Ship Only", "exploration")]
        [Choice("CQC - Ship Only", "cqc")]
        [Choice("Exobiologist - Foot Only", "exobiologist")]
        [Choice("Mercenary - Foot Only", "mercenary")]
        [Choice("Imperial Navy", "empire")]
        [Choice("Federal Navy", "federation")]
        string rankSet,
        IUser? user = null
    )
    {
        var rankList = _ranks.FirstOrDefault(x => x.Name == rankSet)?.Ranks ?? null;
        if (rankList == null)
        {
            await RespondAsync("That rank does not exist.", ephemeral: true);
            return;
        }

        var rankEmbed = new EmbedBuilder()
            .WithTitle("Ranks")
            .WithDescription("Listed from lowest to highest")
            .AddField(
                rankSet.ToUpper(),
                string.Join('\n', rankList)
            )
            .Build();

        if (user != null)
        {
            await user.SendMessageAsync(embed: rankEmbed);
            await RespondAsync(INFORMATION_SENT, ephemeral: true);
        }
        else
            await RespondAsync(embed: rankEmbed);

    }

    [SlashCommand("scoopable", "What stars can I scooop?")]
    public async Task EducateOnKgbFoam(IUser? user = null)
    {
        string message = "Learn to filter the galaxy map for scoopable stars at: https://confluence.fuelrats.com/pages/releaseview.action?pageId=1507609\n\nOther languages available at: https://confluence.fuelrats.com/display/public/FRKB/KGBFOAM";
        if (user != null)
        {
            await user.SendMessageAsync(message);
            await RespondAsync(INFORMATION_SENT, ephemeral: true);
        }
        else
            await RespondAsync(message);

    }

    [SlashCommand("websites", "Gives a list of 3rd party websites")]
    public async Task EducateOn3rdPartySites(IUser? user = null)
    {
        string message = "Find mostly anything related to trading, combat and player stats at: https://inara.cz\nFind accurate trading data at: https://eddb.io\nFind accurate exploration data at: https://edsm.net\nBuild ships virtually and test their stats at: https://coriolis.io and https://edsy.org\nFind mining hotspots and sell locations at: https://edtools.cc/miner\nNeutron highway and road to riches at: https://spansh.co.uk\nUseful material finding, fleet carrier calculators, and more at: https://cmdrs-toolbox.com/\nNeed fuel? https://fuelrats.com";
        if (user != null)
        {
            await user.SendMessageAsync(message);
            await RespondAsync(INFORMATION_SENT, ephemeral: true);
        }
        else
            await RespondAsync(message);

    }
}
