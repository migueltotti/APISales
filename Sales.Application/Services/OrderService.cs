using System.Linq.Expressions;
using System.Text.Encodings.Web;
using AutoMapper;
using FluentValidation;
using Sales.Application.DTOs.OrderDTO;
using Sales.Application.DTOs.ProductDTO;
using Sales.Application.Interfaces;
using Sales.Application.Parameters;
using Sales.Application.Parameters.ModelsParameters;
using Sales.Application.ResultPattern;
using Sales.Domain.Interfaces;
using Sales.Domain.Models;
using Sales.Domain.Models.Enums;
using X.PagedList;
using X.PagedList.Extensions;

namespace Sales.Application.Services;

public class OrderService : IOrderService
{
    private readonly IUnitOfWork _uof;
    private readonly IValidator<OrderDTOInput> _validator;
    private readonly IMapper _mapper;
    private readonly IOrderFilterFactory _orderFilterFactory;

    public OrderService(IUnitOfWork uof, IValidator<OrderDTOInput> validator, IMapper mapper, IOrderFilterFactory orderFilterFactory)
    {
        _uof = uof;
        _validator = validator;
        _mapper = mapper;
        _orderFilterFactory = orderFilterFactory;
    }

    public async Task<IEnumerable<OrderDTOOutput>> GetAllOrders()
    {
        var orders = await _uof.OrderRepository.GetAllAsync();

        return _mapper.Map<IEnumerable<OrderDTOOutput>>(orders);
    }

    public async Task<IPagedList<OrderDTOOutput>> GetAllOrders(QueryStringParameters parameters)
    {
        var orders = (await GetAllOrders()).OrderBy(o => o.OrderDate);
        
        return orders.ToPagedList(parameters.PageNumber, parameters.PageSize);
    }

    public async Task<IPagedList<OrderDTOOutput>> GetOrdersWithFilter(string filter, OrderParameters parameters)
    {
        var orders = await GetAllOrders();
        
        orders = _orderFilterFactory.GetStrategy(filter).ApplyFilter(orders, parameters);
        
        return orders.ToPagedList(parameters.PageNumber, parameters.PageSize);
    }

    public async Task<IPagedList<OrderDTOOutput>> GetOrdersByProduct(OrderParameters parameters)
    {
        var orders = new List<Order>();

        if (parameters.ProductName is not null)
        {
            orders = (await _uof.OrderRepository.GetOrdersByProduct(parameters.ProductName)).ToList();
        }
        
        var orderDto = _mapper.Map<IEnumerable<OrderDTOOutput>>(orders);
        
        return orderDto.ToPagedList(parameters.PageNumber, parameters.PageSize);
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

    public async Task<Result<OrderDTOOutput>> FinishOrder(int orderId)
    {
        var order = await _uof.OrderRepository.GetAsync(o => o.OrderId == orderId);

        if (order is null)
        {
            return Result<OrderDTOOutput>.Failure(OrderErrors.NotFound);
        }
        
        order.FinishOrder();
        _uof.OrderRepository.Update(order);
        
        await _uof.CommitChanges();
        
        return Result<OrderDTOOutput>.Success(_mapper.Map<OrderDTOOutput>(order));
    }

    public async Task<Result<OrderProductDTO>> AddProduct(int orderId, int productId)
    {
        var order = await _uof.OrderRepository.GetAsync(o => o.OrderId == orderId);
        
        if(order is null)
            return Result<OrderProductDTO>.Failure(OrderErrors.NotFound);

        if (order.OrderStatus is Status.Finished)
            return Result<OrderProductDTO>.Failure(OrderErrors.OrderFinished);
        
        var product = await _uof.ProductRepository.GetAsync(p => p.ProductId == productId);
        
        if(product is null)
            return Result<OrderProductDTO>.Failure(ProductErrors.NotFound);
        
        var rowsAffected = await _uof.OrderRepository.AddProduct(order.OrderId, product.ProductId);

        if (!(rowsAffected > 0))
        {
            return Result<OrderProductDTO>.Failure(OrderErrors.DataIsNull);
        }
        
        // Increase Order TotalValue
        order.IncreaseValue(product.Value);
        _uof.OrderRepository.Update(order);
        
        await _uof.CommitChanges();
        order.Products.Add(product);

        return Result<OrderProductDTO>.Success(_mapper.Map<OrderProductDTO>(order));
    }

    public async Task<Result<IEnumerable<ProductDTOOutput>>> GetProductsByOrderId(int orderId)
    {
        var order = await _uof.OrderRepository.GetAsync(o => o.OrderId == orderId);
        
        if(order is null)
            return Result<IEnumerable<ProductDTOOutput>>.Failure(OrderErrors.NotFound);
        
        var productsOrder = await _uof.OrderRepository.GetProducts(orderId);

        if (!productsOrder.Any())
            return Result<IEnumerable<ProductDTOOutput>>.Failure(OrderErrors.ProductsNotFound);
        
        return Result<IEnumerable<ProductDTOOutput>>.Success(_mapper.Map<IEnumerable<ProductDTOOutput>>(productsOrder));
    }

    public async Task<Result<OrderProductDTO>> RemoveProduct(int orderId, int productId)
    {
        var order = await _uof.OrderRepository.GetAsync(o => o.OrderId == orderId);
        
        if(order is null)
            return Result<OrderProductDTO>.Failure(OrderErrors.NotFound);
        
        var rowsAffected = await _uof.OrderRepository.RemoveProduct(order.OrderId, productId);

        if (!(rowsAffected > 0))
            return Result<OrderProductDTO>.Failure(OrderErrors.ProductNotFound);
        
        // Decrease Order TotalValue
        var product = await _uof.ProductRepository.GetAsync(p => p.ProductId == productId);
        order.DecreaseValue(product.Value);
        _uof.OrderRepository.Update(order);
        
        await _uof.CommitChanges();
        
        return Result<OrderProductDTO>.Success(_mapper.Map<OrderProductDTO>(order));
    }

    public async Task<OrderReportDTO> GetOrderReport(DateTime startDate, DateTime endDate)
    {
        var orders = await GetAllOrders();
        
        var ordersByDate = orders.Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate);

        var products = await _uof.OrderRepository.GetProductsByDate(startDate, endDate);

        return new OrderReportDTO()
        {
            OrdersCount = ordersByDate.Count(),
            OrdersTotalValue = ordersByDate.Sum(o => o.TotalValue),
            OrdersTotalProducts = products.Count(),
            OrderMinDate = startDate,
            OrderMaxDate = endDate
        };
    }
}