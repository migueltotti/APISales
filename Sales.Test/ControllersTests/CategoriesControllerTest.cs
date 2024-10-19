using System.Linq.Expressions;
using System.Net;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Sales.API.Controllers;
using Sales.Application.DTOs.CategoryDTO;
using Sales.Application.DTOs.ProductDTO;
using Sales.Application.DTOs.TokenDTO;
using Sales.Application.Interfaces;
using Sales.Application.Parameters;
using Sales.Application.Parameters.ModelsParameters;
using Sales.Application.ResultPattern;
using Sales.Domain.Models;
using X.PagedList.Extensions;

namespace Sales.Test.ControllersTests;

public class CategoriesControllerTest
{
    private readonly CategoriesController _categoriesController;
    private readonly ICategoryService _mockCategoriesService;
    private readonly ILogger<CategoriesController> _mockLogger;
    private readonly Fixture _fixture;

    public CategoriesControllerTest()
    {
        _mockCategoriesService = Substitute.For<ICategoryService>();
        _mockLogger = Substitute.For<ILogger<CategoriesController>>();
        _fixture = new Fixture();
        
        _categoriesController = new CategoriesController(_mockCategoriesService, _mockLogger);
        _categoriesController.ControllerContext.HttpContext = new DefaultHttpContext();
    }

    [Fact]
    public async Task GetAllCategories_ShouldReturnAllCategories()
    {
        // Arange
        var categories = _fixture.CreateMany<CategoryDTOOutput>(3).ToPagedList();
        var parameters = new QueryStringParameters();
        _mockCategoriesService.GetAllCategories(Arg.Any<QueryStringParameters>()).Returns(categories);
        
        // Act
        var result = await _categoriesController.Get(parameters);
        var obj = result.Result as ObjectResult;
        
        // Assert
        result.Result.Should().NotBeNull();
        result.Should().BeOfType<ActionResult<IEnumerable<CategoryDTOOutput>>>();
        result.Result.Should().BeOfType<OkObjectResult>()
            .Which.StatusCode.Should().Be(200);
        obj.Value.Should().BeEquivalentTo(categories);
        
        await _mockCategoriesService.Received(1).GetAllCategories(Arg.Any<QueryStringParameters>());
        var httpContext = _categoriesController.ControllerContext.HttpContext;
        httpContext.Response.Headers.ContainsKey("X-Pagination").Should().BeTrue();
    }

    [Fact]
    public async Task GetCategoryByName_ShouldReturnAllCategoriesWithMatchingName()
    {
        // Arrange
        var categories = _fixture.CreateMany<CategoryDTOOutput>(3).ToPagedList();
        var parameters = new CategoryParameters();
        _mockCategoriesService.GetCategoriesWithFilter("name", Arg.Any<CategoryParameters>())
            .Returns(categories);
        
        // Act
        var result = await _categoriesController.GetCategoriesByName(parameters);
        var obj = result.Result as ObjectResult;

        // Assent
        result.Result.Should().NotBeNull();
        result.Should().BeOfType<ActionResult<IEnumerable<CategoryDTOOutput>>>();
        result.Result.Should().BeOfType<OkObjectResult>()
            .Which.StatusCode.Should().Be(200);
        obj.Value.Should().BeEquivalentTo(categories);
        
        var httpContext = _categoriesController.ControllerContext.HttpContext;
        httpContext.Response.Headers.ContainsKey("X-Pagination").Should().BeTrue();
        await _mockCategoriesService.Received(1).GetCategoriesWithFilter("name", Arg.Any<CategoryParameters>());
    }

    [Fact]
    public async Task GetCategoryById_ShouldReturn200OkResultAndCategory_WhenCategoryExists()
    {
        // Arrange
        var category = _fixture.Create<Result<CategoryDTOOutput>>();
        var categoryId = _fixture.Create<int>();
        _mockCategoriesService.GetCategoryById(Arg.Any<int>())
            .Returns(category);
        
        // Act
        var result = await _categoriesController.GetCategory(categoryId);
        var obj = result.Result as ObjectResult;
        
        // Assert
        result.Result.Should().NotBeNull();
        result.Should().BeOfType<ActionResult<CategoryDTOOutput>>();
        result.Result.Should().BeOfType<OkObjectResult>()
            .Which.StatusCode.Should().Be(200);
        obj.Value.Should().BeEquivalentTo(category.value);
        
        await _mockCategoriesService.Received(1).GetCategoryById(Arg.Any<int>());
    }

    [Fact]
    public async Task GetCategoryById_ShouldReturn404NotFound_WhenCategoryDoesntExist()
    {
        // Arrange
        var error = _fixture.Create<Error>();
        var category = Result<CategoryDTOOutput>.Failure(error);
        var categoryId = _fixture.Create<int>();
        _mockCategoriesService.GetCategoryById(Arg.Any<int>()).Returns(category);
        
        // Act
        var result = await _categoriesController.GetCategory(categoryId);
        var obj = result.Result as ObjectResult;
        
        // Assert
        result.Result.Should().NotBeNull();
        result.Should().BeOfType<ActionResult<CategoryDTOOutput>>();
        result.Result.Should().BeOfType<NotFoundObjectResult>()
            .Which.StatusCode.Should().Be(404);
        obj.Value.Should().BeEquivalentTo(category.GenerateErrorResponse());
        
        await _mockCategoriesService.Received(1).GetCategoryById(Arg.Any<int>());
    }

    [Fact]
    public async Task GetCategoryProducts_ShouldReturnAllProductsByCategoryId()
    {
        // Arrange
        var categoryProducts = _fixture.CreateMany<ProductDTOOutput>(3).ToPagedList();
        var categoryId = _fixture.Create<int>();
        var parameters = new QueryStringParameters();
        _mockCategoriesService.GetProducts(Arg.Any<int>(), Arg.Any<QueryStringParameters>())
            .Returns(categoryProducts);

        // Act
        var result = await _categoriesController.GetCategoryProducts(categoryId, parameters);
        var obj = result.Result as ObjectResult;

        // Assert
        result.Result.Should().NotBeNull();
        result.Should().BeOfType<ActionResult<IEnumerable<ProductDTOOutput>>>();
        result.Result.Should().BeOfType<OkObjectResult>()
            .Which.StatusCode.Should().Be(200);
        obj.Value.Should().BeEquivalentTo(categoryProducts.ToList());
        
        var httpContext = _categoriesController.ControllerContext.HttpContext;
        httpContext.Response.Headers.ContainsKey("X-Pagination").Should().BeTrue();
        await _mockCategoriesService.Received(1).GetProducts(Arg.Any<int>(), Arg.Any<QueryStringParameters>());
    }

    [Fact]
    public async Task GetCategoryProductsByValue_ShouldReturnAllProductsByValueByCategoryId()
    {
        // Arrange
        var categoryProducts = _fixture.CreateMany<ProductDTOOutput>(3).ToPagedList();
        var categoryId = _fixture.Create<int>();
        var parameters = new ProductParameters();
        _mockCategoriesService.GetProductsByValue(Arg.Any<int>(), Arg.Any<ProductParameters>())
            .Returns(categoryProducts);
        
        // Act
        var result = await _categoriesController.GetCategoryProductsByValue(categoryId, parameters);
        var obj = result.Result as ObjectResult;

        // Assert
        result.Result.Should().NotBeNull();
        result.Should().BeOfType<ActionResult<IEnumerable<ProductDTOOutput>>>();
        result.Result.Should().BeOfType<OkObjectResult>()
            .Which.StatusCode.Should().Be(200);
        obj.Value.Should().BeEquivalentTo(categoryProducts.ToList());
        
        var httpContext = _categoriesController.ControllerContext.HttpContext;
        httpContext.Response.Headers.ContainsKey("X-Pagination").Should().BeTrue();
        await _mockCategoriesService.Received(1).GetProductsByValue(Arg.Any<int>(), Arg.Any<ProductParameters>());
    }

    [Fact]
    public async Task CreateCategory_ShouldReturn201CreatedWithCategory_WhenThereIsNoErrors()
    {
        // Arrange
        var categoryInput = _fixture.Create<CategoryDTOInput>();
        var category = _fixture.Create<CategoryDTOOutput>();
        var categoryResult = Result<CategoryDTOOutput>.Success(category);
        _mockCategoriesService.CreateCategory(Arg.Any<CategoryDTOInput>()).Returns(categoryResult);
        
        // Act
        var result = await _categoriesController.Post(categoryInput);
        var obj = result.Result as ObjectResult;
        
        // Assert
        result.Result.Should().NotBeNull();
        result.Should().BeOfType<ActionResult<CategoryDTOOutput>>();
        result.Result.Should().BeOfType<CreatedAtRouteResult>()
            .Which.StatusCode.Should().Be(201);
        obj.Value.Should().BeEquivalentTo(categoryResult.value);
        
        await _mockCategoriesService.Received(1).CreateCategory(Arg.Any<CategoryDTOInput>());
    }
    
    [Fact]
    public async Task CreateCategory_ShouldReturn400BadRequestWithErrorResponse_WhenThereIsErrors()
    {
        // Arrange
        var categoryInput = _fixture.Create<CategoryDTOInput>();
        var error = _fixture.Create<Error>();
        var categoryResult = Result<CategoryDTOOutput>.Failure(error);
        _mockCategoriesService.CreateCategory(Arg.Any<CategoryDTOInput>()).Returns(categoryResult);
        
        // Act
        var result = await _categoriesController.Post(categoryInput);
        var obj = result.Result as ObjectResult;
        
        // Assert
        result.Result.Should().NotBeNull();
        result.Should().BeOfType<ActionResult<CategoryDTOOutput>>();
        result.Result.Should().BeOfType<BadRequestObjectResult>()
            .Which.StatusCode.Should().Be(400);
        obj.Value.Should().BeEquivalentTo(categoryResult.GenerateErrorResponse());
        
        await _mockCategoriesService.Received(1).CreateCategory(Arg.Any<CategoryDTOInput>());
    }

    [Fact]
    public async Task PutCategory_ShouldReturn200OkResultWithUpdateSuccessResponse_WhenThereIsNoErrors()
    {
        // Arrange
        var categoryInput = _fixture.Create<CategoryDTOInput>();
        var categoryId = categoryInput.CategoryId;
        var category = _fixture.Create<CategoryDTOOutput>();
        var categoryResult = Result<CategoryDTOOutput>.Success(category);
        _mockCategoriesService.UpdateCategory(Arg.Any<CategoryDTOInput>(), Arg.Any<int>()).Returns(categoryResult);
        
        // Act
        var result = await _categoriesController.Put(categoryId, categoryInput);
        var obj = result.Result as ObjectResult;

        // Assert
        result.Result.Should().NotBeNull();
        result.Should().BeOfType<ActionResult<CategoryDTOOutput>>();
        result.Result.Should().BeOfType<OkObjectResult>()
            .Which.StatusCode.Should().Be(200);
        obj.Value.Should().BeEquivalentTo($"Category with id = {categoryResult.value.CategoryId} was updated successfully");
        
        await _mockCategoriesService.Received(1).UpdateCategory(Arg.Any<CategoryDTOInput>(), Arg.Any<int>());
    }
    
    [Fact]
    public async Task PutCategory_ShouldReturn404NotFoundResultWithErrorResponse_WhenThereIsErrors()
    {
        // Arrange
        var categoryInput = _fixture.Create<CategoryDTOInput>();
        var categoryId = categoryInput.CategoryId;
        var error = new Error("ErrorCode", "ErrorCode", HttpStatusCode.NotFound);
        var categoryResult = Result<CategoryDTOOutput>.Failure(error);
        _mockCategoriesService.UpdateCategory(Arg.Any<CategoryDTOInput>(), Arg.Any<int>()).Returns(categoryResult);
        
        // Act
        var result = await _categoriesController.Put(categoryId, categoryInput);
        var obj = result.Result as ObjectResult;

        // Assert
        result.Result.Should().NotBeNull();
        result.Should().BeOfType<ActionResult<CategoryDTOOutput>>();
        result.Result.Should().BeOfType<NotFoundObjectResult>()
            .Which.StatusCode.Should().Be(404);
        obj.Value.Should().BeEquivalentTo(categoryResult.GenerateErrorResponse());
        
        await _mockCategoriesService.Received(1).UpdateCategory(Arg.Any<CategoryDTOInput>(), Arg.Any<int>());
    }
    
    [Fact]
    public async Task PutCategory_ShouldReturn400BadRequestResultWithErrorResponse_WhenThereIsErrors()
    {
        // Arrange
        var categoryInput = _fixture.Create<CategoryDTOInput>();
        var categoryId = categoryInput.CategoryId;
        var error = new Error("ErrorCode", "ErrorCode", HttpStatusCode.BadRequest);
        var categoryResult = Result<CategoryDTOOutput>.Failure(error);
        _mockCategoriesService.UpdateCategory(Arg.Any<CategoryDTOInput>(), Arg.Any<int>()).Returns(categoryResult);
        
        // Act
        var result = await _categoriesController.Put(categoryId, categoryInput);
        var obj = result.Result as ObjectResult;

        // Assert
        result.Result.Should().NotBeNull();
        result.Should().BeOfType<ActionResult<CategoryDTOOutput>>();
        result.Result.Should().BeOfType<BadRequestObjectResult>()
            .Which.StatusCode.Should().Be(400);
        obj.Value.Should().BeEquivalentTo(categoryResult.GenerateErrorResponse());
        
        await _mockCategoriesService.Received(1).UpdateCategory(Arg.Any<CategoryDTOInput>(), Arg.Any<int>());
    }

    [Fact]
    public async Task DeleteCategory_ShouldReturn200OkResultWithDeleteSuccessResponse_WhenThereIsNoErrors()
    {
        // Arrange
        var categoryId = _fixture.Create<int>();
        var category = _fixture.Create<CategoryDTOOutput>();
        var categoryResult = Result<CategoryDTOOutput>.Success(category);
        _mockCategoriesService.DeleteCategory(Arg.Any<int>()).Returns(categoryResult);
        
        // Act
        var result = await _categoriesController.Delete(categoryId);
        var obj = result.Result as ObjectResult;
        
        // Assert
        result.Result.Should().NotBeNull();
        result.Should().BeOfType<ActionResult<CategoryDTOOutput>>();
        result.Result.Should().BeOfType<OkObjectResult>()
            .Which.StatusCode.Should().Be(200);
        obj.Value.Should().BeEquivalentTo($"Category with id = {categoryResult.value.CategoryId} was deleted successfully");
        
        await _mockCategoriesService.Received(1).DeleteCategory(Arg.Any<int>());
    }
    
    [Fact]
    public async Task DeleteCategory_ShouldReturn404NotFoundResultWithErrorResponse_WhenThereIsErrors()
    {
        // Arrange
        var categoryId = _fixture.Create<int>();
        var error = _fixture.Create<Error>();
        var categoryResult = Result<CategoryDTOOutput>.Failure(error);
        _mockCategoriesService.DeleteCategory(Arg.Any<int>()).Returns(categoryResult);
        
        // Act
        var result = await _categoriesController.Delete(categoryId);
        var obj = result.Result as ObjectResult;
        
        // Assert
        result.Result.Should().NotBeNull();
        result.Should().BeOfType<ActionResult<CategoryDTOOutput>>();
        result.Result.Should().BeOfType<NotFoundObjectResult>()
            .Which.StatusCode.Should().Be(404);
        obj.Value.Should().BeEquivalentTo(categoryResult.GenerateErrorResponse());
        
        await _mockCategoriesService.Received(1).DeleteCategory(Arg.Any<int>());
    }
}