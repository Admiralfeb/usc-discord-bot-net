using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

#pragma warning disable CS8618
namespace UnitedSystemsCooperative.Bot.Models;

public class DatabaseItem<T>
{
    public ObjectId _id { get; set; }
    [BsonElement("key")] public string Key { get; set; }
    [BsonElement("value")] public T Value { get; set; }
}