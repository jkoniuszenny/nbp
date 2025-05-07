using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Shared.Attributes;

namespace Domain.Common;

public class CreateBaseEntity
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [IgnoreForMongoUpdate]
    public int Version { get; set; }

    [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
    public DateTime CreateTmsTmp { get; set; }

    public string? CreateUser { get; set; }
}