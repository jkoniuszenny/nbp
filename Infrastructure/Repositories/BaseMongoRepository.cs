using Application.Interfaces.Providers;
using Application.Interfaces.Repositories;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;
using Shared.Settings;
using System.Linq.Expressions;

namespace Infrastructure.Repositories;

public class BaseMongoRepository: IAsyncRepository
{
    private const string _version = "Version";
    private const string _createTmsTmp = "CreateTmsTmp";
    private const string _createUser = "CreateUser";
    private const string _modifiedTmsTmp = "ModifiedTmsTmp";
    private const string _modifiedUser = "ModifiedUser";
    private const string _id = "Id";
    private const string _history = "History";

    protected readonly DatabaseMongoContext _databaseContext;
    protected readonly DatabaseMongoSettings _databaseSettings;
    private readonly IUserProvider _userProvider;

    public IClientSessionHandle ClientSessionHandle => _databaseContext.ClientSessionHandle;

    public BaseMongoRepository(
        DatabaseMongoContext databaseContext,
        DatabaseMongoSettings databaseSettings,
        IUserProvider userProvider)
    {
        _databaseContext = databaseContext;
        _databaseSettings = databaseSettings;
        _userProvider = userProvider;
    }



    public async Task DeleteList<T>(IReadOnlyList<T> entities) where T : class
    {
        var collection = await _databaseContext.GetCollection<T>(typeof(T).Name);

        foreach (var item in entities)
        {
            var value = item!.GetType().GetProperty(_id)!.GetValue(item);

            FilterDefinition<T> filter = Builders<T>.Filter.Eq(_id, value);

            await collection.DeleteOneAsync(filter);
        }
    }

    public async Task DeleteOne<T>(T entity) where T : class
    {
        var value = typeof(T).GetProperty(_id)!.GetValue(entity);

        var collection = await _databaseContext.GetCollection<T>(typeof(T).Name);
        FilterDefinition<T> filter = Builders<T>.Filter.Eq(_id, value);

        await collection.DeleteOneAsync(filter);
    }

    public async Task InsertList<T>(IReadOnlyList<T> entities) where T : class
    {
        foreach (var item in entities)
        {
            try
            {
                var createTmsTmp = item.GetType().GetProperty(_createTmsTmp)!;
                createTmsTmp.SetValue(item, DateTime.UtcNow, null);

                var createUser = item.GetType().GetProperty(_createUser)!;
                createUser.SetValue(item, _userProvider?.UserName, null);

                var version = item!.GetType().GetProperty(_version)!;
                version.SetValue(item, 1, null);
            }
            catch
            {
                // no catch
            }
        }

        var collection = await _databaseContext.GetCollection<T>(typeof(T).Name);
        await collection.InsertManyAsync(entities);
    }

    public async Task InsertOne<T>(T entity) where T : class
    {
        try
        {
            var createTmsTmp = entity.GetType().GetProperty(_createTmsTmp)!;
            createTmsTmp.SetValue(entity, DateTime.UtcNow, null);

            var createUser = entity.GetType().GetProperty(_createUser)!;
            createUser.SetValue(entity, _userProvider?.UserName, null);

            var version = entity!.GetType().GetProperty(_version)!;
            version.SetValue(entity, 1, null);
        }
        catch
        {
            // no catch
        }

        var collection = await _databaseContext.GetCollection<T>(typeof(T).Name);

        await collection.InsertOneAsync(entity);
    }

    public async Task<IReadOnlyList<T>> Select<T>(Expression<Func<T, bool>> filters) where T : class
    {
        var collection = await _databaseContext.GetCollection<T>(typeof(T).Name);

        FilterDefinition<T> filter = Builders<T>.Filter.Where(filters);

        return await collection.Find(filter).ToListAsync();
    }

    public async Task<IReadOnlyList<BsonDocument>> SelectParametrize<T>(FilterDefinition<T> filter, ProjectionDefinition<T> projection) where T : class
    {
        var collection = await _databaseContext.GetCollection<T>(typeof(T).Name);

        var result = await collection.Find(filter).Project(projection.Exclude("_id")).ToListAsync();

        return result;
    }

    public async Task<IQueryable<T>> AsQueryable<T>() where T : class
    {
        var collection = await _databaseContext.GetCollection<T>(typeof(T).Name);

        return collection.AsQueryable();
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Critical Code Smell", "S3776:Cognitive Complexity of methods should not be too high", Justification = "<Pending>")]
    public async Task<IReadOnlyList<(string ObjectId, bool Success, string Error)>> UpdateList<T>(IReadOnlyList<T> entities) where T : class
    {
        List<(string ObjectId, bool Success, string Error)> updateResult = [];

        var collection = await _databaseContext.GetCollection<T>(typeof(T).Name);

        List<string> existedPosition = [];

        foreach (var item in entities)
            existedPosition.Add(item!.GetType().GetProperty(_id)?.GetValue(item)?.ToString()!);

        FilterDefinition<T> filterSelect = Builders<T>.Filter.In(_id, existedPosition);
        var existedInMongo = await collection.Find(filterSelect).Project<T>(Builders<T>.Projection.Exclude(_history)).ToListAsync();

        var throtler = new SemaphoreSlim(100);

        Parallel.ForEach(entities, async item =>
        {
            await throtler.WaitAsync();

            try
            {
                long currentVersion = 0;

                try
                {
                    var modifiedTmsTmp = item!.GetType().GetProperty(_modifiedTmsTmp)!;
                    modifiedTmsTmp.SetValue(item, DateTime.UtcNow, null);

                    var modifiedUser = item!.GetType().GetProperty(_modifiedUser)!;
                    modifiedUser.SetValue(item, _userProvider?.UserName, null);

                    var version = item!.GetType().GetProperty(_version)!;
                    currentVersion = Convert.ToInt64(version.GetValue(item));
                }
                catch
                {
                    // no catch
                }

                var id = item!.GetType().GetProperty(_id)!.GetValue(item);

                FilterDefinition<T> filter = Builders<T>.Filter.Eq(_id, id)
                    & Builders<T>.Filter.Eq(_version, currentVersion);

                var update = Builders<T>.Update;
                var updates = new List<UpdateDefinition<T>>();

                foreach (var prop in item!.GetType().GetProperties())
                {
                    var value = item!.GetType().GetProperty(prop.Name)!.GetValue(item);

                    if (value is not null && !prop.CustomAttributes.Any(a => a.AttributeType.Name.Equals("IgnoreForMongoUpdateAttribute")))
                        updates.Add(update.Set(prop.Name, value));
                }

                updates.Add(update.Inc(_version, 1));
                updates.Add(update.Set(_modifiedTmsTmp, DateTime.UtcNow));
                updates.Add(update.Set(_modifiedUser, _userProvider?.UserName));

                var historyPropertyExists = item!.GetType().GetProperties().ToList().Exists(x => x.Name.Equals(_history, StringComparison.Ordinal));
                var history = existedInMongo.Find(f => (f?.GetType().GetProperty(_id)?.GetValue(f)?.ToString() ?? "") == id!.ToString());

                if (history is not null && historyPropertyExists)
                    updates.Add(update.Push("History", history));

                var result = await collection.UpdateOneAsync(filter, update.Combine(updates), new UpdateOptions()
                {
                    IsUpsert = false
                });

                FilterDefinition<T> filterCount = Builders<T>.Filter.Eq(_id, id);

                if (result.ModifiedCount == 0 && (await collection.CountDocumentsAsync(filterCount) == 1))
                    updateResult.Add((item!.GetType().GetProperty(_id)?.GetValue(item)?.ToString()!, false, "Dane nie zostały zapisane. Ktoś w międzyczasie wykonał ich zmianę"));
                else
                    updateResult.Add((item!.GetType().GetProperty(_id)?.GetValue(item)?.ToString()!, true, string.Empty));

            }
            finally
            {
                throtler.Release();
            }


        });

        return updateResult;
    }

    public async Task UpdateOne<T>(T entity) where T : class
    {
        var collection = await _databaseContext.GetCollection<T>(typeof(T).Name);

        List<string> existedPosition =
        [
            entity!.GetType().GetProperty(_id)?.GetValue(entity)?.ToString()!
        ];

        FilterDefinition<T> filterSelect = Builders<T>.Filter.In(_id, existedPosition);
        var existedInMongo = await collection.Find(filterSelect).Project<T>(Builders<T>.Projection.Exclude(_history)).ToListAsync();

        long currentVersion = 0;

        try
        {
            var modifiedTmsTmp = entity!.GetType().GetProperty(_modifiedTmsTmp)!;
            modifiedTmsTmp.SetValue(entity, DateTime.UtcNow, null);

            var modifiedUser = entity!.GetType().GetProperty(_modifiedUser)!;
            modifiedUser.SetValue(entity, _userProvider?.UserName, null);

            var version = entity!.GetType().GetProperty(_version)!;
            currentVersion = Convert.ToInt64(version.GetValue(entity));
        }
        catch
        {
            // no catch
        }

        var id = entity!.GetType().GetProperty(_id)!.GetValue(entity);

        FilterDefinition<T> filter = Builders<T>.Filter.Eq(_id, id)
            & Builders<T>.Filter.Eq(_version, currentVersion);

        var update = Builders<T>.Update;
        var updates = new List<UpdateDefinition<T>>();

        foreach (var prop in entity!.GetType().GetProperties())
        {
            var value = entity!.GetType().GetProperty(prop.Name)!.GetValue(entity);

            if (value is not null && !prop.CustomAttributes.Any(a => a.AttributeType.Name.Equals("IgnoreForMongoUpdateAttribute")))
                updates.Add(update.Set(prop.Name, value));
        }

        updates.Add(update.Inc(_version, 1));
        updates.Add(update.Set(_modifiedTmsTmp, DateTime.UtcNow));
        updates.Add(update.Set(_modifiedUser, _userProvider?.UserName));

        var historyPropertyExists = entity!.GetType().GetProperties().ToList().Exists(x => x.Name.Equals(_history, StringComparison.Ordinal));
        var history = existedInMongo.Find(f => (f?.GetType().GetProperty(_id)?.GetValue(f)?.ToString() ?? "") == id!.ToString());

        if (history is not null && historyPropertyExists)
            updates.Add(update.Push(_history, history));

        var result = await collection.UpdateOneAsync(filter, update.Combine(updates), new UpdateOptions()
        {
            IsUpsert = false
        });

        FilterDefinition<T> filterCount = Builders<T>.Filter.Eq(_id, id);

        if (result.ModifiedCount == 0 && (await collection.CountDocumentsAsync(filterCount) == 1))
        {
            throw new DbUpdateConcurrencyException("Dane nie zostały zapisane. Ktoś w międzyczasie wykonał ich zmianę. Odśwież i spróbuj ponownie");
        }
    }

}
