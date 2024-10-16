using System.Linq.Expressions;
using AutoFixture;
using AutoMapper;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using NSubstitute;
using Sales.Application.DTOs.ProductDTO;
using Sales.Application.Interfaces;
using Sales.Application.Parameters;
using Sales.Application.Parameters.ModelsParameters;
using Sales.Application.ResultPattern;
using Sales.Application.Services;
using Sales.Domain.Interfaces;
using Sales.Domain.Models;
using X.PagedList;
using X.PagedList.Extensions;

namespace Sales.Test.ServicesTests;

public class ProductServiceTest
{
    private readonly ProductService _productService;
    private readonly IUnitOfWork _mockUnitOfWork;
    private readonly IValidator<ProductDTOInput> _mockProductValidator;
    private readonly IMapper _mockMapper;
    private readonly IProductFilterFactory _mockProductFilterFactory;
    private readonly Fixture _fixture;

    public ProductServiceTest()
    {
        _mockMapper = Substitute.For<IMapper>();
        _mockProductValidator = Substitute.For<IValidator<ProductDTOInput>>();
        _mockUnitOfWork = Substitute.For<IUnitOfWork>();
        _mockProductFilterFactory = Substitute.For<IProductFilterFactory>();

        _fixture = new Fixture();
        
        _productService = new ProductService(
            _mockUnitOfWork,
            _mockProductValidator,
            _mockMapper,
            _mockProductFilterFactory);
    }

    [Fact]
    public async Task GetAllProducts_ShouldReturnAllProducts()
    {
        // Arrange
        var products = _fixture.CreateMany<Product>(3).ToList();
        var productsDto = _fixture.CreateMany<ProductDTOOutput>(3).ToList();
        
        _mockUnitOfWork.ProductRepository.GetAllAsync().Returns(products);
        _mockMapper.Map<IEnumerable<ProductDTOOutput>>(Arg.Any<IEnumerable<Product>>()).Returns(productsDto);

        // Act
        var result = await _productService.GetAllProducts();

        // Assert
        result.Should().BeOfType<List<ProductDTOOutput>>();
        result.Should().BeEquivalentTo(productsDto);
        
        await _mockUnitOfWork.ProductRepository.Received(1).GetAllAsync();
        _mockMapper.Received(1).Map<IEnumerable<ProductDTOOutput>>(Arg.Any<IEnumerable<Product>>());
    }
    
    [Fact]
    public async Task GetAllProductsPaged_ShouldReturnAllProductsPaged()
    {
        // Arrange
        var parameters = new QueryStringParameters();
        var products = _fixture.CreateMany<Product>(3).ToList();
        var productsDto = _fixture.CreateMany<ProductDTOOutput>(3).ToList();
        
        _mockUnitOfWork.ProductRepository.GetAllAsync().Returns(products);
        _mockMapper.Map<IEnumerable<ProductDTOOutput>>(Arg.Any<IEnumerable<Product>>()).Returns(productsDto);

        // Act
        var result = await _productService.GetAllProducts(parameters);

        // Assert
        result.Should().BeEquivalentTo(productsDto.ToPagedList(parameters.PageNumber, parameters.PageSize));
        
        await _mockUnitOfWork.ProductRepository.Received(1).GetAllAsync();
        _mockMapper.Received(1).Map<IEnumerable<ProductDTOOutput>>(Arg.Any<IEnumerable<Product>>());
    }
    
    [Fact]
    public async Task GetProductsWithFilter_ShouldReturnAllProductsPagedThatMatchesSomeFilter()
    {
        // Arrange
        var filter = "filter";
        var parameters = new ProductParameters();
        var products = _fixture.CreateMany<Product>(3).ToList();
        var productsDto = _fixture.CreateMany<ProductDTOOutput>(3).ToList();
        
        _mockUnitOfWork.ProductRepository.GetAllAsync().Returns(products);
        _mockMapper.Map<IEnumerable<ProductDTOOutput>>(Arg.Any<IEnumerable<Product>>()).Returns(productsDto);
        _mockProductFilterFactory.GetStrategy(Arg.Any<string>())
            .ApplyFilter(Arg.Any<IEnumerable<ProductDTOOutput>>(), Arg.Any<ProductParameters>())
            .Returns(productsDto);

        // Act
        var result = await _productService.GetProductsWithFilter(filter, parameters);

        // Assert
        result.Should().BeEquivalentTo(productsDto.ToPagedList(parameters.PageNumber, parameters.PageSize));
        
        await _mockUnitOfWork.ProductRepository.Received(1).GetAllAsync();
        _mockMapper.Received(1).Map<IEnumerable<ProductDTOOutput>>(Arg.Any<IEnumerable<Product>>());
        _mockProductFilterFactory.Received(1).GetStrategy(Arg.Any<string>())
            .ApplyFilter(Arg.Any<IEnumerable<ProductDTOOutput>>(), Arg.Any<ProductParameters>());
    }
    
    [Fact]
    public async Task GetProductBy_ShouldReturnProductThatMatchesExpression()
    {
        // Arrange
        var productId = _fixture.Create<int>();
        var products = _fixture.Create<Product>();
        var productsDto = _fixture.Create<ProductDTOOutput>();
        
        _mockUnitOfWork.ProductRepository.GetAsync(Arg.Any<Expression<Func<Product, bool>>>()).Returns(products);
        _mockMapper.Map<ProductDTOOutput>(Arg.Any<Product>()).Returns(productsDto);

        // Act
        var result = await _productService.GetProductBy(p => p.ProductId == productId);

        // Assert
        result.Should().BeEquivalentTo(Result<ProductDTOOutput>.Success(productsDto));
        result.value.Should().BeEquivalentTo(productsDto);
        
        await _mockUnitOfWork.ProductRepository.Received(1).GetAsync(Arg.Any<Expression<Func<Product, bool>>>());
        _mockMapper.Received(1).Map<ProductDTOOutput>(Arg.Any<Product>());
    }
    
    [Fact]
    public async Task GetProductBy_ShouldReturnNotFoundError_WhenNoProductMatchesTheExpression()
    {
        // Arrange
        var productId = _fixture.Create<int>();
        Product products = null;
        
        _mockUnitOfWork.ProductRepository.GetAsync(Arg.Any<Expression<Func<Product, bool>>>()).Returns(products);

        // Act
        var result = await _productService.GetProductBy(p => p.ProductId == productId);

        // Assert
        result.Should().BeEquivalentTo(Result<ProductDTOOutput>.Failure(ProductErrors.NotFound));
        result.error.Should().BeEquivalentTo(ProductErrors.NotFound);
        
        await _mockUnitOfWork.ProductRepository.Received(1).GetAsync(Arg.Any<Expression<Func<Product, bool>>>());
    }

    [Fact]
    public async Task CreateProduct_ShouldReturnCreatedProduct()
    {
        // Arrange
        var productInput = _fixture.Create<ProductDTOInput>();
        var validationResult = new ValidationResult()
        {
            Errors = new List<ValidationFailure>()
        };
        Product productExists = null;
        var product = _fixture.Create<Product>();
        var productOutput = _fixture.Create<ProductDTOOutput>();
        
        _mockProductValidator.ValidateAsync(Arg.Any<ProductDTOInput>()).Returns(validationResult);
        _mockUnitOfWork.ProductRepository.GetAsync(Arg.Any<Expression<Func<Product, bool>>>()).Returns(productExists);
        _mockMapper.Map<Product>(Arg.Any<ProductDTOInput>()).Returns(product);
        _mockUnitOfWork.ProductRepository.Create(Arg.Any<Product>()).Returns(product);
        _mockMapper.Map<ProductDTOOutput>(Arg.Any<Product>()).Returns(productOutput);
        
        // Act
        var result = await _productService.CreateProduct(productInput);
        
        // Assert
        result.isSuccess.Should().BeTrue();
        result.value.Should().BeEquivalentTo(productOutput);

        await _mockProductValidator.Received(1).ValidateAsync(Arg.Any<ProductDTOInput>());
        await _mockUnitOfWork.ProductRepository.Received(1).GetAsync(Arg.Any<Expression<Func<Product, bool>>>());
        _mockMapper.Received(1).Map<Product>(Arg.Any<ProductDTOInput>());
        _mockUnitOfWork.ProductRepository.Received(1).Create(Arg.Any<Product>());
        _mockMapper.Received(1).Map<ProductDTOOutput>(Arg.Any<Product>());
    }
    
    [Fact]
    public async Task CreateProduct_ShouldReturnBadRequestWithDuplicateDataResult_WhenProductAlreadyExists()
    {
        // Arrange
        var productInput = _fixture.Create<ProductDTOInput>();
        var validationResult = new ValidationResult()
        {
            Errors = new List<ValidationFailure>()
        };
        var productExists = _fixture.Create<Product>();
        
        _mockProductValidator.ValidateAsync(Arg.Any<ProductDTOInput>()).Returns(validationResult);
        _mockUnitOfWork.ProductRepository.GetAsync(Arg.Any<Expression<Func<Product, bool>>>()).Returns(productExists);
        
        // Act
        var result = await _productService.CreateProduct(productInput);
        
        // Assert
        result.isSuccess.Should().BeFalse();
        result.error.Should().BeEquivalentTo(ProductErrors.DuplicateData);

        await _mockProductValidator.Received(1).ValidateAsync(Arg.Any<ProductDTOInput>());
        await _mockUnitOfWork.ProductRepository.Received(1).GetAsync(Arg.Any<Expression<Func<Product, bool>>>());
    }
    
    [Fact]
    public async Task CreateProduct_ShouldReturnBadRequestWithIncorrectFormatDataResult_WhenProductInputDataIsInvalid()
    {
        // Arrange
        var productInput = _fixture.Create<ProductDTOInput>();
        var validationResult = new ValidationResult()
        {
            Errors = new List<ValidationFailure>()
            {
                new ValidationFailure("InvalidDataInput", "InvalidDataInput")
            }
        };
        
        _mockProductValidator.ValidateAsync(Arg.Any<ProductDTOInput>()).Returns(validationResult);
        
        // Act
        var result = await _productService.CreateProduct(productInput);
        
        // Assert
        result.isSuccess.Should().BeFalse();
        result.error.Should().BeEquivalentTo(ProductErrors.IncorrectFormatData);

        await _mockProductValidator.Received(1).ValidateAsync(Arg.Any<ProductDTOInput>());
    }
    
    [Fact]
    public async Task CreateProduct_ShouldReturnBadRequestWithDataIsNullResult_WhenProductInputDataIsNull()
    {
        // Arrange
        ProductDTOInput productInput = null;
        
        // Act
        var result = await _productService.CreateProduct(productInput);
        
        // Assert
        result.isSuccess.Should().BeFalse();
        result.error.Should().BeEquivalentTo(ProductErrors.DataIsNull);
    }
    
    [Fact]
    public async Task UpdateProduct_ShouldReturnUpdatedProduct()
    {
        // Arrange
        var productInput = _fixture.Create<ProductDTOInput>();
        var productId = productInput.ProductId;
        var product = _fixture.Create<Product>();
        var validationResult = new ValidationResult()
        {
            Errors = new List<ValidationFailure>()
        };
        var productForUpdate = _fixture.Create<Product>();
        var productDtoUpdated = _fixture.Create<ProductDTOOutput>();
        
        _mockUnitOfWork.ProductRepository.GetAsync(Arg.Any<Expression<Func<Product, bool>>>())
            .Returns(product);
        _mockProductValidator.ValidateAsync(Arg.Any<ProductDTOInput>()).Returns(validationResult);
        _mockMapper.Map<Product>(Arg.Any<ProductDTOInput>()).Returns(productForUpdate);
        _mockUnitOfWork.ProductRepository.Update(Arg.Any<Product>()).Returns(productForUpdate);
        _mockMapper.Map<ProductDTOOutput>(Arg.Any<Product>()).Returns(productDtoUpdated);
        
        // Act
        var result = await _productService.UpdateProduct(productInput, productId);
        
        // Assert
        result.isSuccess.Should().BeTrue();
        result.value.Should().BeEquivalentTo(productDtoUpdated);
        
        await _mockUnitOfWork.ProductRepository.Received(1).GetAsync(Arg.Any<Expression<Func<Product, bool>>>());
        await _mockProductValidator.Received(1).ValidateAsync(Arg.Any<ProductDTOInput>());
        _mockMapper.Received(1).Map<Product>(Arg.Any<ProductDTOInput>());
        _mockUnitOfWork.ProductRepository.Received(1).Update(Arg.Any<Product>());
        _mockMapper.Received(1).Map<ProductDTOOutput>(Arg.Any<Product>());
    }
    
    [Fact]
    public async Task UpdateProduct_ShouldReturnBadRequestWithIncorretFormatDataResult_WhenProductInputDataIsInvalid()
    {
        // Arrange
        var productInput = _fixture.Create<ProductDTOInput>();
        var productId = productInput.ProductId;
        var product = _fixture.Create<Product>();
        var validationResult = new ValidationResult()
        {
            Errors = new List<ValidationFailure>()
            {
                new ValidationFailure("InvalidDataInput", "InvalidDataInput")
            }
        };
        
        _mockUnitOfWork.ProductRepository.GetAsync(Arg.Any<Expression<Func<Product, bool>>>())
            .Returns(product);
        _mockProductValidator.ValidateAsync(Arg.Any<ProductDTOInput>()).Returns(validationResult);
        
        // Act
        var result = await _productService.UpdateProduct(productInput, productId);
        
        // Assert
        result.isSuccess.Should().BeFalse();
        result.error.Should().BeEquivalentTo(ProductErrors.IncorrectFormatData);
        result.validationFailures.Should().NotBeEmpty();
        
        await _mockUnitOfWork.ProductRepository.Received(1).GetAsync(Arg.Any<Expression<Func<Product, bool>>>());
        await _mockProductValidator.Received(1).ValidateAsync(Arg.Any<ProductDTOInput>());
    }
    
    [Fact]
    public async Task UpdateProduct_ShouldReturnNotFoundWithNotFoundResult_WhenProductDoesNotExist()
    {
        // Arrange
        var productInput = _fixture.Create<ProductDTOInput>();
        var productId = productInput.ProductId;
        Product product = null;
        
        _mockUnitOfWork.ProductRepository.GetAsync(Arg.Any<Expression<Func<Product, bool>>>())
            .Returns(product);
        
        // Act
        var result = await _productService.UpdateProduct(productInput, productId);
        
        // Assert
        result.isSuccess.Should().BeFalse();
        result.error.Should().BeEquivalentTo(ProductErrors.NotFound);
        
        await _mockUnitOfWork.ProductRepository.Received(1).GetAsync(Arg.Any<Expression<Func<Product, bool>>>());
    }
    
    [Fact]
    public async Task UpdateProduct_ShouldReturnBadRequestWithIdMismatchResult_WhenProductAndProductIdDoesNotMatch()
    {
        // Arrange
        var productInput = _fixture.Create<ProductDTOInput>();
        var productId = _fixture.Create<int>();
        
        // Act
        var result = await _productService.UpdateProduct(productInput, productId);
        
        // Assert
        result.isSuccess.Should().BeFalse();
        result.error.Should().BeEquivalentTo(ProductErrors.IdMismatch);
    }
    
    [Fact]
    public async Task UpdateProduct_ShouldReturnBadRequestWithDataInputIsNullResult_WhenProductInputIsNull()
    {
        // Arrange
        ProductDTOInput productInput = null;
        var productId = _fixture.Create<int>();
        
        // Act
        var result = await _productService.UpdateProduct(productInput, productId);
        
        // Assert
        result.isSuccess.Should().BeFalse();
        result.error.Should().BeEquivalentTo(ProductErrors.DataIsNull);
    }

    [Fact]
    public async Task DeleteProduct_ShouldReturnDeletedProduct()
    {
        // Arrange
        var productId = _fixture.Create<int>();
        var product = _fixture.Create<Product>();
        var productDtoDeleted = _fixture.Create<ProductDTOOutput>();
        
        _mockUnitOfWork.ProductRepository.GetAsync(Arg.Any<Expression<Func<Product, bool>>>())
            .Returns(product);
        _mockUnitOfWork.ProductRepository.Delete(Arg.Any<Product>()).Returns(product);
        _mockMapper.Map<ProductDTOOutput>(Arg.Any<Product>()).Returns(productDtoDeleted);
        
        // Act
        var result = await _productService.DeleteProduct(productId);
        
        // Assert
        result.isSuccess.Should().BeTrue();
        result.value.Should().BeEquivalentTo(productDtoDeleted);
        
        await _mockUnitOfWork.ProductRepository.Received(1).GetAsync(Arg.Any<Expression<Func<Product, bool>>>());
        _mockUnitOfWork.ProductRepository.Received(1).Delete(Arg.Any<Product>());
        _mockMapper.Received(1).Map<ProductDTOOutput>(Arg.Any<Product>());
    }
    
    [Fact]
    public async Task DeleteProduct_ShouldReturnNotFoundWithNotFoundResponse_WhenProductDoesNotExist()
    {
        // Arrange
        var productId = _fixture.Create<int>();
        Product product = null;
        
        _mockUnitOfWork.ProductRepository.GetAsync(Arg.Any<Expression<Func<Product, bool>>>())
            .Returns(product);
        
        // Act
        var result = await _productService.DeleteProduct(productId);
        
        // Assert
        result.isSuccess.Should().BeFalse();
        result.error.Should().BeEquivalentTo(ProductErrors.NotFound);
        
        await _mockUnitOfWork.ProductRepository.Received(1).GetAsync(Arg.Any<Expression<Func<Product, bool>>>());
    }
}