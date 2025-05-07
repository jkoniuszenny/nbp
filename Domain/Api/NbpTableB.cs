using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Api;

public record NbpTableB
(
     string Table,
     string No,
     DateTime EffectiveDate,
     List<Rate> Rates

);

public record Rate
(
     string Currency,
     string Code,
     double Mid
);
