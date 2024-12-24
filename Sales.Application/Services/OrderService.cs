using System.Linq.Expressions;
using System.Runtime.InteropServices.JavaScript;
using System.Text.Encodings.Web;
using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.IdentityModel.Tokens;
using Sales.Application.DTOs.OrderDTO;
using Sales.Application.DTOs.ProductDTO;
using Sales.Application.Interfaces;
using Sales.Application.Mapping.Extentions;
using Sales.Application.Parameters;
using Sales.Application.Parameters.ModelsParameters;
using Sales.Application.ResultPattern;
using Sales.Application.Strategy.FilterImplementation.OrderStrategy;
using Sales.Application.Strategy.FilterImplementation.ProductStrategy;
using Sales.Domain.Interfaces;
using Sales.Domain.Models;
using Sales.Domain.Models.Enums;
using X.PagedList;
using X.PagedList.Extensions;
using ValidationFailure = FluentValidation.Results.ValidationFailure;

namespace Sales.Application.Services;

public class OrderService : IOrderService
{
    private readonly IUnitOfWork _uof;
    private readonly IShoppingCartService _shoppingCartService;
    private readonly IValidator<OrderDTOInput> _validator;
    private readonly IMapper _mapper;
    private readonly IOrderFilterFactory _orderFilterFactory;
    private readonly ICacheService _cacheService;

    public OrderService(IUnitOfWork uof, IShoppingCartService shoppingCartService, IValidator<OrderDTOInput> validator, IMapper mapper, IOrderFilterFactory orderFilterFactory, ICacheService cacheService)
    {
        _uof = uof;
        _shoppingCartService = shoppingCartService;
        _validator = validator;
        _mapper = mapper;
        _orderFilterFactory = orderFilterFactory;
        _cacheService = cacheService;
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

    public async Task<IPagedList<OrderDTOOutput>> GetOrdersByUserId(int userId, QueryStringParameters parameters)
    {
        var orders = await _uof.OrderRepository.GetOrdersWithProductsByUserId(userId);
        
        orders = orders.OrderBy(o => o.OrderDate);
        
        var ordersDto = _mapper.Map<IEnumerable<OrderDTOOutput>>(orders);
        
        return ordersDto.ToPagedList(parameters.PageNumber, parameters.PageSize);
    }

    public async Task<Result<OrderDTOOutput>> GetOrderById(int id)
    {
        var order = await _uof.OrderRepository.GetByIdAsync(id);

        if (order is null)
        {
            return Result<OrderDTOOutput>.Failure(OrderErrors.NotFound);
        }

        var orderDto = _mapper.Map<OrderDTOOutput>(order);
        
        return Result<OrderDTOOutput>.Success(orderDto);
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

    public async Task<IPagedList<OrderDTOOutput>> GetOrdersByAffiliateId(int affiliateId, OrderParameters parameters)
    {
        var orders = await _uof.OrderRepository.GetOrdersByAffiliateId(affiliateId);
        
        var ordersDto = _mapper.Map<IEnumerable<OrderDTOOutput>>(orders);
        
        return ordersDto.ToPagedList(parameters.PageNumber, parameters.PageSize);
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
            return Result<OrderDTOOutput>.Failure(OrderErrors.IncorrectFormatData, validation.Errors);
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
            return Result<OrderDTOOutput>.Failure(OrderErrors.IncorrectFormatData, validation.Errors);
        }
        
        var order = await _uof.OrderRepository.GetByIdAsync(id);

        if (order is null)
        {
            return Result<OrderDTOOutput>.Failure(OrderErrors.NotFound);
        }

        var orderForUpdate = _mapper.Map<Order>(orderDtoInput);

        var orderUpdated = _uof.OrderRepository.Update(orderForUpdate);
        
        // remove old data from cache
        await _cacheService.RemoveAsync($"order-{id}"); 
        
        await _uof.CommitChanges();

        var orderDtoUpdated = _mapper.Map<OrderDTOOutput>(orderUpdated);

        return Result<OrderDTOOutput>.Success(orderDtoUpdated);
    }

    public async Task<Result<OrderDTOOutput>> DeleteOrder(int? id)
    {
        var order = await _uof.OrderRepository.GetAsync(o => o.OrderId == id);
        
        if (order is null)
        {
            return Result<OrderDTOOutput>.Failure(OrderErrors.NotFound);
        }

        var orderDeleted = _uof.OrderRepository.Delete(order);
        
        // remove old data from cache
        await _cacheService.RemoveAsync($"order-{id}"); 
        
        await _uof.CommitChanges();

        var orderDtoDeleted = _mapper.Map<OrderDTOOutput>(orderDeleted);

        return Result<OrderDTOOutput>.Success(orderDtoDeleted);
    }

    public async Task<Result<OrderDTOOutput>> CreateAndSendOrder(int userId, string? note = null)
    {
        var user = await _uof.UserRepository.GetByIdAsync(userId);

        if (user is null)
        {
            return Result<OrderDTOOutput>.Failure(UserErrors.NotFound);
        }
        
        // Get shoppingCart from database
        var shoppingCart = await _shoppingCartService.GetShoppingCartWithProductsCheckedAsync(userId);
        
        if(!shoppingCart.isSuccess)
            return Result<OrderDTOOutput>.Failure(shoppingCart.error);
        
        var orderToCreate = new OrderDTOInput(
            0,
            (decimal) shoppingCart.value.TotalValue,
            DateTime.Now, 
            Status.Preparing,
            user.Name,
            note,
            user.UserId
        );
        
        // Create order with values of shopping cart (TotalValue, UserId)
        var createdOrderResult = await CreateOrder(orderToCreate);
        
        if(!createdOrderResult.isSuccess)
            return Result<OrderDTOOutput>.Failure(createdOrderResult.error);
        
        var shoppingCartProductInfo = shoppingCart.value.ToShoppingCartProduct();
        
        // Add all checked products in order created
        var order = _mapper.Map<Order>(createdOrderResult);

        foreach (var product in shoppingCartProductInfo.Products)
        {
            order.LineItems.Add(new LineItem(
                order.OrderId,
                product.Product.ProductId,
                product.Amount.Value,
                product.Product.Value
                ));
        }

        var productsAddedToOrder = _uof.OrderRepository.Update(order);
        
        if(productsAddedToOrder is null)
            return Result<OrderDTOOutput>.Failure(OrderErrors.AddRangeError);
        
        // Remove products from ShoppingCart that were Checked
        var removeProductsRowsAffected = await _uof.ShoppingCartRepository
            .RemoveCheckedItemsFromShoppingCartAsync(shoppingCartProductInfo.ShoppingCart.ShoppingCartId);

        if(removeProductsRowsAffected == 0)
            return Result<OrderDTOOutput>.Failure(ShoppingCartError.RemoveCheckedItemsError);
        
        // Update TotalValue and ProductsCount from ShoppingCart
        shoppingCartProductInfo.ShoppingCart.
            DecreaseTotalValue((double)createdOrderResult.value.TotalValue);
        shoppingCartProductInfo.ShoppingCart
            .DecreaseProductCount(shoppingCartProductInfo.Products.Count);
        
        var shoppingCartUpdated = _uof.ShoppingCartRepository.Update(shoppingCartProductInfo.ShoppingCart);

        if (shoppingCartUpdated is null)
            return Result<OrderDTOOutput>.Failure(ShoppingCartError.UpdateTotalValueAndProductCountItemsError);
        
        // Sent Order
        var sendOrderResult = await SentOrder(createdOrderResult.value.OrderId, note);
        
        // remove old data from cache
        await _cacheService.RemoveAsync($"order-{order.OrderId}"); 
        
        if(!sendOrderResult.isSuccess)
            return sendOrderResult;
        
        return Result<OrderDTOOutput>.Success(sendOrderResult.value);
    }

    public async Task<Result<OrderDTOOutput>> SentOrder(int orderId, string? note = null)
    {
        var order = await _uof.OrderRepository.GetOrderWithProductsByOrderId(orderId);

        if (order is null)
        {
            return Result<OrderDTOOutput>.Failure(OrderErrors.NotFound);
        }
        
        if(order.LineItems.IsNullOrEmpty())
            return Result<OrderDTOOutput>.Failure(OrderErrors.ProductListEmpty);

        if (order.OrderStatus is Status.Sent)
            return Result<OrderDTOOutput>.Failure(OrderErrors.OrderFinishedOrSent);
        
        // Get all products and verify if still have all the products in Db
        var products = await VerifyProductsExist(orderId);
        
        // List with 0 elements mean that all the products exists and the order can be sent
        // List with 1 or more elements mean that these products does not exist

        if (products.Count > 0)
        {
            var productsUnavailable = products.Select(p => 
                new ValidationFailure(p.Name, "product unavailable"))
                .ToList();
            
            // return all the products that are unavailable 
            return Result<OrderDTOOutput>.Failure(OrderErrors.ProductsStockUnavailable, productsUnavailable);
        }
        
        order.SentOrder();
        order.ChangeNote(note);
        foreach (var li in order.LineItems) 
        {
            li.Product.DecreaseStockQuantity();
        }
        
        _uof.OrderRepository.Update(order);
        
        // remove old data from cache
        await _cacheService.RemoveAsync($"order-{order.OrderId}"); 
        
        await _uof.CommitChanges();
        
        return Result<OrderDTOOutput>.Success(_mapper.Map<OrderDTOOutput>(order));
    }

    public async Task<Result<OrderDTOOutput>> FinishOrder(int orderId)
    {
        var order = await _uof.OrderRepository.GetOrderWithProductsByOrderId(orderId);

        if (order is null)
        {
            return Result<OrderDTOOutput>.Failure(OrderErrors.NotFound);
        }

        if (order.OrderStatus is not Status.Sent && order.OrderStatus is Status.Preparing)
            return Result<OrderDTOOutput>.Failure(OrderErrors.OrderNotSent);
        
        if (order.OrderStatus is Status.Finished)
            return Result<OrderDTOOutput>.Failure(OrderErrors.OrderFinishedOrSent);
        
        order.FinishOrder();
        
        _uof.OrderRepository.Update(order);
        
        // remove old data from cache
        await _cacheService.RemoveAsync($"order-{order.OrderId}"); 
        
        await _uof.CommitChanges();
        
        return Result<OrderDTOOutput>.Success(_mapper.Map<OrderDTOOutput>(order));
    }

    public async Task<Result<OrderDTOOutput>> AddProduct(int orderId, int productId, decimal amount)
    {
        var order = await _uof.OrderRepository.GetOrderWithProductsByOrderId(orderId);
        
        if(order is null)
            return Result<OrderDTOOutput>.Failure(OrderErrors.NotFound);

        if (order.OrderStatus is Status.Finished or Status.Sent)
            return Result<OrderDTOOutput>.Failure(OrderErrors.OrderFinishedOrSent);
        
        var product = await _uof.ProductRepository.GetAsync(p => p.ProductId == productId);
        
        if(product is null)
            return Result<OrderDTOOutput>.Failure(ProductErrors.NotFound);

        if (product.StockQuantity <= 0)
            return Result<OrderDTOOutput>.Failure(ProductErrors.StockUnavailable);

        if (product.TypeValue is TypeValue.Uni)
            amount = Math.Truncate(amount);
        
        order.LineItems.Add(new LineItem(order.OrderId, product.ProductId, amount, product.Value));
        
        // Increase Order TotalValue
        order.IncreaseValue(product.Value * amount);
        _uof.OrderRepository.Update(order);
        
        // remove old data from cache
        await _cacheService.RemoveAsync($"order-{order.OrderId}"); 
        
        await _uof.CommitChanges();
        //order.Products.Add(product);

        return Result<OrderDTOOutput>.Success(_mapper.Map<OrderDTOOutput>(order));
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

    // Refactor
    public async Task<Result<OrderDTOOutput>> RemoveProduct(int orderId, int productId)
    {
        var order = await _uof.OrderRepository.GetOrderWithProductsByOrderId(orderId);
        
        if(order is null)
            return Result<OrderDTOOutput>.Failure(OrderErrors.NotFound);
        
        // Get the value and the amount of the product that`s gonna be removed.
        var lineItem = await _uof.OrderRepository.GetLineItemByOrderIdAndProductId(orderId, productId);
        
        if(lineItem is null)
            return Result<OrderDTOOutput>.Failure(OrderErrors.ProductNotFound);
        
        order.LineItems.Remove(lineItem);

        // Decrease Order TotalValue
        order.DecreaseValue(lineItem.Price * lineItem.Amount);
        _uof.OrderRepository.Update(order);
        
        // remove old data from cache
        await _cacheService.RemoveAsync($"order-{order.OrderId}"); 
        
        await _uof.CommitChanges();
        
        return Result<OrderDTOOutput>.Success(_mapper.Map<OrderDTOOutput>(order));
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
    
    public async Task<List<Product>> VerifyProductsExist(int orderId)
    {
        var products = await _uof.OrderRepository.GetProducts(orderId);
        
        return products.Where(p => p.StockQuantity == 0).ToList();
    }
}