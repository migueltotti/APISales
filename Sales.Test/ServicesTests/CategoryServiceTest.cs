using System.Linq.Expressions;
using System.Net;
using AutoFixture;
using AutoMapper;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using NSubstitute;
using Sales.Application.DTOs.CategoryDTO;
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

public class CategoryServiceTest
{
    private readonly CategoryService _categoryService;
    private readonly IUnitOfWork _mockUof;
    private readonly IValidator<CategoryDTOInput> _mockValidator;
    private readonly IMapper _mockMapper;
    private readonly ICategoryFilterFactory _mockCategoryFilterFactory;
    private readonly Fixture _fixture;

    public CategoryServiceTest()
    {
        _mockUof = Substitute.For<IUnitOfWork>();
        _mockValidator = Substitute.For<IValidator<CategoryDTOInput>>();
        _mockMapper = Substitute.For<IMapper>();
        _mockCategoryFilterFactory = Substitute.For<ICategoryFilterFactory>();
        _fixture = new Fixture();
        _categoryService = new CategoryService(
            _mockUof,
            _mockValidator,
            _mockMapper,
            _mockCategoryFilterFactory);
    }

    [Fact]
    public async Task GetAllCategories_ShouldReturnAllCategories()
    {
        // Arrange
        var categories = _fixture.CreateMany<Category>(3);
        var categoriesDto = _fixture.CreateMany<CategoryDTOOutput>(3);
        _mockUof.CategoryRepository.GetAllAsync().Returns(categories);
        _mockMapper.Map<IEnumerable<CategoryDTOOutput>>(Arg.Any<IEnumerable<Category>>())
            .Returns(categoriesDto);
        
        // Act
        var result = await _categoryService.GetAllCategories();
        
        // Assert
        result.Should().BeEquivalentTo(categoriesDto);
        
        await _mockUof.CategoryRepository.Received(1).GetAllAsync();
        _mockMapper.Received(1).Map<IEnumerable<CategoryDTOOutput>>(Arg.Any<IEnumerable<Category>>());
    }
    
    [Fact]
    public async Task GetAllCategoriesPaged_ShouldReturnAllCategoriesPaged()
    {
        // Arrange
        var parameters = new QueryStringParameters();
        var categories = _fixture.CreateMany<Category>(3);
        var categoriesDto = _fixture.CreateMany<CategoryDTOOutput>(3);

        _mockUof.CategoryRepository.GetAllAsync().Returns(categories);
        _mockMapper.Map<IEnumerable<CategoryDTOOutput>>(Arg.Any<IEnumerable<Category>>())
            .Returns(categoriesDto);
        
        // Act
        var result = await _categoryService.GetAllCategories(parameters);
        
        // Assert
        result.Should().BeEquivalentTo(categoriesDto.ToPagedList(parameters.PageNumber, parameters.PageSize));
        await _mockUof.CategoryRepository.Received(1).GetAllAsync();
        _mockMapper.Received(1).Map<IEnumerable<CategoryDTOOutput>>(Arg.Any<IEnumerable<Category>>());
    }
    
    [Fact]
    public async Task GetCategoriesWithFilter_ShouldReturnAllCategoriesPagedWithFilter()
    {
        // Arrange
        var filter = "filter";
        var parameters = new CategoryParameters();
        var categories = _fixture.CreateMany<Category>(3);
        var categoriesDto = _fixture.CreateMany<CategoryDTOOutput>(3);

        _mockUof.CategoryRepository.GetAllAsync().Returns(categories);
        _mockMapper.Map<IEnumerable<CategoryDTOOutput>>(Arg.Any<IEnumerable<Category>>())
            .Returns(categoriesDto);
        _mockCategoryFilterFactory.GetStrategy(Arg.Any<string>())
            .ApplyFilter(Arg.Any<IEnumerable<CategoryDTOOutput>>(), Arg.Any<CategoryParameters>())
            .Returns(categoriesDto);
        
        // Act
        var result = await _categoryService.GetCategoriesWithFilter(filter, parameters);
        
        // Assert
        result.Should().BeEquivalentTo(categoriesDto.ToPagedList(parameters.PageNumber, parameters.PageSize));
        
        await _mockUof.CategoryRepository.Received(1).GetAllAsync();
        _mockMapper.Received(1).Map<IEnumerable<CategoryDTOOutput>>(Arg.Any<IEnumerable<Category>>());
        _mockCategoryFilterFactory.Received(1).GetStrategy(Arg.Any<string>())
            .ApplyFilter(Arg.Any<IEnumerable<CategoryDTOOutput>>(), Arg.Any<CategoryParameters>());
    }
    
    [Fact]
    public async Task GetCategoryById_ShouldReturnCategoryThatMatchesExpression_WhenCategoryExists()
    {
        // Arrange
        var categoryId = _fixture.Create<int>();
        var category = _fixture.Create<Category>();
        var categoryDTO = _fixture.Create<CategoryDTOOutput>();
        
        _mockUof.CategoryRepository.GetByIdAsync(Arg.Any<int>()).Returns(category);
        _mockMapper.Map<CategoryDTOOutput>(Arg.Any<Category>()).Returns(categoryDTO);

        // Act
        var result = await _categoryService.GetCategoryById(categoryId);

        // Assert
        result.Should().BeEquivalentTo(Result<CategoryDTOOutput>.Success(categoryDTO));
        result.value.Should().BeEquivalentTo(categoryDTO);
        
        await _mockUof.CategoryRepository.Received(1).GetByIdAsync(Arg.Any<int>());
        _mockMapper.Received(1).Map<CategoryDTOOutput>(Arg.Any<Category>());
    }
    
    [Fact]
    public async Task GetCategoryById_ShouldReturnNotFound_WhenCategoryDoesNotExists()
    {
        // Arrange
        var categoryId = _fixture.Create<int>();
        Category category = null;
        
        _mockUof.CategoryRepository.GetByIdAsync(Arg.Any<int>()).Returns(category);

        // Act
        var result = await _categoryService.GetCategoryById(categoryId);

        // Assert
        result.Should().BeEquivalentTo(Result<CategoryDTOOutput>.Failure(CategoryErrors.NotFound));
        
        await _mockUof.CategoryRepository.Received(1).GetByIdAsync(Arg.Any<int>());
    }

    [Fact]
    public async Task GetCategoryBy_ShouldReturnCategoryThatMatchesExpression_WhenCategoryExists()
    {
        // Arrange
        var category = _fixture.Create<Category>();
        var categoryDTO = _fixture.Create<CategoryDTOOutput>();
        
        _mockUof.CategoryRepository.GetAsync(Arg.Any<Expression<Func<Category, bool>>>()).Returns(category);
        _mockMapper.Map<CategoryDTOOutput>(Arg.Any<Category>()).Returns(categoryDTO);

        // Act
        var result = await _categoryService.GetCategoryBy(c => c.CategoryId == category.CategoryId);

        // Assert
        result.Should().BeEquivalentTo(Result<CategoryDTOOutput>.Success(categoryDTO));
        result.value.Should().BeEquivalentTo(categoryDTO);
        
        await _mockUof.CategoryRepository.Received(1).GetAsync(Arg.Any<Expression<Func<Category, bool>>>());
        _mockMapper.Received(1).Map<CategoryDTOOutput>(Arg.Any<Category>());
    }
    
    [Fact]
    public async Task GetCategoryBy_ShouldReturnNotFound_WhenCategoryDoesNotExists()
    {
        // Arrange
        Category category = null;
        
        _mockUof.CategoryRepository.GetAsync(Arg.Any<Expression<Func<Category, bool>>>()).Returns(category);

        // Act
        var result = await _categoryService.GetCategoryBy(c => c.CategoryId == category.CategoryId);

        // Assert
        result.Should().BeEquivalentTo(Result<CategoryDTOOutput>.Failure(CategoryErrors.NotFound));
        
        await _mockUof.CategoryRepository.Received(1).GetAsync(Arg.Any<Expression<Func<Category, bool>>>());
    }
    
    [Fact]
    public async Task GetProductsOfCategory_ShouldReturnAllProductsOfSomeCategory()
    {
        // Arrange
        var parameters = new QueryStringParameters();
        var categoryId = _fixture.Create<int>();
        var products = _fixture.CreateMany<Product>(3);
        var productsDto = _fixture.CreateMany<ProductDTOOutput>(3);

        _mockUof.ProductRepository.GetAllAsync().Returns(products);
        _mockMapper.Map<IEnumerable<ProductDTOOutput>>(Arg.Any<IEnumerable<Product>>())
            .Returns(productsDto);
        
        // Act
        var result = await _categoryService.GetProducts(categoryId, parameters);
        
        // Assert
        result.Should().BeOfType<PagedList<ProductDTOOutput>>();
        
        await _mockUof.ProductRepository.Received(1).GetAllAsync();
        _mockMapper.Received(1).Map<IEnumerable<ProductDTOOutput>>(Arg.Any<IEnumerable<Product>>());
    }
    
    [Fact]
    public async Task GetProductsByValueOfCategory_ShouldReturnAllProductsOfSomeCategoryThatMatchesSomeValueAndGreaterCriteria()
    {
        // Arrange
        var parameters = new ProductParameters()
        {
            Price = 1.99m,
            PriceCriteria = "greater"
        };
        var categoryId = _fixture.Create<int>();
        var products = _fixture.CreateMany<Product>(3);
        var productsDto = _fixture.CreateMany<ProductDTOOutput>(3);

        _mockUof.ProductRepository.GetAllAsync().Returns(products);
        _mockMapper.Map<IEnumerable<ProductDTOOutput>>(Arg.Any<IEnumerable<Product>>())
            .Returns(productsDto);
        
        // Act
        var result = await _categoryService.GetProductsByValue(categoryId, parameters);
        
        // Assert
        result.Should().BeOfType<PagedList<ProductDTOOutput>>();
        
        await _mockUof.ProductRepository.Received(1).GetAllAsync();
        _mockMapper.Received(1).Map<IEnumerable<ProductDTOOutput>>(Arg.Any<IEnumerable<Product>>());
    }
    
    [Fact]
    public async Task GetProductsByValueOfCategory_ShouldReturnAllProductsOfSomeCategoryThatMatchesSomeValueAndEqualCriteria()
    {
        // Arrange
        var parameters = new ProductParameters()
        {
            Price = _fixture.Create<int>(),
            PriceCriteria = "equal"
        };
        var categoryId = _fixture.Create<int>();
        var products = _fixture.CreateMany<Product>(3);
        var productsDto = _fixture.CreateMany<ProductDTOOutput>(3);

        _mockUof.ProductRepository.GetAllAsync().Returns(products);
        _mockMapper.Map<IEnumerable<ProductDTOOutput>>(Arg.Any<IEnumerable<Product>>())
            .Returns(productsDto);
        
        // Act
        var result = await _categoryService.GetProductsByValue(categoryId, parameters);
        
        // Assert
        result.Should().BeOfType<PagedList<ProductDTOOutput>>();
        
        await _mockUof.ProductRepository.Received(1).GetAllAsync();
        _mockMapper.Received(1).Map<IEnumerable<ProductDTOOutput>>(Arg.Any<IEnumerable<Product>>());
    }
    
    [Fact]
    public async Task GetProductsByValueOfCategory_ShouldReturnAllProductsOfSomeCategoryThatMatchesSomeValueAndLessCriteria()
    {
        // Arrange
        var parameters = new ProductParameters()
        {
            Price = _fixture.Create<int>(),
            PriceCriteria = "less"
        };
        var categoryId = _fixture.Create<int>();
        var products = _fixture.CreateMany<Product>(3);
        var productsDto = _fixture.CreateMany<ProductDTOOutput>(3);

        _mockUof.ProductRepository.GetAllAsync().Returns(products);
        _mockMapper.Map<IEnumerable<ProductDTOOutput>>(Arg.Any<IEnumerable<Product>>())
            .Returns(productsDto);
        
        // Act
        var result = await _categoryService.GetProductsByValue(categoryId, parameters);
        
        // Assert
        result.Should().BeOfType<PagedList<ProductDTOOutput>>();
        
        await _mockUof.ProductRepository.Received(1).GetAllAsync();
        _mockMapper.Received(1).Map<IEnumerable<ProductDTOOutput>>(Arg.Any<IEnumerable<Product>>());
    }
    
    [Fact]
    public async Task GetProductsByValueOfCategory_ShouldReturnAllProductsOfSomeCategoryThatMatchesNoneValueAndCriteria()
    {
        // Arrange
        var parameters = new ProductParameters();
        var categoryId = _fixture.Create<int>();
        var products = _fixture.CreateMany<Product>(3);
        var productsDto = _fixture.CreateMany<ProductDTOOutput>(3);

        _mockUof.ProductRepository.GetAllAsync().Returns(products);
        _mockMapper.Map<IEnumerable<ProductDTOOutput>>(Arg.Any<IEnumerable<Product>>())
            .Returns(productsDto);
        
        // Act
        var result = await _categoryService.GetProductsByValue(categoryId, parameters);
        
        // Assert
        result.Should().BeOfType<PagedList<ProductDTOOutput>>();
        
        await _mockUof.ProductRepository.Received(1).GetAllAsync();
        _mockMapper.Received(1).Map<IEnumerable<ProductDTOOutput>>(Arg.Any<IEnumerable<Product>>());
    }
    
    [Fact]
    public async Task CreateCategoryAsync_ValidInput_ShouldCreateCategory()
    {
        // Arrange
        var categoryDtoInput = _fixture.Create<CategoryDTOInput>();
        Category categoryExists = null;
        var category = _fixture.Create<Category>();
        var categoryDtoOutput = _fixture.Create<CategoryDTOOutput>();
        var validationResult = new ValidationResult()
        {
            // IsValid == True caso Erros.Count() seja 0;
            Errors = new List<ValidationFailure>()
        };
        _mockValidator.ValidateAsync(Arg.Any<CategoryDTOInput>())
            .Returns(validationResult);
        _mockUof.CategoryRepository.GetAsync(Arg.Any<Expression<Func<Category, bool>>>())
            .Returns(categoryExists);
        _mockMapper.Map<Category>(Arg.Any<CategoryDTOInput>())
            .Returns(category);
        _mockUof.CategoryRepository.Create(Arg.Any<Category>())
            .Returns(category);
        _mockMapper.Map<CategoryDTOOutput>(Arg.Any<Category>())
            .Returns(categoryDtoOutput);

        // Act
        var result = await _categoryService.CreateCategory(categoryDtoInput);

        // Assert
        result.isSuccess.Should().BeTrue();
        result.value.Should().BeOfType<CategoryDTOOutput>();
        result.value.Should().BeEquivalentTo(categoryDtoOutput);

        await _mockValidator.Received(1).ValidateAsync(Arg.Any<CategoryDTOInput>());
        await _mockUof.CategoryRepository.Received(1).GetAsync(Arg.Any<Expression<Func<Category, bool>>>());
        _mockUof.CategoryRepository.Received(1).Create(Arg.Any<Category>());
        _mockMapper.Received(1).Map<Category>(Arg.Any<CategoryDTOInput>());
        _mockMapper.Received(1).Map<CategoryDTOOutput>(Arg.Any<Category>());
    }
    
    [Fact]
    public async Task CreateCategoryAsync_ShouldReturn500BadRequestWithDuplicateDataResponse_WhenCategoryAlreadyExists()
    {
        // Arrange
        var categoryDtoInput = _fixture.Create<CategoryDTOInput>();
        var validationResult = new ValidationResult()
        {
            // IsValid == True caso Erros.Count() seja 0;
            Errors = new List<ValidationFailure>()
        };
        var categoryExists = _fixture.Create<Category>();
        
        _mockValidator.ValidateAsync(Arg.Any<CategoryDTOInput>())
            .Returns(validationResult);
        _mockUof.CategoryRepository.GetAsync(Arg.Any<Expression<Func<Category, bool>>>())
            .Returns(categoryExists);

        // Act
        var result = await _categoryService.CreateCategory(categoryDtoInput);

        // Assert
        result.Should().BeOfType<Result<CategoryDTOOutput>>();
        result.error.Should().BeEquivalentTo(CategoryErrors.DuplicateData);

        await _mockValidator.Received(1).ValidateAsync(Arg.Any<CategoryDTOInput>());
        await _mockUof.CategoryRepository.Received(1).GetAsync(Arg.Any<Expression<Func<Category, bool>>>());
    }
    
    [Fact]
    public async Task CreateCategoryAsync_ShouldReturn500BadRequestWithIncorrectFormatDataResponse_WhenCategoryInputDataIsInvalid()
    {
        // Arrange
        var categoryDtoInput = _fixture.Create<CategoryDTOInput>();
        var validationResult = new ValidationResult()
        {
            // IsValid == True caso Erros.Count() seja 0;
            Errors = new List<ValidationFailure>()
            {
                new ValidationFailure("InvalidFormatData", "InvalidFormatData")
            }
        };
        
        _mockValidator.ValidateAsync(Arg.Any<CategoryDTOInput>())
            .Returns(validationResult);

        // Act
        var result = await _categoryService.CreateCategory(categoryDtoInput);

        // Assert
        result.Should().BeOfType<Result<CategoryDTOOutput>>();
        result.error.Should().BeEquivalentTo(CategoryErrors.IncorrectFormatData);
        result.validationFailures.Should().NotBeEmpty();

        await _mockValidator.Received(1).ValidateAsync(Arg.Any<CategoryDTOInput>());
    }
    
    [Fact]
    public async Task CreateCategoryAsync_ShouldReturn500BadRequestWithNullDataResponse_WhenCategoryInputDataIsNull()
    {
        // Arrange
        CategoryDTOInput categoryDtoInput = null;

        // Act
        var result = await _categoryService.CreateCategory(categoryDtoInput);

        // Assert
        result.Should().BeOfType<Result<CategoryDTOOutput>>();
        result.error.Should().BeEquivalentTo(CategoryErrors.DataIsNull);
    }
    
    [Fact]
    public async Task UpdateCategoryAsync_ShouldReturnUpdateCategory()
    {
        // Arrange
        var categoryDtoInput = _fixture.Create<CategoryDTOInput>();
        var categoryId = categoryDtoInput.CategoryId;
        var validationResult = new ValidationResult()
        {
            // IsValid == True caso Erros.Count() seja 0;
            Errors = new List<ValidationFailure>()
        };
        var categoryExists = _fixture.Create<Category>();
        var categoryForUpdate = _fixture.Create<Category>();
        var categoryDtoOutput = _fixture.Create<CategoryDTOOutput>();
        
        _mockValidator.ValidateAsync(Arg.Any<CategoryDTOInput>())
            .Returns(validationResult);
        _mockUof.CategoryRepository.GetAsync(Arg.Any<Expression<Func<Category, bool>>>())
            .Returns(categoryExists);
        _mockMapper.Map<Category>(Arg.Any<CategoryDTOInput>())
            .Returns(categoryForUpdate);
        _mockUof.CategoryRepository.Update(Arg.Any<Category>())
            .Returns(categoryForUpdate);
        _mockMapper.Map<CategoryDTOOutput>(Arg.Any<Category>())
            .Returns(categoryDtoOutput);

        // Act
        var result = await _categoryService.UpdateCategory(categoryDtoInput, categoryId);

        // Assert
        result.isSuccess.Should().BeTrue();
        result.value.Should().BeOfType<CategoryDTOOutput>();
        result.value.Should().BeEquivalentTo(categoryDtoOutput);

        await _mockValidator.Received(1).ValidateAsync(Arg.Any<CategoryDTOInput>());
        await _mockUof.CategoryRepository.Received(1).GetAsync(Arg.Any<Expression<Func<Category, bool>>>());
        _mockUof.CategoryRepository.Received(1).Update(Arg.Any<Category>());
        _mockMapper.Received(1).Map<Category>(Arg.Any<CategoryDTOInput>());
        _mockMapper.Received(1).Map<CategoryDTOOutput>(Arg.Any<Category>());
    }
    
    [Fact]
    public async Task UpdateCategoryAsync_ShouldReturnNotFoundResponse_WhenCategoryDoesNotExist()
    {
        // Arrange
        var categoryDtoInput = _fixture.Create<CategoryDTOInput>();
        var categoryId = categoryDtoInput.CategoryId;
        var validationResult = new ValidationResult()
        {
            // IsValid == True caso Erros.Count() seja 0;
            Errors = new List<ValidationFailure>()
        };
        Category categoryExists = null;
        
        _mockValidator.ValidateAsync(Arg.Any<CategoryDTOInput>())
            .Returns(validationResult);
        _mockUof.CategoryRepository.GetAsync(Arg.Any<Expression<Func<Category, bool>>>())
            .Returns(categoryExists);

        // Act
        var result = await _categoryService.UpdateCategory(categoryDtoInput, categoryId);

        // Assert
        result.Should().BeOfType<Result<CategoryDTOOutput>>();
        result.error.Should().BeEquivalentTo(CategoryErrors.NotFound);

        await _mockValidator.Received(1).ValidateAsync(Arg.Any<CategoryDTOInput>());
        await _mockUof.CategoryRepository.Received(1).GetAsync(Arg.Any<Expression<Func<Category, bool>>>());
    }
    
    [Fact]
    public async Task UpdateCategoryAsync_ShouldReturnBadRequestWithIncorrectDataFormatResponse_WhenCategoryInputDataIsInvalid()
    {
        // Arrange
        var categoryDtoInput = _fixture.Create<CategoryDTOInput>();
        var categoryId = categoryDtoInput.CategoryId;
        var validationResult = new ValidationResult()
        {
            // IsValid == True caso Erros.Count() seja 0;
            Errors = new List<ValidationFailure>()
            {
                new ValidationFailure("InvalidFormatData", "InvalidFormatData")
            }
        };
        
        _mockValidator.ValidateAsync(Arg.Any<CategoryDTOInput>())
            .Returns(validationResult);

        // Act
        var result = await _categoryService.UpdateCategory(categoryDtoInput, categoryId);

        // Assert
        result.Should().BeOfType<Result<CategoryDTOOutput>>();
        result.error.Should().BeEquivalentTo(CategoryErrors.IncorrectFormatData);

        await _mockValidator.Received(1).ValidateAsync(Arg.Any<CategoryDTOInput>());
    }
    
    [Fact]
    public async Task UpdateCategoryAsync_ShouldReturnBadRequestWithIdMismatchResponse_WhenCategoryAndCategoryIdInputDataIsInvalid()
    {
        // Arrange
        var categoryDtoInput = _fixture.Create<CategoryDTOInput>();
        var categoryId =  _fixture.Create<int>();

        // Act
        var result = await _categoryService.UpdateCategory(categoryDtoInput, categoryId);

        // Assert
        result.Should().BeOfType<Result<CategoryDTOOutput>>();
        result.error.Should().BeEquivalentTo(CategoryErrors.IdMismatch);
    }
    
    [Fact]
    public async Task UpdateCategoryAsync_ShouldReturnBadRequestWithDataNullResponse_WhenCategoryInputDataIsNull()
    {
        // Arrange
        CategoryDTOInput categoryDtoInput = null;
        var categoryId =  _fixture.Create<int>();

        // Act
        var result = await _categoryService.UpdateCategory(categoryDtoInput, categoryId);

        // Assert
        result.Should().BeOfType<Result<CategoryDTOOutput>>();
        result.error.Should().BeEquivalentTo(CategoryErrors.DataIsNull);
    }
    
    [Fact]
    public async Task DeleteCategoryAsync_ShouldReturnSuccessWithCategoryDeleted()
    {
        // Arrange
        var categoryId =  _fixture.Create<int>();
        var category = _fixture.Create<Category>();
        var categoryDtoDeleted = _fixture.Create<CategoryDTOOutput>();
        
        _mockUof.CategoryRepository.GetAsync(Arg.Any<Expression<Func<Category, bool>>>()).Returns(category);
        _mockUof.CategoryRepository.Delete(Arg.Any<Category>()).Returns(category);
        _mockMapper.Map<CategoryDTOOutput>(Arg.Any<Category>()).Returns(categoryDtoDeleted);

        // Act
        var result = await _categoryService.DeleteCategory(categoryId);

        // Assert
        result.isSuccess.Should().BeTrue();
        result.Should().BeOfType<Result<CategoryDTOOutput>>();
        result.value.Should().BeEquivalentTo(categoryDtoDeleted);
        
        await _mockUof.CategoryRepository.Received(1).GetAsync(Arg.Any<Expression<Func<Category, bool>>>());
        _mockUof.CategoryRepository.Received(1).Delete(Arg.Any<Category>());
        _mockMapper.Received(1).Map<CategoryDTOOutput>(Arg.Any<Category>());
    }
    
    [Fact]
    public async Task DeleteCategoryAsync_ShouldReturnNotFoundNotFoundResponse_WhenCategoryDoesNotExist()
    {
        // Arrange
        var categoryId =  _fixture.Create<int>();
        Category category = null;
        
        _mockUof.CategoryRepository.GetAsync(Arg.Any<Expression<Func<Category, bool>>>()).Returns(category);

        // Act
        var result = await _categoryService.DeleteCategory(categoryId);

        // Assert
        result.Should().BeOfType<Result<CategoryDTOOutput>>();
        result.error.Should().BeEquivalentTo(CategoryErrors.NotFound);
        
        await _mockUof.CategoryRepository.Received(1).GetAsync(Arg.Any<Expression<Func<Category, bool>>>());
    }
}