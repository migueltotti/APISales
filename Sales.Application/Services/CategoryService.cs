using System.Linq.Expressions;
using AutoMapper;
using FluentValidation;
using Sales.Application.DTOs.CategoryDTO;
using Sales.Application.Interfaces;
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

    public CategoryService(IUnitOfWork unitOfWork, IValidator<CategoryDTOInput> validator, IMapper mapper)
    {
        _uof = unitOfWork;
        _validator = validator;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CategoryDTOOutput>> GetAllCategories()
    {
        var categories = await _uof.CategoryRepository.GetAllAsync();
        
        return _mapper.Map<IEnumerable<CategoryDTOOutput>>(categories);
    }
    
    public async Task<IPagedList<CategoryDTOOutput>> GetAllCategories(CategoryParameters parameters)
    {
        var categories = (await GetAllCategories()).OrderBy(c => c.Name);

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