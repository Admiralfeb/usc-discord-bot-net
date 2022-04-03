using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using UnitedSystemsCooperative.Bot.Interfaces;
using UnitedSystemsCooperative.Bot.Models;

namespace UnitedSystemsCooperative.Bot.Services;

public class MongoDbService : IDatabaseService
{
    private readonly string _connString;

    public MongoDbService(IConfiguration config)
    {
        _connString = config.GetConnectionString("mongoDb");
    }

    public async Task<T> GetValueAsync<T>(string key)
    {
        var client = GetClient();
        var database = client.GetDatabase("usc");
        var collection = database.GetCollection<DatabaseItem<T>>("discordKeys");
        var doc = await collection.Find(Builders<DatabaseItem<T>>.Filter.Eq("key", key)).FirstOrDefaultAsync();

        return doc.Value;
    }

    public async Task SetValueAsync<T>(string key, T value)
    {
        var client = GetClient();
        var database = client.GetDatabase("usc");
        var collection = database.GetCollection<DatabaseItem<T>>("discordKeys");

        var newDoc = new DatabaseItem<T>() {Key = key, Value = value};

        await collection.FindOneAndUpdateAsync(
            Builders<DatabaseItem<T>>.Filter.Eq("key", key),
            Builders<DatabaseItem<T>>.Update.Set("value", value),
            new FindOneAndUpdateOptions<DatabaseItem<T>>() {IsUpsert = true}
        );
    }

    public async Task<JoinRequest?> GetJoinRequest(string discordUserName)
    {
        var client = GetClient();
        var database = client.GetDatabase("usc");
        var collection = database.GetCollection<JoinRequest>("joinRequests");

        var request = await collection
            .Find(Builders<JoinRequest>
                .Filter.Eq("discord", discordUserName))
            .FirstOrDefaultAsync();

        return request;
    }

    public Task SetEmail(string tag, string email)
    {
        throw new NotImplementedException();
    }

    private MongoClient GetClient()
    {
        var settings = MongoClientSettings.FromConnectionString(_connString);
        settings.ServerApi = new ServerApi(ServerApiVersion.V1);
        return new MongoClient(settings);
    }
}