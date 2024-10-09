using System.Linq.Expressions;
using System.Net;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Sales.API.Controllers;
using Sales.Application.DTOs.ProductDTO;
using Sales.Application.Interfaces;
using Sales.Application.Parameters;
using Sales.Application.Parameters.ModelsParameters;
using Sales.Application.ResultPattern;
using Sales.Domain.Models;
using X.PagedList.Extensions;

namespace Sales.Test.ControllersTests;

public class ProductsControllerTest
{
    private readonly ProductsController _productsController;
    private readonly IProductService _mockProductService;
    private readonly Fixture _fixture;

    public ProductsControllerTest()
    {
        _mockProductService = Substitute.For<IProductService>();
        _fixture = new Fixture();
        
        _productsController = new ProductsController(_mockProductService)
        {
            ControllerContext =
            {
                HttpContext = new DefaultHttpContext()
            }
        };
    }

    [Fact]
    public async Task Get_ShouldReturn200OkResultWithAllProducts()
    {
        // Arrange
        var products = _fixture.CreateMany<ProductDTOOutput>(3).ToPagedList();
        var parameters = new QueryStringParameters();
        _mockProductService.GetAllProducts(Arg.Any<QueryStringParameters>()).Returns(products);
        
        // Act
        var result = await _productsController.Get(parameters);
        var obj = result.Result as ObjectResult;

        // Assert
        result.Result.Should().NotBeNull();
        result.Should().BeOfType<ActionResult<IEnumerable<ProductDTOOutput>>>();
        result.Result.Should().BeOfType<OkObjectResult>()
            .Which.StatusCode.Should().Be(200);
        obj.Value.Should().BeEquivalentTo(products.ToList());

        var httpContext = _productsController.ControllerContext.HttpContext;
        httpContext.Response.Headers.Should().ContainKey("X-Pagination");
        await _mockProductService.Received(1).GetAllProducts(Arg.Any<QueryStringParameters>());
    }

    [Fact]
    public async Task GetProductsByValue_ShouldReturnAll200OkResultWithProductsThatMatcherValueAndValueCriteria()
    {
        // Arrange
        var products = _fixture.CreateMany<ProductDTOOutput>(3).ToPagedList();
        var parameters = new ProductParameters();
        _mockProductService.GetProductsWithFilter("value", Arg.Any<ProductParameters>())
            .Returns(products);

        // Act
        var result = await _productsController.GetProductsByValue(parameters);
        var obj = result.Result as ObjectResult;

        // Assert
        result.Result.Should().NotBeNull();
        result.Should().BeOfType<ActionResult<IEnumerable<ProductDTOOutput>>>();
        result.Result.Should().BeOfType<OkObjectResult>()
            .Which.StatusCode.Should().Be(200);
        obj.Value.Should().BeEquivalentTo(products);
        
        var httpContext = _productsController.ControllerContext.HttpContext;
        httpContext.Response.Headers.ContainsKey("X-Pagination").Should().BeTrue();
        await _mockProductService.Received(1).GetProductsWithFilter("value", Arg.Any<ProductParameters>());
    }

    [Fact]
    public async Task GetProductsByTypeValue_ShouldReturn200OkResultWithAllProductsThatMatcherTypeValue()
    {
        // Arrange
        var products = _fixture.CreateMany<ProductDTOOutput>(3).ToPagedList();
        var parameters = new ProductParameters();
        _mockProductService.GetProductsWithFilter("typevalue", Arg.Any<ProductParameters>())
            .Returns(products);

        // Act
        var result = await _productsController.GetProductsByTypeValue(parameters);
        var obj = result.Result as ObjectResult;

        // Assert
        result.Result.Should().NotBeNull();
        result.Should().BeOfType<ActionResult<IEnumerable<ProductDTOOutput>>>();
        result.Result.Should().BeOfType<OkObjectResult>()
            .Which.StatusCode.Should().Be(200);
        obj.Value.Should().BeEquivalentTo(products);
        
        var httpContext = _productsController.ControllerContext.HttpContext;
        httpContext.Response.Headers.ContainsKey("X-Pagination").Should().BeTrue();
        await _mockProductService.Received(1).GetProductsWithFilter("typevalue", Arg.Any<ProductParameters>());
    }

    [Fact]
    public async Task GetProductsByName_ShouldReturn200OkResultWithAllProductsThatMatcherName()
    {
        // Arrange
        var products = _fixture.CreateMany<ProductDTOOutput>(3).ToPagedList();
        var parameters = new ProductParameters();
        _mockProductService.GetProductsWithFilter("name", Arg.Any<ProductParameters>())
            .Returns(products);

        // Act
        var result = await _productsController.GetProductsByName(parameters);
        var obj = result.Result as ObjectResult;

        // Assert
        result.Result.Should().NotBeNull();
        result.Should().BeOfType<ActionResult<IEnumerable<ProductDTOOutput>>>();
        result.Result.Should().BeOfType<OkObjectResult>()
            .Which.StatusCode.Should().Be(200);
        obj.Value.Should().BeEquivalentTo(products);
        
        var httpContext = _productsController.ControllerContext.HttpContext;
        httpContext.Response.Headers.ContainsKey("X-Pagination").Should().BeTrue();
        await _mockProductService.Received(1).GetProductsWithFilter("name", Arg.Any<ProductParameters>());
    }

    [Fact]
    public async Task GetProduct_ShouldReturn200OkResultWithProductById_WhenProductExists()
    {
        // Arrange
        var products = _fixture.Create<ProductDTOOutput>();
        var productId = products.ProductId;
        var productResult = Result<ProductDTOOutput>.Success(products);
        _mockProductService.GetProductBy(Arg.Any<Expression<Func<Product, bool>>>())
            .Returns(productResult);
        
        // Act
        var result = await _productsController.Get(productId);
        var obj = result.Result as ObjectResult;

        // Assert
        result.Result.Should().NotBeNull();
        result.Should().BeOfType<ActionResult<ProductDTOOutput>>();
        result.Result.Should().BeOfType<OkObjectResult>()
            .Which.StatusCode.Should().Be(200);
        obj.Value.Should().BeEquivalentTo(productResult.value);

        await _mockProductService.Received(1).GetProductBy(Arg.Any<Expression<Func<Product, bool>>>());
    }
    
    [Fact]
    public async Task GetProduct_ShouldReturn404NotFoundResultWithErrorResponse_WhenProductDoesNotExists()
    {
        // Arrange
        var error = _fixture.Create<Error>();
        var productId = _fixture.Create<int>();
        var productResult = Result<ProductDTOOutput>.Failure(error);
        _mockProductService.GetProductBy(Arg.Any<Expression<Func<Product, bool>>>())
            .Returns(productResult);
        
        // Act
        var result = await _productsController.Get(productId);
        var obj = result.Result as ObjectResult;

        // Assert
        result.Result.Should().NotBeNull();
        result.Should().BeOfType<ActionResult<ProductDTOOutput>>();
        result.Result.Should().BeOfType<NotFoundObjectResult>()
            .Which.StatusCode.Should().Be(404);
        obj.Value.Should().BeEquivalentTo(productResult.GenerateErrorResponse());

        await _mockProductService.Received(1).GetProductBy(Arg.Any<Expression<Func<Product, bool>>>());
    }

    [Fact]
    public async Task Post_ShouldReturn201CreatedAtRouteResultWithProduct_WhenProductIsCreatedSuccessfully()
    {
        // Arrange
        var productInput = _fixture.Create<ProductDTOInput>();
        var product = _fixture.Create<ProductDTOOutput>();
        var productResult = Result<ProductDTOOutput>.Success(product);
        _mockProductService.CreateProduct(Arg.Any<ProductDTOInput>()).Returns(productResult);

        // Act
        var result = await _productsController.Post(productInput);
        var obj = result.Result as ObjectResult;

        // Assert
        result.Result.Should().NotBeNull();
        result.Should().BeOfType<ActionResult<ProductDTOOutput>>();
        result.Result.Should().BeOfType<CreatedAtRouteResult>()
            .Which.StatusCode.Should().Be(201);
        obj.Value.Should().BeEquivalentTo(productResult.value);
        
        await _mockProductService.Received(1).CreateProduct(Arg.Any<ProductDTOInput>());
    }
    
    [Fact]
    public async Task Post_ShouldReturn400BadRequestResultWithProduct_WhenProductIsNotCreatedSuccessfully()
    {
        // Arrange
        var productInput = _fixture.Create<ProductDTOInput>();
        var error = _fixture.Create<Error>();
        var productResult = Result<ProductDTOOutput>.Failure(error);
        _mockProductService.CreateProduct(Arg.Any<ProductDTOInput>()).Returns(productResult);

        // Act
        var result = await _productsController.Post(productInput);
        var obj = result.Result as ObjectResult;

        // Assert
        result.Result.Should().NotBeNull();
        result.Should().BeOfType<ActionResult<ProductDTOOutput>>();
        result.Result.Should().BeOfType<BadRequestObjectResult>()
            .Which.StatusCode.Should().Be(400);
        obj.Value.Should().BeEquivalentTo(productResult.GenerateErrorResponse());
        
        await _mockProductService.Received(1).CreateProduct(Arg.Any<ProductDTOInput>());
    }

    [Fact]
    public async Task Put_ShouldReturn200OkResultWithProductResponse_WhenProductIsUpdatedSuccessfully()
    {
        // Arrange
        var productInput = _fixture.Create<ProductDTOInput>();
        var productId = productInput.ProductId;
        var product = _fixture.Create<ProductDTOOutput>();
        var productResponse = Result<ProductDTOOutput>.Success(product);
        _mockProductService.UpdateProduct(Arg.Any<ProductDTOInput>(), Arg.Any<int>()).Returns(productResponse);

        // Act
        var result = await _productsController.Put(productId, productInput);
        var obj = result.Result as ObjectResult;

        // Assert
        result.Result.Should().NotBeNull();
        result.Should().BeOfType<ActionResult<ProductDTOOutput>>();
        result.Result.Should().BeOfType<OkObjectResult>()
            .Which.StatusCode.Should().Be(200);
        obj.Value.Should().BeEquivalentTo($"Product with id = {productResponse.value.ProductId} was updated successfully");
        
        await _mockProductService.Received(1).UpdateProduct(Arg.Any<ProductDTOInput>(), Arg.Any<int>());
    }
    
    [Fact]
    public async Task Put_ShouldReturn404NotFoundResultWithErrorResponse_WhenProductDoesNotUpdatedSuccessfully()
    {
        // Arrange
        var productInput = _fixture.Create<ProductDTOInput>();
        var productId = productInput.ProductId;
        var error = new Error("NotFound", "NotFound", HttpStatusCode.NotFound);
        var productResponse = Result<ProductDTOOutput>.Failure(error);
        _mockProductService.UpdateProduct(Arg.Any<ProductDTOInput>(), Arg.Any<int>()).Returns(productResponse);

        // Act
        var result = await _productsController.Put(productId, productInput);
        var obj = result.Result as ObjectResult;

        // Assert
        result.Result.Should().NotBeNull();
        result.Should().BeOfType<ActionResult<ProductDTOOutput>>();
        result.Result.Should().BeOfType<NotFoundObjectResult>()
            .Which.StatusCode.Should().Be(404);
        obj.Value.Should().BeEquivalentTo(productResponse.error.Description);
        
        await _mockProductService.Received(1).UpdateProduct(Arg.Any<ProductDTOInput>(), Arg.Any<int>());
    }
    
    [Fact]
    public async Task Put_ShouldReturn400BadRequestResultWithErrorResponse_WhenProductDoesNotUpdatedSuccessfully()
    {
        // Arrange
        var productInput = _fixture.Create<ProductDTOInput>();
        var productId = productInput.ProductId;
        var error = new Error("BadRequest", "BadRequest", HttpStatusCode.BadRequest);
        var productResponse = Result<ProductDTOOutput>.Failure(error);
        _mockProductService.UpdateProduct(Arg.Any<ProductDTOInput>(), Arg.Any<int>()).Returns(productResponse);

        // Act
        var result = await _productsController.Put(productId, productInput);
        var obj = result.Result as ObjectResult;

        // Assert
        result.Result.Should().NotBeNull();
        result.Should().BeOfType<ActionResult<ProductDTOOutput>>();
        result.Result.Should().BeOfType<BadRequestObjectResult>()
            .Which.StatusCode.Should().Be(400);
        obj.Value.Should().BeEquivalentTo(productResponse.GenerateErrorResponse());
        
        await _mockProductService.Received(1).UpdateProduct(Arg.Any<ProductDTOInput>(), Arg.Any<int>());
    }

    [Fact]
    public async Task Delete_ShouldReturn200OkResultWithProductResponse_WhenProductIsDeletedSuccessfully()
    {
        // Arrange
        var productId = _fixture.Create<int>();
        var product = _fixture.Create<ProductDTOOutput>();
        var productResponse = Result<ProductDTOOutput>.Success(product);
        _mockProductService.DeleteProduct(Arg.Any<int>()).Returns(productResponse);
        
        // Act
        var result = await _productsController.Delete(productId);
        var obj = result.Result as ObjectResult;
        
        // Assert
        result.Result.Should().NotBeNull();
        result.Should().BeOfType<ActionResult<ProductDTOOutput>>();
        result.Result.Should().BeOfType<OkObjectResult>()
            .Which.StatusCode.Should().Be(200);
        obj.Value.Should().BeEquivalentTo($"Category with id = {productResponse.value.ProductId} was deleted successfully");
        
        await _mockProductService.Received(1).DeleteProduct(Arg.Any<int>());
    }
    
    [Fact]
    public async Task Delete_ShouldReturn404NotFoundResultWithErrorResponse_WhenProductIsDeletedSuccessfully()
    {
        // Arrange
        var productId = _fixture.Create<int>();
        var error = _fixture.Create<Error>();
        var productResponse = Result<ProductDTOOutput>.Failure(error);
        _mockProductService.DeleteProduct(Arg.Any<int>()).Returns(productResponse);
        
        // Act
        var result = await _productsController.Delete(productId);
        var obj = result.Result as ObjectResult;
        
        // Assert
        result.Result.Should().NotBeNull();
        result.Should().BeOfType<ActionResult<ProductDTOOutput>>();
        result.Result.Should().BeOfType<NotFoundObjectResult>()
            .Which.StatusCode.Should().Be(404);
        obj.Value.Should().BeEquivalentTo(productResponse.GenerateErrorResponse());
        
        await _mockProductService.Received(1).DeleteProduct(Arg.Any<int>());
    }
}