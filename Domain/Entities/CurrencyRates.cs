using Domain.Common;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Entities;

public class CurrencyRates : BaseEntity
{
    public string No { get; set; }
    [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
    public DateTime EffectiveDate { get; set; }
    public Rates[] Rates { get; set; }
}

public class Rates
{
    public string Currency { get; set; }
    public string Code { get; set; }
    [BsonRepresentation(BsonType.Decimal128)]
    public decimal Mid { get; set; }
}
