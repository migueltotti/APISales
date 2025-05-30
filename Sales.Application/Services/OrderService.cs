using System.Linq.Expressions;
using System.Runtime.InteropServices.JavaScript;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.IdentityModel.Tokens;
using Sales.Application.DTOs.LineItemDTO;
using Sales.Application.DTOs.OrderDTO;
using Sales.Application.DTOs.ProductDTO;
using Sales.Application.DTOs.UserDTO;
using Sales.Application.DTOs.WorkDayDTO;
using Sales.Application.Interfaces;
using Sales.Application.Mapping.Extentions;
using Sales.Application.MassTransit.GenerateReport;
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
    private readonly IWorkDayService _workDayService;
    private readonly ISendBusMessage _sendBusMessage;
    

    public OrderService(IUnitOfWork uof, IShoppingCartService shoppingCartService, IValidator<OrderDTOInput> validator, IMapper mapper, IOrderFilterFactory orderFilterFactory, ICacheService cacheService, IWorkDayService workDayService, ISendBusMessage sendBusMessage)
    {
        _uof = uof;
        _shoppingCartService = shoppingCartService;
        _validator = validator;
        _mapper = mapper;
        _orderFilterFactory = orderFilterFactory;
        _cacheService = cacheService;
        _workDayService = workDayService;
        _sendBusMessage = sendBusMessage;
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
    
    public async Task<IPagedList<OrderDTOOutput>> GetAllOrdersWithProductsByDateTimeNow(OrderParameters parameters)
    {
        Status.TryParse(parameters.Status, out Status status);
        
        var ordersWithProducts = await 
            _uof.OrderRepository.GetAllOrdersWithProductsByTodayDate(status);
        
        var ordersWithProductsMap = _mapper.Map<IEnumerable<OrderDTOOutput>>(ordersWithProducts);
        
        return ordersWithProductsMap.ToPagedList(parameters.PageNumber, parameters.PageSize);
    }

    public async Task<Result<IPagedList<OrderWeekReportDTO>>> GetNumberOfOrdersFromTodayToLastSundays(OrderParameters parameters)
    {
        if (parameters.Since is null)
            return Result<IPagedList<OrderWeekReportDTO>>.Failure(OrderErrors.SinceNullParameter);
        
        var orders = await GetAllOrders();
        var lastSunday = GetLastSundayFromToday();
        var lastSundaysList = new List<DateTime>();
        for (int i = 0; i < parameters.Since.Value; i++)
        {
            lastSundaysList.Add(lastSunday.AddDays(-(i*7)));
        }
        
        var ordersFilteredFromNowToLastWeeks = orders.
            Where(o => lastSundaysList.Contains(o.OrderDate.Date));

        var orderWeekReport = new Dictionary<DateOnly, int>();
            
        foreach (var order in ordersFilteredFromNowToLastWeeks)
        {
            if (orderWeekReport.ContainsKey(DateOnly.FromDateTime(order.OrderDate)))
                orderWeekReport[DateOnly.FromDateTime(order.OrderDate)]++;
            else
                orderWeekReport.Add(DateOnly.FromDateTime(order.OrderDate), 1);
        }

        List<OrderWeekReportDTO> orderWeekReportList = [];
        orderWeekReportList.AddRange(orderWeekReport.
            Select(o => new OrderWeekReportDTO(o.Key, o.Value)));

        return Result<IPagedList<OrderWeekReportDTO>>.Success(orderWeekReportList.ToPagedList(parameters.PageNumber, parameters.PageSize));
    }
    
    public async Task<Result<List<NumberOfProductDTO>>> Get5BestSellingProductsByNumberOfMonths(ProductParameters parameters)
    {
        if (parameters.Months_Count.Equals(0) || parameters.Months_Count is null)
            return Result<List<NumberOfProductDTO>>.Failure(ProductErrors.MonthsCountNullParameter);

        var ordersFilteredByMonthsCount = await _uof.OrderRepository
            .GetAllOrdersWithProductsByLastMonths(parameters.Months_Count.Value);

        var numberOfProducts = new Dictionary<string, int>();

        foreach (var order in ordersFilteredByMonthsCount)
        {
            foreach (var item in order.LineItems)
            {
                if(!numberOfProducts.TryAdd(item.Product.Name, 1))
                    numberOfProducts[item.Product.Name]++;
            }
        }

        var bestSellingProducts = numberOfProducts.ToList()
            .OrderBy(p => p.Value)
            .Take(5)
            .Select(prod => new NumberOfProductDTO(prod.Key, prod.Value))
            .ToList();

        return Result<List<NumberOfProductDTO>>.Success(bestSellingProducts);
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

        Order order;
        if (orderDtoInput.Products.Count != 0)
        {
            var totalValue = orderDtoInput.Products.Sum(p => p.Price * p.Amount);
            
            order = new Order(
                totalValue,
                orderDtoInput.OrderDate,
                orderDtoInput.Holder,
                orderDtoInput.Note,
                orderDtoInput.UserId
            );
        }
        else
        {
            order = new Order(
                orderDtoInput.TotalValue,
                orderDtoInput.OrderDate,
                orderDtoInput.Holder,
                orderDtoInput.Note,
                orderDtoInput.UserId
            );
        }

        var orderCreated = _uof.OrderRepository.Create(order);
        
        // Update WorkDay data if still in progress
        await _workDayService.RegisterOrderToWorkDay();
        
        await _uof.CommitChanges();
        
        // Add Products if not empty
        if (orderDtoInput.Products.Count != 0)
        {
            var lineItems = orderDtoInput.Products
                .Select(li => new LineItem(
                    orderCreated.OrderId,
                    li.ProductId,
                    li.Amount,
                    li.Price
                )).ToList();
            
            orderCreated.AddProducts(lineItems);
            
            orderCreated = _uof.OrderRepository.Update(orderCreated);
            await _uof.CommitChanges();
        }
        
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
        if(id is null)
            return Result<OrderDTOOutput>.Failure(OrderErrors.DataIsNull);
        
        var order = await _uof.OrderRepository.GetOrderWithProductsByOrderId(id.Value);
        
        if (order is null)
            return Result<OrderDTOOutput>.Failure(OrderErrors.NotFound);
        
        // Update WorkDay data if still in progress
        // Verify if WorkDay is in progress and update StockQuantity of Products that we`re used in deleted order
        var workDayResult = await _workDayService.CancelOrderToWorkDay();

        if (workDayResult.isSuccess)
        {
            foreach (var orderLineItem in order.LineItems)
            {
                var prod = orderLineItem.Product;
                prod.IncreaseStockQuantity(orderLineItem.Amount);
                _uof.ProductRepository.Update(prod);
            }
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
            [],
            user.UserId
        );
        
        var validation = await _validator.ValidateAsync(orderToCreate);

        if (!validation.IsValid)
        {
            return Result<OrderDTOOutput>.Failure(OrderErrors.IncorrectFormatData, validation.Errors);
        }

        var order = _mapper.Map<Order>(orderToCreate);
        
        // Add all checked products in order created
        var shoppingCartProductInfo = shoppingCart.value.ToShoppingCartProduct();
        
        foreach (var product in shoppingCartProductInfo.Products)
        {
            order.LineItems.Add(new LineItem(
                order.OrderId,
                product.Product.ProductId,
                product.Amount.Value,
                product.Product.Value
            ));
        } 

        var orderCreated = _uof.OrderRepository.Create(order);
        
        await _uof.CommitChanges();
        
        if(orderCreated is null)
            return Result<OrderDTOOutput>.Failure(OrderErrors.AddRangeError);
        
        // Remove products from ShoppingCart that were Checked
        var removeProductsRowsAffected = await _uof.ShoppingCartRepository
            .RemoveCheckedItemsFromShoppingCartAsync(shoppingCartProductInfo.ShoppingCart.ShoppingCartId);

        if(removeProductsRowsAffected == 0)
            return Result<OrderDTOOutput>.Failure(ShoppingCartError.RemoveCheckedItemsError);
        
        // Update TotalValue and ProductsCount from ShoppingCart
        shoppingCartProductInfo.ShoppingCart.
            DecreaseTotalValue((double)orderCreated.TotalValue);
        shoppingCartProductInfo.ShoppingCart
            .DecreaseProductCount(shoppingCartProductInfo.Products.Count);
        
        var shoppingCartUpdated = _uof.ShoppingCartRepository.Update(shoppingCartProductInfo.ShoppingCart);

        if (shoppingCartUpdated is null)
            return Result<OrderDTOOutput>.Failure(ShoppingCartError.UpdateTotalValueAndProductCountItemsError);

        await _uof.CommitChanges();
        
        // Sent Order
        var sendOrderResult = await SentOrder(orderCreated.OrderId, note);
        
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
        if(note is not null)
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

    public async Task<Result<OrderReportDTO>> GetOrderReport(DateTime? date)
    {
        if (date is null)
            return Result<OrderReportDTO>.Failure(OrderErrors.DateNullParameter);
        
        var orders = await _uof.OrderRepository.GetAllOrdersWithProductsByDate(date.Value);

        return Result<OrderReportDTO>.Success(new OrderReportDTO(
            Date: date.Value,
            OrdersCount: orders.Count(),
            TotalValue: orders.Sum(o => o.TotalValue),
            Orders: _mapper.Map<List<OrderDTOOutput>>(orders)
        ));
    }
    
    public async Task<Result<object>> GenerateOrderReport(DateTime? date, ReportType reportType, string emailDest)
    {
        if (date is null || string.IsNullOrEmpty(emailDest) || string.IsNullOrWhiteSpace(emailDest))
            return Result<object>.Failure(OrderErrors.DateNullParameter);
        
        if(!Regex.Match(emailDest, @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$").Success)
            return Result<object>.Failure(OrderErrors.InvalidEmail);
        
        // get orderReport
        var orderReport = await GetOrderReport(date);
        
        if(!orderReport.isSuccess)
            return Result<object>.Failure(orderReport.error);
        
        // get workDay if reportType is POS
        Result<WorkDayDTOOutput> workDayReport;
        if (reportType.ToString().Contains("POS"))
        {
            workDayReport = await _workDayService.GetWorkDayByDateAsync(date.Value);
            // workDayReport = Result<WorkDayDTOOutput>.Success(new WorkDayDTOOutput(
            //     0, 
            //     0, 
            //     "",
            //     new UserDTOOutput(0, "", "", "", 0, DateTime.Now, 0, Role.Customer),
            //     DateTime.Now, DateTime.Now.AddMinutes(10),
            //     0,
            //     0
            // ));
            
            if(!workDayReport.isSuccess)
                return Result<object>.Failure(workDayReport.error);
        }
        else
        {
            workDayReport = Result<WorkDayDTOOutput>.Success(new WorkDayDTOOutput(
                0, 
                0, 
                "",
                new UserDTOOutput(0, "", "", "", 0, DateTime.Now, 0, Role.Customer),
                DateTime.Now, DateTime.Now.AddMinutes(10),
                0,
                0
            ));
        }

        await _sendBusMessage.SendAsync(new GeneratePOSReportEvent(
                Guid.NewGuid(),
                orderReport.value,
                workDayReport.value,
                reportType,
                emailDest
        ));

        return Result<object>.Success(
            new { message = "Report sent successfully!" }   
        );
    }
    
    public async Task<List<Product>> VerifyProductsExist(int orderId)
    {
        var products = await _uof.OrderRepository.GetProducts(orderId);
        
        return products.Where(p => p.StockQuantity == 0).ToList();
    }

    private DateTime GetLastSundayFromToday()
    {
        DateTime lastSunday = DateTime.Today.DayOfWeek switch
        {
            DayOfWeek.Sunday => DateTime.Today.Date,
            DayOfWeek.Monday => DateTime.Today.AddDays(-1).Date,
            DayOfWeek.Tuesday => DateTime.Today.AddDays(-2).Date,
            DayOfWeek.Wednesday => DateTime.Today.AddDays(-3).Date,
            DayOfWeek.Thursday => DateTime.Today.AddDays(-4).Date,
            DayOfWeek.Friday => DateTime.Today.AddDays(-5).Date,
            DayOfWeek.Saturday => DateTime.Today.AddDays(-6).Date
        };

        return lastSunday;
    }
}