using System.Linq.Expressions;
using System.Net;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Sales.API.Controllers;
using Sales.Application.DTOs.AffiliateDTO;
using Sales.Application.Interfaces;
using Sales.Application.Parameters.ModelsParameters;
using Sales.Application.ResultPattern;
using Sales.Domain.Models;
using X.PagedList.Extensions;

namespace Sales.Test.ControllersTests;

public class AffiliateControllerTest
{
    private readonly AffiliatesController _controller;
    private readonly IAffiliateService _mockAffiliateService;
    private readonly Fixture _fixture;

    public AffiliateControllerTest()
    {
        _mockAffiliateService = Substitute.For<IAffiliateService>();
        
        _fixture = new Fixture();
        
        _controller = new AffiliatesController(_mockAffiliateService)
        {
            ControllerContext =
            {
                HttpContext = new DefaultHttpContext()
            }
        };
    }

    [Fact]
    public async Task Get_ShouldReturnAllAffiliates()
    {
        // Arrange
        var parameters = new AffiliateParameters();
        var affiliates = _fixture.CreateMany<AffiliateDTOOutput>(3).ToPagedList();

        _mockAffiliateService.GetAllAffiliate(Arg.Any<AffiliateParameters>())
            .Returns(affiliates);

        // Act
        var result = await _controller.Get(parameters);
        var obj = result.Result as ObjectResult;

        // Assert
        result.Result.Should().NotBeNull();
        result.Should().BeOfType<ActionResult<IEnumerable<AffiliateDTOOutput>>>();
        result.Result.Should().BeOfType<OkObjectResult>()
            .Which.StatusCode.Should().Be(200);
        obj.Value.Should().BeEquivalentTo(affiliates);
        
        var httpContext = _controller.ControllerContext.HttpContext;
        
        httpContext.Response.Headers.ContainsKey("X-Pagination").Should().BeTrue();

        await _mockAffiliateService.Received(1).GetAllAffiliate(Arg.Any<AffiliateParameters>());
    }
    
    [Fact]
    public async Task GetAffiliateById_ShouldReturnAffiliateById()
    {
        // Arrange
        var affiliateId = _fixture.Create<int>();
        var affiliate = _fixture.Create<AffiliateDTOOutput>();
        var affiliateResult = Result<AffiliateDTOOutput>.Success(affiliate);

        _mockAffiliateService.GetAffiliateBy(Arg.Any<Expression<Func<Affiliate, bool>>>())
            .Returns(affiliateResult);

        // Act
        var result = await _controller.GetAffiliate(affiliateId);
        var obj = result.Result as ObjectResult;

        // Assert
        result.Result.Should().NotBeNull();
        result.Should().BeOfType<ActionResult<AffiliateDTOOutput>>();
        result.Result.Should().BeOfType<OkObjectResult>()
            .Which.StatusCode.Should().Be(200);
        obj.Value.Should().BeEquivalentTo(affiliateResult.value);
        
        await _mockAffiliateService.Received(1).GetAffiliateBy(Arg.Any<Expression<Func<Affiliate, bool>>>());
    }
    
    [Fact]
    public async Task GetAffiliateById_ShouldReturn400NotFoundWithErrorResponse_WhenAffiliateDoesNotExist()
    {
        // Arrange
        var affiliateId = _fixture.Create<int>();
        var error = _fixture.Create<Error>();
        var affiliateResult = Result<AffiliateDTOOutput>.Failure(error);

        _mockAffiliateService.GetAffiliateBy(Arg.Any<Expression<Func<Affiliate, bool>>>())
            .Returns(affiliateResult);

        // Act
        var result = await _controller.GetAffiliate(affiliateId);
        var obj = result.Result as ObjectResult;

        // Assert
        result.Result.Should().NotBeNull();
        result.Should().BeOfType<ActionResult<AffiliateDTOOutput>>();
        result.Result.Should().BeOfType<NotFoundObjectResult>()
            .Which.StatusCode.Should().Be(404);
        obj.Value.Should().BeEquivalentTo(affiliateResult.GenerateErrorResponse());
        
        await _mockAffiliateService.Received(1).GetAffiliateBy(Arg.Any<Expression<Func<Affiliate, bool>>>());
    }
    
    [Fact]
    public async Task Post_ShouldReturn201CreatedAtRouteResultWithAffiliate_WhenAffiliateIsCreatedSuccessfully()
    {
        // Arrange
        var affiliateInput= _fixture.Create<AffiliateDTOInput>();
        var affiliate = _fixture.Create<AffiliateDTOOutput>();
        var affiliateResult = Result<AffiliateDTOOutput>.Success(affiliate);

        _mockAffiliateService.CreateAffiliate(Arg.Any<AffiliateDTOInput>())
            .Returns(affiliateResult);

        // Act
        var result = await _controller.Post(affiliateInput);
        var obj = result.Result as ObjectResult;

        // Assert
        result.Result.Should().NotBeNull();
        result.Should().BeOfType<ActionResult<AffiliateDTOOutput>>();
        result.Result.Should().BeOfType<CreatedAtRouteResult>()
            .Which.StatusCode.Should().Be(201);
        obj.Value.Should().BeEquivalentTo(affiliateResult.value);
        
        await _mockAffiliateService.Received(1).CreateAffiliate(Arg.Any<AffiliateDTOInput>());
    }
    
    [Fact]
    public async Task Post_ShouldReturn400BadRequestWithErrorResponse_WhenAffiliateIsCreatedWithErrors()
    {
        // Arrange
        var affiliateInput= _fixture.Create<AffiliateDTOInput>();
        var error = _fixture.Create<Error>();
        var affiliateResult = Result<AffiliateDTOOutput>.Failure(error);

        _mockAffiliateService.CreateAffiliate(Arg.Any<AffiliateDTOInput>())
            .Returns(affiliateResult);

        // Act
        var result = await _controller.Post(affiliateInput);
        var obj = result.Result as ObjectResult;

        // Assert
        result.Result.Should().NotBeNull();
        result.Should().BeOfType<ActionResult<AffiliateDTOOutput>>();
        result.Result.Should().BeOfType<BadRequestObjectResult>()
            .Which.StatusCode.Should().Be(400);
        obj.Value.Should().BeEquivalentTo(affiliateResult.GenerateErrorResponse());
        
        await _mockAffiliateService.Received(1).CreateAffiliate(Arg.Any<AffiliateDTOInput>());
    }
    
    [Fact]
    public async Task Put_ShouldReturn200OkResultWithAffiliateUpdated_WhenAffiliateIsUpdatedSuccessfully()
    {
        // Arrange
        var affiliateInput= _fixture.Create<AffiliateDTOInput>();
        var affiliateId = affiliateInput.AffiliateId;
        var affiliate = _fixture.Create<AffiliateDTOOutput>();
        var affiliateResult = Result<AffiliateDTOOutput>.Success(affiliate);

        _mockAffiliateService.UpdateAffiliate(Arg.Any<AffiliateDTOInput>(), Arg.Any<int>())
            .Returns(affiliateResult);

        // Act
        var result = await _controller.Put(affiliateId, affiliateInput);
        var obj = result.Result as ObjectResult;

        // Assert
        result.Result.Should().NotBeNull();
        result.Should().BeOfType<ActionResult<AffiliateDTOOutput>>();
        result.Result.Should().BeOfType<OkObjectResult>()
            .Which.StatusCode.Should().Be(200);
        obj.Value.Should().BeEquivalentTo(
            $"Affiliate with id = {affiliateResult.value.AffiliateId} was updated successfully");
        
        await _mockAffiliateService.Received(1).UpdateAffiliate(Arg.Any<AffiliateDTOInput>(), Arg.Any<int>());
    }
    
    [Fact]
    public async Task Put_ShouldReturn404NotFoundWithErrorResponse_WhenAffiliateDoesNotExist()
    {
        // Arrange
        var affiliateInput= _fixture.Create<AffiliateDTOInput>();
        var affiliateId = affiliateInput.AffiliateId;
        var error = new Error(
            "NotFound",
            "NotFound",
            HttpStatusCode.NotFound);
        var affiliateResult = Result<AffiliateDTOOutput>.Failure(error);

        _mockAffiliateService.UpdateAffiliate(Arg.Any<AffiliateDTOInput>(), Arg.Any<int>())
            .Returns(affiliateResult);

        // Act
        var result = await _controller.Put(affiliateId, affiliateInput);
        var obj = result.Result as ObjectResult;

        // Assert
        result.Result.Should().NotBeNull();
        result.Should().BeOfType<ActionResult<AffiliateDTOOutput>>();
        result.Result.Should().BeOfType<NotFoundObjectResult>()
            .Which.StatusCode.Should().Be(404);
        obj.Value.Should().BeEquivalentTo(affiliateResult.GenerateErrorResponse());
        
        await _mockAffiliateService.Received(1).UpdateAffiliate(Arg.Any<AffiliateDTOInput>(), Arg.Any<int>());
    }
    
    [Fact]
    public async Task Put_ShouldReturn400BadRequestWithErrorResponse_WhenAffiliateInputIsInvalid()
    {
        // Arrange
        var affiliateInput= _fixture.Create<AffiliateDTOInput>();
        var affiliateId = affiliateInput.AffiliateId;
        var error = new Error(
            "BadRequest",
            "BadRequest",
            HttpStatusCode.BadRequest);
        var affiliateResult = Result<AffiliateDTOOutput>.Failure(error);

        _mockAffiliateService.UpdateAffiliate(Arg.Any<AffiliateDTOInput>(), Arg.Any<int>())
            .Returns(affiliateResult);

        // Act
        var result = await _controller.Put(affiliateId, affiliateInput);
        var obj = result.Result as ObjectResult;

        // Assert
        result.Result.Should().NotBeNull();
        result.Should().BeOfType<ActionResult<AffiliateDTOOutput>>();
        result.Result.Should().BeOfType<BadRequestObjectResult>()
            .Which.StatusCode.Should().Be(400);
        obj.Value.Should().BeEquivalentTo(affiliateResult.GenerateErrorResponse());
        
        await _mockAffiliateService.Received(1).UpdateAffiliate(Arg.Any<AffiliateDTOInput>(), Arg.Any<int>());
    }
    
    [Fact]
    public async Task Delete_ShouldReturn200OkResultWithAffiliateDeleted()
    {
        // Arrange
        var affiliateId = _fixture.Create<int>();;
        var affiliate = _fixture.Create<AffiliateDTOOutput>();
        var affiliateResult = Result<AffiliateDTOOutput>.Success(affiliate);

        _mockAffiliateService.DeleteAffiliate(Arg.Any<int>())
            .Returns(affiliateResult);

        // Act
        var result = await _controller.Delete(affiliateId);
        var obj = result.Result as ObjectResult;

        // Assert
        result.Result.Should().NotBeNull();
        result.Should().BeOfType<ActionResult<AffiliateDTOOutput>>();
        result.Result.Should().BeOfType<OkObjectResult>()
            .Which.StatusCode.Should().Be(200);
        obj.Value.Should().BeEquivalentTo(
            $"Affiliate with id = {affiliateResult.value.AffiliateId} was deleted successfully");
        
        await _mockAffiliateService.Received(1).DeleteAffiliate(Arg.Any<int>());
    }
    
    [Fact]
    public async Task Delete_ShouldReturn404NotFoundWithErrorResponse_WhenAffiliateDoesNotExist()
    {
        // Arrange
        var affiliateId = _fixture.Create<int>();;
        var error = _fixture.Create<Error>();
        var affiliateResult = Result<AffiliateDTOOutput>.Failure(error);

        _mockAffiliateService.DeleteAffiliate(Arg.Any<int>())
            .Returns(affiliateResult);

        // Act
        var result = await _controller.Delete(affiliateId);
        var obj = result.Result as ObjectResult;

        // Assert
        result.Result.Should().NotBeNull();
        result.Should().BeOfType<ActionResult<AffiliateDTOOutput>>();
        result.Result.Should().BeOfType<NotFoundObjectResult>()
            .Which.StatusCode.Should().Be(404);
        obj.Value.Should().BeEquivalentTo(affiliateResult.GenerateErrorResponse());
        
        await _mockAffiliateService.Received(1).DeleteAffiliate(Arg.Any<int>());
    }
}