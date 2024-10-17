using System.Linq.Expressions;
using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Sales.Application.DTOs.AffiliateDTO;
using Sales.Application.Interfaces;
using Sales.Application.Parameters.ModelsParameters;
using Sales.Application.ResultPattern;
using Sales.Domain.Interfaces;
using Sales.Domain.Models;
using X.PagedList;
using X.PagedList.Extensions;

namespace Sales.Application.Services;

public class AffiliateService : IAffiliateService
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<AffiliateDTOInput> _validator;

    public AffiliateService(IMapper mapper, IUnitOfWork unitOfWork, IValidator<AffiliateDTOInput> validator)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }


    public async Task<IEnumerable<AffiliateDTOOutput>> GetAllAffiliate()
    {
        var affiliates = await _unitOfWork.AffiliateRepository.GetAllAsync();
        
        return _mapper.Map<IEnumerable<AffiliateDTOOutput>>(affiliates);
    }

    public async Task<IPagedList<AffiliateDTOOutput>> GetAllAffiliate(AffiliateParameters parameters)
    {
        var affiliates = await GetAllAffiliate();
        
        return affiliates.ToPagedList<AffiliateDTOOutput>(parameters.PageNumber, parameters.PageSize);
    }

    public async Task<Result<AffiliateDTOOutput>> GetAffiliateBy(Expression<Func<Affiliate, bool>> expression)
    {
        var affiliate = await _unitOfWork.AffiliateRepository.GetAsync(expression);

        if (affiliate is null)
            return Result<AffiliateDTOOutput>.Failure(AffiliateErros.NotFound);
        
        return Result<AffiliateDTOOutput>.Success(_mapper.Map<AffiliateDTOOutput>(affiliate));
    }

    public async Task<Result<AffiliateDTOOutput>> CreateAffiliate(AffiliateDTOInput affiliate)
    {
        if (affiliate is null)
            return Result<AffiliateDTOOutput>.Failure(AffiliateErros.DataIsNull);
        
        var validationResult = await _validator.ValidateAsync(affiliate);
        
        if (!validationResult.IsValid)
            return Result<AffiliateDTOOutput>.Failure(AffiliateErros.IncorrectFormatData, validationResult.Errors);
        
        var affiliateExists = await _unitOfWork.AffiliateRepository.GetAsync(a => a.Name == affiliate.Name);
        if (affiliateExists is not null)
            return Result<AffiliateDTOOutput>.Failure(AffiliateErros.DuplicateData);
            
        var affiliateEntity = _mapper.Map<Affiliate>(affiliate);
        
        var affiliateCreated = _unitOfWork.AffiliateRepository.Create(affiliateEntity);
        await _unitOfWork.CommitChanges();
        
        return Result<AffiliateDTOOutput>.Success(_mapper.Map<AffiliateDTOOutput>(affiliateCreated));
    }

    public async Task<Result<AffiliateDTOOutput>> UpdateAffiliate(AffiliateDTOInput affiliate, int id)
    {
        if (affiliate is null)
            return Result<AffiliateDTOOutput>.Failure(AffiliateErros.DataIsNull);
        
        if(affiliate.AffiliateId != id)
            return Result<AffiliateDTOOutput>.Failure(AffiliateErros.IdMismatch);
        
        var affiliateExists = await _unitOfWork.AffiliateRepository.GetAsync(p => p.AffiliateId == id);

        if (affiliateExists is null)
            return Result<AffiliateDTOOutput>.Failure(AffiliateErros.NotFound);
        
        var validationResult = await _validator.ValidateAsync(affiliate);
        
        if (!validationResult.IsValid)
            return Result<AffiliateDTOOutput>.Failure(AffiliateErros.IncorrectFormatData, validationResult.Errors);
        
        var affiliateEntity = _mapper.Map<Affiliate>(affiliate);
        
        var affiliateUpdated = _unitOfWork.AffiliateRepository.Update(affiliateEntity);
        _unitOfWork.CommitChanges();
        
        return Result<AffiliateDTOOutput>.Success(_mapper.Map<AffiliateDTOOutput>(affiliateUpdated));
    }

    public async Task<Result<AffiliateDTOOutput>> DeleteAffiliate(int? id)
    {
        var affiliate = await _unitOfWork.AffiliateRepository.GetAsync(a => a.AffiliateId == id);
        
        if(affiliate is null)
            return Result<AffiliateDTOOutput>.Failure(AffiliateErros.NotFound);
        
        
        var affiliateDeleted = _unitOfWork.AffiliateRepository.Delete(affiliate);
        _unitOfWork.CommitChanges();

        return Result<AffiliateDTOOutput>.Success(_mapper.Map<AffiliateDTOOutput>(affiliateDeleted));
    }
}