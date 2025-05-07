using Domain.Common;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Entities;

public class Wallets : BaseEntity
{
    public string Name { get; set; }
    public List<Currency> Currencies { get; set; }

    public void AddCurrency(Currency currency)
    {
        var existingCurrency = Currencies.FirstOrDefault(c => c.Code == currency.Code);

        if (existingCurrency != null)
            existingCurrency.Value += currency.Value;
        else
            Currencies.Add(currency);
    }

    public void SubtractCurrency(Currency currency)
    {
        var existingCurrency = Currencies.FirstOrDefault(c => c.Code == currency.Code);

        if (existingCurrency != null)
            existingCurrency.Value -= currency.Value;
    }
}

public class Currency
{
    public string Name { get; set; }
    public string Code { get; set; }
    [BsonRepresentation(BsonType.Decimal128)]
    public decimal Value { get; set; }
}