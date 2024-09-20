namespace Sales.Application.DTOs.AffiliateDTO;

public record AffiliateDTOInput(
    int AffiliateId,
    string Name,
    decimal Discount
);