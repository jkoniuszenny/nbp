using Domain.Common;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    public double Mid { get; set; }
}
