using MongoDB.Bson.Serialization.Attributes;
using Shared.Attributes;

namespace Domain.Common;

public class BaseEntity : CreateBaseEntity
{
    [IgnoreForMongoUpdate]
    [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
    public DateTime? ModifiedTmsTmp { get; set; }

    [IgnoreForMongoUpdate]
    public string? ModifiedUser { get; set; }
}
