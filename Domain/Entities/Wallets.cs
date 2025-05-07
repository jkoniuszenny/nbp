using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

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
    public double Value { get; set; }
}