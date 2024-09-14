using System.Linq.Expressions;
using AutoMapper;
using FluentValidation;
using Sales.Application.DTOs.OrderDTO;
using Sales.Application.DTOs.ProductDTO;
using Sales.Application.Interfaces;
using Sales.Application.ResultPattern;
using Sales.Domain.Interfaces;
using Sales.Domain.Models;

namespace Sales.Application.Services;

public class OrderService : IOrderService
{
    private readonly IUnitOfWork _uof;
    private readonly IValidator<OrderDTOInput> _validator;
    private readonly IMapper _mapper;

    public OrderService(IUnitOfWork uof, IValidator<OrderDTOInput> validator, IMapper mapper)
    {
        _uof = uof;
        _validator = validator;
        _mapper = mapper;
    }

    public async Task<IEnumerable<OrderDTOOutput>> GetAllOrders()
    {
        var orders = await _uof.OrderRepository.GetAllAsync();

        return _mapper.Map<IEnumerable<OrderDTOOutput>>(orders);
    }

    public async Task<Result<OrderDTOOutput>> GetOrderBy(Expression<Func<Order, bool>> expression)
    {
        var order = await _uof.OrderRepository.GetAsync(expression);

        if (order is null)
        {
            return Result<OrderDTOOutput>.Failure(OrderErrors.NotFound);
        }

        var orderDto = _mapper.Map<OrderDTOOutput>(order);
        
        return Result<OrderDTOOutput>.Success(orderDto);
    }

    public async Task<Result<OrderDTOOutput>> CreateOrder(OrderDTOInput orderDtoInput)
    {
        if (orderDtoInput is null)
        {
            return Result<OrderDTOOutput>.Failure(OrderErrors.DataIsNull);
        }
        
        var validation = await _validator.ValidateAsync(orderDtoInput);

        if (!validation.IsValid)
        {
            Result<OrderDTOOutput>.Failure(OrderErrors.IncorrectFormatData, validation.Errors);
        }

        var order = _mapper.Map<Order>(orderDtoInput);

        var orderCreated = _uof.OrderRepository.Create(order);
        await _uof.CommitChanges();

        var orderDtoCreated = _mapper.Map<OrderDTOOutput>(orderCreated);

        return Result<OrderDTOOutput>.Success(orderDtoCreated);
    }

    public async Task<Result<OrderDTOOutput>> UpdateOrder(OrderDTOInput orderDtoInput, int id)
    {
        if (orderDtoInput is null)
        {
            return Result<OrderDTOOutput>.Failure(OrderErrors.DataIsNull);
        }

        if (id != orderDtoInput.OrderId)
        {
            return Result<OrderDTOOutput>.Failure(OrderErrors.IdMismatch);
        }
        
        var validation = await _validator.ValidateAsync(orderDtoInput);

        if (!validation.IsValid)
        {
            Result<OrderDTOOutput>.Failure(OrderErrors.IncorrectFormatData, validation.Errors);
        }
        
        var order = await _uof.OrderRepository.GetAsync(o => o.OrderId == id);

        if (order is null)
        {
            Result<OrderDTOOutput>.Failure(OrderErrors.NotFound);
        }

        var orderForUpdate = _mapper.Map<Order>(orderDtoInput);

        var orderUpdated = _uof.OrderRepository.Update(orderForUpdate);
        await _uof.CommitChanges();

        var orderDtoUpdated = _mapper.Map<OrderDTOOutput>(orderUpdated);

        return Result<OrderDTOOutput>.Success(orderDtoUpdated);
    }

    public async Task<Result<OrderDTOOutput>> DeleteOrder(int? id)
    {
        var order = await _uof.OrderRepository.GetAsync(o => o.OrderId == id);
        
        if (order is null)
        {
            Result<OrderDTOOutput>.Failure(OrderErrors.NotFound);
        }

        var orderDeleted = _uof.OrderRepository.Delete(order);
        await _uof.CommitChanges();

        var orderDtoDeleted = _mapper.Map<OrderDTOOutput>(orderDeleted);

        return Result<OrderDTOOutput>.Success(orderDtoDeleted);
    }

    public async Task AddProduct(ProductDTOInput product)
    {
        throw new NotImplementedException();
    }

    public async Task RemoveProduct(ProductDTOInput product)
    {
        throw new NotImplementedException();
    }

    public async Task<OrderReportDTO> GetOrderReport(DateTime startDate, DateTime endDate)
    {
        throw new NotImplementedException();
    }
}