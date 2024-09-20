namespace Sales.Application.DTOs.AffiliateDTO;

public record AffiliateDTOOutput(
    int AffiliateId,
    string Name,
    decimal Discount
);
