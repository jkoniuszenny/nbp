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
     decimal Mid
);
