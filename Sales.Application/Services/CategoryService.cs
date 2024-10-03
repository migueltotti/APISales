using System.Globalization;
using System.Linq.Expressions;
using AutoMapper;
using FluentValidation;
using Sales.Application.DTOs.CategoryDTO;
using Sales.Application.DTOs.ProductDTO;
using Sales.Application.Interfaces;
using Sales.Application.Parameters;
using Sales.Application.Parameters.ModelsParameters;
using Sales.Application.ResultPattern;
using Sales.Domain.Models;
using Sales.Domain.Interfaces;
using X.PagedList;
using X.PagedList.Extensions;

namespace Sales.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly IUnitOfWork _uof;
    private readonly IValidator<CategoryDTOInput> _validator;
    private readonly IMapper _mapper;
    private readonly ICategoryFilterFactory _categoryFilterFactory;

    public CategoryService(IUnitOfWork unitOfWork, IValidator<CategoryDTOInput> validator, IMapper mapper, ICategoryFilterFactory categoryFilterFactory)
    {
        _uof = unitOfWork;
        _validator = validator;
        _mapper = mapper;
        _categoryFilterFactory = categoryFilterFactory;
    }

    public async Task<IEnumerable<CategoryDTOOutput>> GetAllCategories()
    {
        var categories = await _uof.CategoryRepository.GetAllAsync();
        
        return _mapper.Map<IEnumerable<CategoryDTOOutput>>(categories);
    }
    
    public async Task<IPagedList<CategoryDTOOutput>> GetAllCategories(QueryStringParameters parameters)
    {
        var categories = (await GetAllCategories()).OrderBy(c => c.Name);

        return categories.ToPagedList(parameters.PageNumber, parameters.PageSize);
    }

    public async Task<IPagedList<CategoryDTOOutput>> GetCategoriesWithFilter(string filter, CategoryParameters parameters)
    {
        var categories = await GetAllCategories();
        
        categories = _categoryFilterFactory.GetStrategy(filter).ApplyFilter(categories, parameters);
        
        return categories.ToPagedList(parameters.PageNumber, parameters.PageSize);
    }
    
    

    public async Task<Result<CategoryDTOOutput>> GetCategoryBy(Expression<Func<Category, bool>> expression)
    {
        var category = await _uof.CategoryRepository.GetAsync(expression);

        if (category is null)
        {
            return Result<CategoryDTOOutput>.Failure(CategoryErrors.NotFound);
        }
        
        var categoryDto = _mapper.Map<CategoryDTOOutput>(category);

        return Result<CategoryDTOOutput>.Success(categoryDto);
    }

    public async Task<IPagedList<ProductDTOOutput>> GetProducts(int categoryId, QueryStringParameters parameters)
    {
        var products = await _uof.ProductRepository.GetAllAsync();
        
        var categoryProducts = products.Where(p => p.CategoryId == categoryId);
        
        var categoryProductsDto = _mapper.Map<IEnumerable<ProductDTOOutput>>(categoryProducts);
        
        return categoryProductsDto.ToPagedList(parameters.PageNumber, parameters.PageSize);
    }

    public async Task<IPagedList<ProductDTOOutput>> GetProductsByValue(int categoryId, ProductParameters parameters)
    {
        var products = await _uof.ProductRepository.GetAllAsync();
        
        var categoryProducts = products.Where(p => p.CategoryId == categoryId);

        if (parameters is { Price: not null, PriceCriteria: not null })
        {
            categoryProducts = parameters.PriceCriteria.ToLower() switch
            {
                "greater" => categoryProducts.Where(p => p.Value > parameters.Price).OrderBy(p => p.Value),
                "equal" => categoryProducts.Where(p => p.Value == parameters.Price).OrderBy(p => p.Value),
                "less" => categoryProducts.Where(p => p.Value < parameters.Price).OrderBy(p => p.Value),
                _ => categoryProducts
            };
        }
        
        var categoryProductsDto = _mapper.Map<IEnumerable<ProductDTOOutput>>(categoryProducts);
        
        return categoryProductsDto.ToPagedList(parameters.PageNumber, parameters.PageSize);
    }

    public async Task<Result<CategoryDTOOutput>> CreateCategory(CategoryDTOInput categoryDto)
    {
        if (categoryDto is null)
        {
            return Result<CategoryDTOOutput>.Failure(CategoryErrors.DataIsNull);
        }
        
        var validation = await _validator.ValidateAsync(categoryDto);
        
        if (!validation.IsValid)
        {
            return Result<CategoryDTOOutput>.Failure(CategoryErrors.IncorrectFormatData, validation.Errors);
        }
        
        var categoryExists = await _uof.CategoryRepository.GetAsync(c => c.Name == categoryDto.Name);
        if (categoryExists is not null)
            return Result<CategoryDTOOutput>.Failure(CategoryErrors.DuplicateData);

        var category = _mapper.Map<Category>(categoryDto);
            
        var categoryCreated = _uof.CategoryRepository.Create(category);
        await _uof.CommitChanges();

        var categoryDtoToReturn = _mapper.Map<CategoryDTOOutput>(categoryCreated);

        return Result<CategoryDTOOutput>.Success(categoryDtoToReturn);
        
    }

    public async Task<Result<CategoryDTOOutput>> UpdateCategory(CategoryDTOInput categoryDtoInput, int id)
    {
        if (categoryDtoInput is null)
        {
            return Result<CategoryDTOOutput>.Failure(CategoryErrors.DataIsNull); 
        }

        if (id != categoryDtoInput.CategoryId)
        {
            return Result<CategoryDTOOutput>.Failure(CategoryErrors.IdMismatch); 
        }
        
        var validation = await _validator.ValidateAsync(categoryDtoInput);
        
        if (!validation.IsValid)
        {
            return Result<CategoryDTOOutput>.Failure(CategoryErrors.IncorrectFormatData, validation.Errors);
        }
        
        var category = await _uof.CategoryRepository.GetAsync(c => c.CategoryId == id);

        if (category is null)
        {
            return Result<CategoryDTOOutput>.Failure(CategoryErrors.NotFound);
        }
        
        var categoryForUpdate = _mapper.Map<Category>(categoryDtoInput);

        var categoryUpdated = _uof.CategoryRepository.Update(categoryForUpdate);
        await _uof.CommitChanges();
        
        var categoryUpdatedDto = _mapper.Map<CategoryDTOOutput>(categoryUpdated);

        return Result<CategoryDTOOutput>.Success(categoryUpdatedDto);
    }

    public async Task<Result<CategoryDTOOutput>> DeleteCategory(int? id)
    {
        var category = await _uof.CategoryRepository.GetAsync(c => c.CategoryId == id);

        if (category is null)
        {
            return Result<CategoryDTOOutput>.Failure(CategoryErrors.NotFound); 
        }
        
        var categoryDeleted = _uof.CategoryRepository.Delete(category);
        await _uof.CommitChanges();

        var categoryDtoDeleted = _mapper.Map<CategoryDTOOutput>(categoryDeleted);
        
        return Result<CategoryDTOOutput>.Success(categoryDtoDeleted);
    }
}