using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

#pragma warning disable CS8618

namespace UnitedSystemsCooperative.Bot.Models;

public class JoinRequest
{
    public ObjectId _id { get; set; }
    [BsonElement("type")] public string Type { get; set; }
    [BsonElement("cmdr")] public string Cmdr { get; set; }
    [BsonElement("discord")] public string DiscordName { get; set; }
    [BsonElement("platform")] public PlatformType Platform { get; set; }
}