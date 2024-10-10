using System.Linq.Expressions;
using AutoFixture;
using AutoMapper;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using NSubstitute;
using Sales.Application.DTOs.CategoryDTO;
using Sales.Application.Interfaces;
using Sales.Application.Services;
using Sales.Domain.Interfaces;
using Sales.Domain.Models;

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
}