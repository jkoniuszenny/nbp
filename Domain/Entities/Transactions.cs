using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities;

public class Transactions:BaseEntity
{
    public string WalletId { get; set; }
    public string CurrencyCode { get; set; }
    public double Value { get; set; }
    public TransactionType TransactionType { get; set; }
}

public enum TransactionType
{
    Add,
    Subtract
}