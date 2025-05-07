using Domain.Common;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Entities;

public class Transactions:BaseEntity
{
    public string WalletId { get; set; }
    public string CurrencyCodeFrom { get; set; }
    public string CurrencyCodeTo { get; set; }
    [BsonRepresentation(BsonType.Decimal128)]
    public decimal Value { get; set; }
    public TransactionType TransactionType { get; set; }
}

public enum TransactionType
{
    Add,
    Subtract,
    Convert
}