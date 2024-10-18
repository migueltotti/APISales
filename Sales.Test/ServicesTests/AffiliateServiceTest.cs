using System.Linq.Expressions;
using AutoFixture;
using AutoMapper;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using Sales.Application.DTOs.AffiliateDTO;
using Sales.Application.Interfaces;
using Sales.Application.Parameters;
using Sales.Application.Parameters.ModelsParameters;
using Sales.Application.ResultPattern;
using Sales.Application.Services;
using Sales.Domain.Interfaces;
using Sales.Domain.Models;
using X.PagedList.Extensions;

namespace Sales.Test.ServicesTests;

public class AffiliateServiceTest
{
    private readonly IAffiliateService _affiliateService;
    private readonly IMapper _mockMapper;
    private readonly IUnitOfWork _mockUnitOfWork;
    private readonly IValidator<AffiliateDTOInput> _mockValidator;
    private readonly Fixture _fixture;

    public AffiliateServiceTest()
    {
        _mockUnitOfWork = Substitute.For<IUnitOfWork>();
        _mockMapper = Substitute.For<IMapper>();
        _mockValidator = Substitute.For<IValidator<AffiliateDTOInput>>();
        _fixture = new Fixture();

        _affiliateService = new AffiliateService(
            _mockMapper,
            _mockUnitOfWork,
            _mockValidator);
    }

    [Fact]
    public async Task GetAllAffiliate_ShouldReturnAllAffiliates()
    {
        // Arrange
        var affiliates = _fixture.CreateMany<Affiliate>(3);
        var affiliatesDto = _fixture.CreateMany<AffiliateDTOOutput>(3);

        _mockUnitOfWork.AffiliateRepository.GetAllAsync().Returns(affiliates);
        _mockMapper.Map<IEnumerable<AffiliateDTOOutput>>(Arg.Any<IEnumerable<Affiliate>>())
            .Returns(affiliatesDto);

        // Act
        var result = await _affiliateService.GetAllAffiliate();

        // Assert
        result.Should().BeEquivalentTo(affiliatesDto);

        await _mockUnitOfWork.AffiliateRepository.Received(1).GetAllAsync();
        _mockMapper.Received(1).Map<IEnumerable<AffiliateDTOOutput>>(Arg.Any<IEnumerable<Affiliate>>());
    }

    [Fact]
    public async Task GetAllAffiliatePaged_ShouldReturnAllAffiliatesPaged()
    {
        // Arrange
        var parameters = new AffiliateParameters();
        var affiliates = _fixture.CreateMany<Affiliate>(3);
        var affiliatesDto = _fixture.CreateMany<AffiliateDTOOutput>(3);

        _mockUnitOfWork.AffiliateRepository.GetAllAsync().Returns(affiliates);
        _mockMapper.Map<IEnumerable<AffiliateDTOOutput>>(Arg.Any<IEnumerable<Affiliate>>())
            .Returns(affiliatesDto);

        // Act
        var result = await _affiliateService.GetAllAffiliate(parameters);

        // Assert
        result.Should().BeEquivalentTo(affiliatesDto.ToPagedList(
            parameters.PageNumber, parameters.PageSize));

        await _mockUnitOfWork.AffiliateRepository.Received(1).GetAllAsync();
        _mockMapper.Received(1).Map<IEnumerable<AffiliateDTOOutput>>(Arg.Any<IEnumerable<Affiliate>>());
    }
    
    [Fact]
    public async Task GetAffiliateById_ShouldReturnAffiliateThatMatchesId()
    {
        // Arrange
        var affiliateId = _fixture.Create<int>();
        var affiliate = _fixture.Create<Affiliate>();
        var affiliateDto = _fixture.Create<AffiliateDTOOutput>();

        _mockUnitOfWork.AffiliateRepository.GetByIdAsync(Arg.Any<int>())
            .Returns(affiliate);
        _mockMapper.Map<AffiliateDTOOutput>(Arg.Any<Affiliate>())
            .Returns(affiliateDto);

        // Act
        var result = await _affiliateService.GetAffiliateById(affiliateId);

        // Assert
        result.isSuccess.Should().BeTrue();
        result.value.Should().BeEquivalentTo(affiliateDto);

        await _mockUnitOfWork.AffiliateRepository.Received(1).GetByIdAsync(Arg.Any<int>());
        _mockMapper.Received(1).Map<AffiliateDTOOutput>(Arg.Any<Affiliate>());
    }

    [Fact]
    public async Task GetAffiliateById_ShouldReturnNotFoundWithNotFoundResponse_WhenAffiliateDoesNotExist()
    {
        // Arrange
        var affiliateId = _fixture.Create<int>();
        Affiliate affiliate = null;

        _mockUnitOfWork.AffiliateRepository.GetByIdAsync(Arg.Any<int>())
            .Returns(affiliate);

        // Act
        var result = await _affiliateService.GetAffiliateById(affiliateId);

        // Assert
        result.isSuccess.Should().BeFalse();
        result.error.Should().BeEquivalentTo(AffiliateErros.NotFound);

        await _mockUnitOfWork.AffiliateRepository.Received(1).GetByIdAsync(Arg.Any<int>());
    }

    [Fact]
    public async Task GetAffiliateBy_ShouldReturnAffiliateThatMatchesSomeExpression()
    {
        // Arrange
        var affiliate = _fixture.Create<Affiliate>();
        var affiliateDto = _fixture.Create<AffiliateDTOOutput>();

        _mockUnitOfWork.AffiliateRepository.GetAsync(Arg.Any<Expression<Func<Affiliate, bool>>>())
            .Returns(affiliate);
        _mockMapper.Map<AffiliateDTOOutput>(Arg.Any<Affiliate>())
            .Returns(affiliateDto);

        // Act
        var result = await _affiliateService.GetAffiliateBy(a => a.AffiliateId == 1);

        // Assert
        result.isSuccess.Should().BeTrue();
        result.value.Should().BeEquivalentTo(affiliateDto);

        await _mockUnitOfWork.AffiliateRepository.Received(1).GetAsync(Arg.Any<Expression<Func<Affiliate, bool>>>());
        _mockMapper.Received(1).Map<AffiliateDTOOutput>(Arg.Any<Affiliate>());
    }

    [Fact]
    public async Task GetAffiliateBy_ShouldReturnNotFoundWithNotFoundResponse_WhenAffiliateDoesNotExist()
    {
        // Arrange
        Affiliate affiliate = null;

        _mockUnitOfWork.AffiliateRepository.GetAsync(Arg.Any<Expression<Func<Affiliate, bool>>>())
            .Returns(affiliate);

        // Act
        var result = await _affiliateService.GetAffiliateBy(a => a.AffiliateId == 1);

        // Assert
        result.isSuccess.Should().BeFalse();
        result.error.Should().BeEquivalentTo(AffiliateErros.NotFound);

        await _mockUnitOfWork.AffiliateRepository.Received(1).GetAsync(Arg.Any<Expression<Func<Affiliate, bool>>>());
    }

    [Fact]
    public async Task CreateAffiliate_ShouldReturnCreatedAffiliate()
    {
        // Arrange
        var affiliateInput = _fixture.Create<AffiliateDTOInput>();
        var validation = new ValidationResult()
        {
            Errors = new List<ValidationFailure>()
        };
        Affiliate affiliate = null;
        var affiliateToCreate = _fixture.Create<Affiliate>();
        var affiliateCreatedDto = _fixture.Create<AffiliateDTOOutput>();

        _mockValidator.ValidateAsync(Arg.Any<AffiliateDTOInput>())
            .Returns(validation);
        _mockUnitOfWork.AffiliateRepository.GetAsync(Arg.Any<Expression<Func<Affiliate, bool>>>())
            .Returns(affiliate);
        _mockMapper.Map<Affiliate>(Arg.Any<AffiliateDTOInput>())
            .Returns(affiliateToCreate);
        _mockUnitOfWork.AffiliateRepository.Create(Arg.Any<Affiliate>())
            .Returns(affiliate);
        _mockMapper.Map<AffiliateDTOOutput>(Arg.Any<Affiliate>())
            .Returns(affiliateCreatedDto);

        // Act
        var result = await _affiliateService.CreateAffiliate(affiliateInput);

        // Assert
        result.isSuccess.Should().BeTrue();
        result.value.Should().BeEquivalentTo(affiliateCreatedDto);

        await _mockValidator.Received(1).ValidateAsync(Arg.Any<AffiliateDTOInput>());
        await _mockUnitOfWork.AffiliateRepository.Received(1).GetAsync(Arg.Any<Expression<Func<Affiliate, bool>>>());
        _mockMapper.Received(1).Map<Affiliate>(Arg.Any<AffiliateDTOInput>());
        _mockUnitOfWork.AffiliateRepository.Received(1).Create(Arg.Any<Affiliate>());
        _mockMapper.Received(1).Map<AffiliateDTOOutput>(Arg.Any<Affiliate>());
    }

    [Fact]
    public async Task CreateAffiliate_ShouldReturnBadRequestWithDuplicateDataResponse_WhenAffiliateAlreadyExists()
    {
        // Arrange
        var affiliateInput = _fixture.Create<AffiliateDTOInput>();
        var validation = new ValidationResult()
        {
            Errors = new List<ValidationFailure>()
        };
        var affiliate = _fixture.Create<Affiliate>();

        _mockValidator.ValidateAsync(Arg.Any<AffiliateDTOInput>())
            .Returns(validation);
        _mockUnitOfWork.AffiliateRepository.GetAsync(Arg.Any<Expression<Func<Affiliate, bool>>>())
            .Returns(affiliate);

        // Act
        var result = await _affiliateService.CreateAffiliate(affiliateInput);

        // Assert
        result.isSuccess.Should().BeFalse();
        result.error.Should().BeEquivalentTo(AffiliateErros.DuplicateData);

        await _mockValidator.Received(1).ValidateAsync(Arg.Any<AffiliateDTOInput>());
        await _mockUnitOfWork.AffiliateRepository.Received(1).GetAsync(Arg.Any<Expression<Func<Affiliate, bool>>>());
    }

    [Fact]
    public async Task CreateAffiliate_ShouldReturnBadRequestWithInputDataInvalidResponse_WhenAffiliateInputIsInvalid()
    {
        // Arrange
        var affiliateInput = _fixture.Create<AffiliateDTOInput>();
        var validation = new ValidationResult()
        {
            Errors = new List<ValidationFailure>()
            {
                new ValidationFailure("InvalidInputData", "InvalidInputData")
            }
        };

        _mockValidator.ValidateAsync(Arg.Any<AffiliateDTOInput>())
            .Returns(validation);

        // Act
        var result = await _affiliateService.CreateAffiliate(affiliateInput);

        // Assert
        result.isSuccess.Should().BeFalse();
        result.error.Should().BeEquivalentTo(AffiliateErros.IncorrectFormatData);

        await _mockValidator.Received(1).ValidateAsync(Arg.Any<AffiliateDTOInput>());
    }

    [Fact]
    public async Task CreateAffiliate_ShouldReturnBadRequestWithDataIsNullResponse_WhenAffiliateInputIsNull()
    {
        // Arrange
        AffiliateDTOInput affiliateInput = null;

        // Act
        var result = await _affiliateService.CreateAffiliate(affiliateInput);

        // Assert
        result.isSuccess.Should().BeFalse();
        result.error.Should().BeEquivalentTo(AffiliateErros.DataIsNull);
    }

    [Fact]
    public async Task UpdateAffiliate_ShouldReturnUpdatedAffiliate()
    {
        // Arrange
        var affiliateInput = _fixture.Create<AffiliateDTOInput>();
        var affiliateId = affiliateInput.AffiliateId;
        var affiliateExists = _fixture.Create<Affiliate>();
        var validation = new ValidationResult()
        {
            Errors = new List<ValidationFailure>()
        };

        var affiliateToUpdate = _fixture.Create<Affiliate>();
        var affiliateUpdateDto = _fixture.Create<AffiliateDTOOutput>();

        _mockUnitOfWork.AffiliateRepository.GetAsync(Arg.Any<Expression<Func<Affiliate, bool>>>())
            .Returns(affiliateExists);
        _mockValidator.ValidateAsync(Arg.Any<AffiliateDTOInput>())
            .Returns(validation);
        _mockMapper.Map<Affiliate>(Arg.Any<AffiliateDTOInput>())
            .Returns(affiliateToUpdate);
        _mockUnitOfWork.AffiliateRepository.Update(Arg.Any<Affiliate>())
            .Returns(affiliateToUpdate);
        _mockMapper.Map<AffiliateDTOOutput>(Arg.Any<Affiliate>())
            .Returns(affiliateUpdateDto);

        // Act
        var result = await _affiliateService.UpdateAffiliate(affiliateInput, affiliateId);

        // Assert
        result.isSuccess.Should().BeTrue();
        result.value.Should().BeEquivalentTo(affiliateUpdateDto);

        await _mockUnitOfWork.AffiliateRepository.Received(1).GetAsync(Arg.Any<Expression<Func<Affiliate, bool>>>());
        await _mockValidator.Received(1).ValidateAsync(Arg.Any<AffiliateDTOInput>());
        _mockMapper.Received(1).Map<Affiliate>(Arg.Any<AffiliateDTOInput>());
        _mockUnitOfWork.AffiliateRepository.Received(1).Update(Arg.Any<Affiliate>());
        _mockMapper.Received(1).Map<AffiliateDTOOutput>(Arg.Any<Affiliate>());
    }

    [Fact]
    public async Task UpdateAffiliate_ShouldReturnBadRequestWithInvalidInptData_WhenAffiliateInputIsInvalid()
    {
        // Arrange
        var affiliateInput = _fixture.Create<AffiliateDTOInput>();
        var affiliateId = affiliateInput.AffiliateId;
        var affiliateExists = _fixture.Create<Affiliate>();
        var validation = new ValidationResult()
        {
            Errors = new List<ValidationFailure>()
            {
                new ValidationFailure("InvalidInputData", "InvalidInputData")
            }
        };

        _mockUnitOfWork.AffiliateRepository.GetAsync(Arg.Any<Expression<Func<Affiliate, bool>>>())
            .Returns(affiliateExists);
        _mockValidator.ValidateAsync(Arg.Any<AffiliateDTOInput>())
            .Returns(validation);

        // Act
        var result = await _affiliateService.UpdateAffiliate(affiliateInput, affiliateId);

        // Assert
        result.isSuccess.Should().BeFalse();
        result.error.Should().BeEquivalentTo(AffiliateErros.IncorrectFormatData);

        await _mockUnitOfWork.AffiliateRepository.Received(1).GetAsync(Arg.Any<Expression<Func<Affiliate, bool>>>());
        await _mockValidator.Received(1).ValidateAsync(Arg.Any<AffiliateDTOInput>());
    }

    [Fact]
    public async Task UpdateAffiliate_ShouldReturnNotFoundWithNotFoundResponse_WhenAffiliateDoesNotExist()
    {
        // Arrange
        var affiliateInput = _fixture.Create<AffiliateDTOInput>();
        var affiliateId = affiliateInput.AffiliateId;
        Affiliate affiliateExists = null;

        _mockUnitOfWork.AffiliateRepository.GetAsync(Arg.Any<Expression<Func<Affiliate, bool>>>())
            .Returns(affiliateExists);

        // Act
        var result = await _affiliateService.UpdateAffiliate(affiliateInput, affiliateId);

        // Assert
        result.isSuccess.Should().BeFalse();
        result.error.Should().BeEquivalentTo(AffiliateErros.NotFound);

        await _mockUnitOfWork.AffiliateRepository.Received(1).GetAsync(Arg.Any<Expression<Func<Affiliate, bool>>>());
    }

    [Fact]
    public async Task UpdateAffiliate_ShouldReturnBadRequestWithIdMismacthResponse_WhenAffiliateAndAffiliateIdMismatch()
    {
        // Arrange
        var affiliateInput = _fixture.Create<AffiliateDTOInput>();
        var affiliateId = _fixture.Create<int>();

        // Act
        var result = await _affiliateService.UpdateAffiliate(affiliateInput, affiliateId);

        // Assert
        result.isSuccess.Should().BeFalse();
        result.error.Should().BeEquivalentTo(AffiliateErros.IdMismatch);
    }

    [Fact]
    public async Task UpdateAffiliate_ShouldReturnBadRequestWithDataIsNullResponse_WhenAffiliateInputIsNull()
    {
        // Arrange
        AffiliateDTOInput affiliateInput = null;
        var affiliateId = _fixture.Create<int>();

        // Act
        var result = await _affiliateService.UpdateAffiliate(affiliateInput, affiliateId);

        // Assert
        result.isSuccess.Should().BeFalse();
        result.error.Should().BeEquivalentTo(AffiliateErros.DataIsNull);
    }

    [Fact]
    public async Task DeleteAffiliate_ShouldReturnDeletedAffiliate()
    {
        // Arrange
        var affiliateId = _fixture.Create<int>();
        var affiliate = _fixture.Create<Affiliate>();
        var affiliateDto = _fixture.Create<AffiliateDTOOutput>();

        _mockUnitOfWork.AffiliateRepository.GetAsync(Arg.Any<Expression<Func<Affiliate, bool>>>())
            .Returns(affiliate);
        _mockUnitOfWork.AffiliateRepository.Delete(Arg.Any<Affiliate>())
            .Returns(affiliate);
        _mockMapper.Map<AffiliateDTOOutput>(Arg.Any<Affiliate>())
            .Returns(affiliateDto);

        // Act
        var result = await _affiliateService.DeleteAffiliate(affiliateId);

        // Assert
        result.isSuccess.Should().BeTrue();
        result.value.Should().BeEquivalentTo(affiliateDto);

        await _mockUnitOfWork.AffiliateRepository.Received(1).GetAsync(Arg.Any<Expression<Func<Affiliate, bool>>>());
        _mockUnitOfWork.AffiliateRepository.Received(1).Delete(Arg.Any<Affiliate>());
        _mockMapper.Received(1).Map<AffiliateDTOOutput>(Arg.Any<Affiliate>());
    }

    [Fact]
    public async Task DeleteAffiliate_ShouldReturnNotFoundWithNotFoundResponse_WhenAffiliateDoesNotExist()
    {
        // Arrange
        var affiliateId = _fixture.Create<int>();
        Affiliate affiliate = null;

        _mockUnitOfWork.AffiliateRepository.GetAsync(Arg.Any<Expression<Func<Affiliate, bool>>>())
            .Returns(affiliate);

        // Act
        var result = await _affiliateService.DeleteAffiliate(affiliateId);

        // Assert
        result.isSuccess.Should().BeFalse();
        result.error.Should().BeEquivalentTo(AffiliateErros.NotFound);

        await _mockUnitOfWork.AffiliateRepository.Received(1).GetAsync(Arg.Any<Expression<Func<Affiliate, bool>>>());
    }
}