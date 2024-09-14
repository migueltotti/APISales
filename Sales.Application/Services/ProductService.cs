using System.Linq.Expressions;
using AutoMapper;
using FluentValidation;
using Sales.Application.DTOs.ProductDTO;
using Sales.Application.Interfaces;
using Sales.Application.ResultPattern;
using Sales.Domain.Interfaces;
using Sales.Domain.Models;

namespace Sales.Application.Services;

public class ProductService : IProductService
{
    private readonly IUnitOfWork _uof;
    private readonly IValidator<ProductDTOInput> _validator;
    private readonly IMapper _mapper;

    public ProductService(IUnitOfWork uof, IValidator<ProductDTOInput> validator, IMapper mapper)
    {
        _uof = uof;
        _validator = validator;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ProductDTOOutput>> GetAllProducts()
    {
        var products = await _uof.ProductRepository.GetAllAsync();

        return _mapper.Map<IEnumerable<ProductDTOOutput>>(products);
    }

    public async Task<Result<ProductDTOOutput>> GetProductBy(Expression<Func<Product, bool>> expression)
    {
        var product = await _uof.ProductRepository.GetAsync(expression);

        if (product is null)
        {
            return Result<ProductDTOOutput>.Failure(ProductErrors.NotFound);
        }

        var productDto = _mapper.Map<ProductDTOOutput>(product);
        
        return Result<ProductDTOOutput>.Success(productDto);
    }

    public async Task<Result<ProductDTOOutput>> CreateProduct(ProductDTOInput productDtoInput)
    {
        if (productDtoInput is null)    
        {
            return Result<ProductDTOOutput>.Failure(ProductErrors.DataIsNull);
        }
        
        var validation = await _validator.ValidateAsync(productDtoInput);

        if (!validation.IsValid)
        {
            return Result<ProductDTOOutput>.Failure(ProductErrors.IncorrectFormatData);
        }

        var product = _mapper.Map<Product>(productDtoInput);

        var productCreated = _uof.ProductRepository.Create(product);
        await _uof.CommitChanges();

        var productDtoCreated = _mapper.Map<ProductDTOOutput>(productCreated);   
        
        return Result<ProductDTOOutput>.Success(productDtoCreated);
    }

    public async Task<Result<ProductDTOOutput>> UpdateProduct(ProductDTOInput productDtoInput, int id)
    {
        if (productDtoInput is null)
        {
            return Result<ProductDTOOutput>.Failure(ProductErrors.DataIsNull);
        }

        if (id != productDtoInput.ProductId)
        {
            return Result<ProductDTOOutput>.Failure(ProductErrors.IdMismatch);
        }

        var product = await _uof.ProductRepository.GetAsync(p => p.ProductId == id);

        if (product is null)
        {
            return Result<ProductDTOOutput>.Failure(ProductErrors.NotFound);
        }
        
        var validation = await _validator.ValidateAsync(productDtoInput);

        if (!validation.IsValid)
        {
            return Result<ProductDTOOutput>.Failure(ProductErrors.IncorrectFormatData);
        }
        
        var productForUpdate = _mapper.Map<Product>(productDtoInput);

        var productUpdated = _uof.ProductRepository.Update(productForUpdate);
        await _uof.CommitChanges();

        var productDtoUpdated = _mapper.Map<ProductDTOOutput>(productUpdated);

        return Result<ProductDTOOutput>.Success(productDtoUpdated);
    }

    public async Task<Result<ProductDTOOutput>> DeleteProduct(int? id)
    {
        var product = await _uof.ProductRepository.GetAsync(p => p.ProductId == id);
        
        if (product is null)
        {
            return Result<ProductDTOOutput>.Failure(ProductErrors.NotFound);
        }

        var productDeleted = _uof.ProductRepository.Delete(product);
        await _uof.CommitChanges();

        var productDtoDeleted = _mapper.Map<ProductDTOOutput>(productDeleted);

        return Result<ProductDTOOutput>.Success(productDtoDeleted);
    }
}