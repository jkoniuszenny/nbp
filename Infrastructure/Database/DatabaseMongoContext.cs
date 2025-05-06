using Mongo2Go;
using MongoDB.Bson;
using MongoDB.Driver;
using Shared.Settings;

namespace Infrastructure.Database;

public class DatabaseMongoContext
{
    private readonly IMongoDatabase _database;

    public DatabaseMongoContext(
        DatabaseMongoSettings settings
        )
    {
        var runner = MongoDbRunner.Start();      

        var client = new MongoClient(runner.ConnectionString);
        _database = client.GetDatabase(settings.DatabaseName);
    }

    public async Task<IMongoCollection<T>> GetCollection<T>(string collectionName)
    {
        return await Task.FromResult(_database.GetCollection<T>(collectionName));
    }

    public async Task<bool> CheckHealthAsync()
    {
        try
        {
            await _database.RunCommandAsync((Command<BsonDocument>)"{ping:1}");
        }
        catch
        {
            return false;
        }

        return true;


    }
}
