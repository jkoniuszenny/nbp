using MongoDB.Bson;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace Application.Interfaces.Repositories;

public interface IAsyncRepository : IRepository
{
    IClientSessionHandle ClientSessionHandle { get; }

    Task<IQueryable<T>> AsQueryable<T>() where T : class;
    Task DeleteList<T>(IReadOnlyList<T> entities) where T : class;
    Task DeleteOne<T>(T entity) where T : class;
    Task InsertList<T>(IReadOnlyList<T> entities) where T : class;
    Task<IReadOnlyList<T>> Select<T>(Expression<Func<T, bool>> filters) where T : class;
    Task<IReadOnlyList<BsonDocument>> SelectParametrize<T>(FilterDefinition<T> filter, ProjectionDefinition<T> projection) where T : class;
    Task<IReadOnlyList<(string ObjectId, bool Success, string Error)>> UpdateList<T>(IReadOnlyList<T> entities) where T : class;
    Task UpdateOne<T>(T entity) where T : class;
}

