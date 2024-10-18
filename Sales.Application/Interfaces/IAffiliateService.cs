using System.Linq.Expressions;
using Sales.Application.DTOs.AffiliateDTO;
using Sales.Application.DTOs.OrderDTO;
using Sales.Application.Parameters.ModelsParameters;
using Sales.Application.ResultPattern;
using Sales.Domain.Models;
using X.PagedList;

namespace Sales.Application.Interfaces;

public interface IAffiliateService
{
    Task<IEnumerable<AffiliateDTOOutput>> GetAllAffiliate();
    Task<IPagedList<AffiliateDTOOutput>> GetAllAffiliate(AffiliateParameters parameters);
    Task<Result<AffiliateDTOOutput>> GetAffiliateById(int id);
    Task<Result<AffiliateDTOOutput>> GetAffiliateBy(Expression<Func<Affiliate, bool>> expression);
    Task<Result<AffiliateDTOOutput>> CreateAffiliate(AffiliateDTOInput affiliate);
    Task<Result<AffiliateDTOOutput>> UpdateAffiliate(AffiliateDTOInput affiliate, int id);
    Task<Result<AffiliateDTOOutput>> DeleteAffiliate(int? id); 
}