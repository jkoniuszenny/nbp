using Mongo2Go;
using MongoDB.Bson;
using MongoDB.Driver;
using Shared.Settings;

namespace Infrastructure.Database;

public class DatabaseMongoContext
{
    private readonly IMongoDatabase _database;
    public readonly IClientSessionHandle ClientSessionHandle;

    public DatabaseMongoContext(
        DatabaseMongoSettings settings
        )
    {
        var runner = MongoDbRunner.Start(singleNodeReplSet:true);      

        var client = new MongoClient(runner.ConnectionString);
        _database = client.GetDatabase(settings.DatabaseName);

        var sessionOptions = new ClientSessionOptions
        {
            DefaultTransactionOptions = new TransactionOptions(
            readConcern: ReadConcern.Majority,
            writeConcern: WriteConcern.WMajority,
            readPreference: ReadPreference.Primary)
        };

        ClientSessionHandle = _database.Client.StartSession(sessionOptions);

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
