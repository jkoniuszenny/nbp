using Domain.Common;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Entities;

public class Wallets : BaseEntity
{
    public string Name { get; set; }
    public Currency[] Currencies { get; set; }
}

public class Currency
{
    public string Name { get; set; }
    public string Code { get; set; }
    [BsonRepresentation(BsonType.Decimal128)]
    public decimal Value { get; set; }
}