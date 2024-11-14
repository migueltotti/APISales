using System.Linq.Expressions;
using System.Net;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Sales.API.Controllers;
using Sales.Application.DTOs.OrderDTO;
using Sales.Application.DTOs.ProductDTO;
using Sales.Application.Interfaces;
using Sales.Application.Parameters;
using Sales.Application.Parameters.ModelsParameters;
using Sales.Application.ResultPattern;
using Sales.Domain.Models;
using X.PagedList.Extensions;

namespace Sales.Test.ControllersTests;

public class OrdersControllerTest
{
    private readonly IOrderService _mockOrderService;
    private readonly OrdersController _orderController;
    private readonly ILogger<OrdersController> _mockLogger;
    private readonly Fixture _fixture;

    public OrdersControllerTest()
    {
        _mockOrderService = Substitute.For<IOrderService>();
        _mockLogger = Substitute.For<ILogger<OrdersController>>();
        _fixture = new Fixture();

        _orderController = new OrdersController(_mockOrderService, _mockLogger)
        {
            ControllerContext =
            {
                HttpContext = new DefaultHttpContext()
            }
        };
    }
    
    [Fact]
    public async Task Get_ShouldReturn200OkObjectResultWithAllOrders()
    {   
        // Arrange
        var orders = _fixture.CreateMany<OrderDTOOutput>(3).ToPagedList();
        var parameters = new QueryStringParameters();
        
        _mockOrderService.GetAllOrders(Arg.Any<QueryStringParameters>()).Returns(orders);
        
        // Act
        var result = await _orderController.Get(parameters);
        var obj = result.Result as ObjectResult;
        
        // Assert
        result.Result.Should().NotBeNull();
        result.Should().BeOfType<ActionResult<IEnumerable<OrderDTOOutput>>>();
        result.Result.Should().BeOfType<OkObjectResult>()
            .Which.StatusCode.Should().Be(200);
        obj.Value.Should().BeEquivalentTo(orders.ToList());

        var httpContext = _orderController.ControllerContext.HttpContext;
        httpContext.Response.Headers.ContainsKey("X-Pagination").Should().BeTrue();
        await _mockOrderService.Received(1).GetAllOrders(Arg.Any<QueryStringParameters>());
    }

    [Fact]
    public async Task Get_ShouldReturn200OkObjectResultWithOrdersFromUserPassedByUserId()
    {   
        // Arrange
        var orders = _fixture.CreateMany<OrderDTOOutput>(3).ToPagedList();
        var parameters = new OrderParameters();
        var userId = _fixture.Create<int>();
        
        _mockOrderService.GetOrdersByUserId(Arg.Any<int>(), Arg.Any<OrderParameters>()).Returns(orders);
        
        // Act
        var result = await _orderController.GetOrdersByUserId(userId, parameters);
        var obj = result.Result as ObjectResult;
        
        // Assert
        result.Result.Should().NotBeNull();
        result.Should().BeOfType<ActionResult<IEnumerable<OrderDTOOutput>>>();
        result.Result.Should().BeOfType<OkObjectResult>()
            .Which.StatusCode.Should().Be(200);
        obj.Value.Should().BeEquivalentTo(orders.ToList());

        var httpContext = _orderController.ControllerContext.HttpContext;
        httpContext.Response.Headers.ContainsKey("X-Pagination").Should().BeTrue();
        await _mockOrderService.Received(1).GetOrdersByUserId(Arg.Any<int>(), Arg.Any<OrderParameters>());
    }
    
    [Fact]
    public async Task Get_ShouldReturn200OkObjectResultWithOrdersMatchingValueAndValueCriteria()
    {   
        // Arrange
        var orders = _fixture.CreateMany<OrderDTOOutput>(3).ToPagedList();
        var parameters = new OrderParameters();
        
        _mockOrderService.GetOrdersWithFilter("value", Arg.Any<OrderParameters>()).Returns(orders);
        // Act
        var result = await _orderController.GetOrdersByValue(parameters);
        var obj = result.Result as ObjectResult;
        
        // Assert
        result.Result.Should().NotBeNull();
        result.Should().BeOfType<ActionResult<IEnumerable<OrderDTOOutput>>>();
        result.Result.Should().BeOfType<OkObjectResult>()
            .Which.StatusCode.Should().Be(200);
        obj.Value.Should().BeEquivalentTo(orders.ToList());

        var httpContext = _orderController.ControllerContext.HttpContext;
        httpContext.Response.Headers.ContainsKey("X-Pagination").Should().BeTrue();
        await _mockOrderService.Received(1).GetOrdersWithFilter("value", Arg.Any<OrderParameters>());
    }
    
    [Fact]
    public async Task Get_ShouldReturn200OkObjectResultWithOrdersMatchingStartDayAndEndDate()
    {   
        // Arrange
        var orders = _fixture.CreateMany<OrderDTOOutput>(3).ToPagedList();
        var parameters = new OrderParameters();
        
        _mockOrderService.GetOrdersWithFilter("date", Arg.Any<OrderParameters>()).Returns(orders);
        // Act
        var result = await _orderController.GetOrdersByDate(parameters);
        var obj = result.Result as ObjectResult;
        
        // Assert
        result.Result.Should().NotBeNull();
        result.Should().BeOfType<ActionResult<IEnumerable<OrderDTOOutput>>>();
        result.Result.Should().BeOfType<OkObjectResult>()
            .Which.StatusCode.Should().Be(200);
        obj.Value.Should().BeEquivalentTo(orders.ToList());

        var httpContext = _orderController.ControllerContext.HttpContext;
        httpContext.Response.Headers.ContainsKey("X-Pagination").Should().BeTrue();
        await _mockOrderService.Received(1).GetOrdersWithFilter("date", Arg.Any<OrderParameters>());
    }
    
    [Fact]
    public async Task Get_ShouldReturn200OkObjectResultWithOrderThatHaveTheSpecifiedProduct()
    {   
        // Arrange
        var orders = _fixture.CreateMany<OrderDTOOutput>(3).ToPagedList();
        var parameters = new OrderParameters();
        
        _mockOrderService.GetOrdersByProduct(Arg.Any<OrderParameters>()).Returns(orders);
        // Act
        var result = await _orderController.GetOrdersByProduct(parameters);
        var obj = result.Result as ObjectResult;
        
        // Assert
        result.Result.Should().NotBeNull();
        result.Should().BeOfType<ActionResult<IEnumerable<OrderDTOOutput>>>();
        result.Result.Should().BeOfType<OkObjectResult>()
            .Which.StatusCode.Should().Be(200);
        obj.Value.Should().BeEquivalentTo(orders.ToList());

        var httpContext = _orderController.ControllerContext.HttpContext;
        httpContext.Response.Headers.ContainsKey("X-Pagination").Should().BeTrue();
        await _mockOrderService.Received(1).GetOrdersByProduct(Arg.Any<OrderParameters>());
    }
    
    [Fact]
    public async Task Get_ShouldReturn200OkObjectResultWithOrdersMatchingSpecifiedStatus()
    {   
        // Arrange
        var orders = _fixture.CreateMany<OrderDTOOutput>(3).ToPagedList();
        var parameters = new OrderParameters();
        
        _mockOrderService.GetOrdersWithFilter("status", Arg.Any<OrderParameters>()).Returns(orders);
        // Act
        var result = await _orderController.GetOrdersByStatus(parameters);
        var obj = result.Result as ObjectResult;
        
        // Assert
        result.Result.Should().NotBeNull();
        result.Should().BeOfType<ActionResult<IEnumerable<OrderDTOOutput>>>();
        result.Result.Should().BeOfType<OkObjectResult>()
            .Which.StatusCode.Should().Be(200);
        obj.Value.Should().BeEquivalentTo(orders.ToList());

        var httpContext = _orderController.ControllerContext.HttpContext;
        httpContext.Response.Headers.ContainsKey("X-Pagination").Should().BeTrue();
        await _mockOrderService.Received(1).GetOrdersWithFilter("status", Arg.Any<OrderParameters>());
    }
    
    [Fact]
    public async Task Get_ShouldReturn200OkObjectResultWithOrdersFromUsersThatHaveTheSpecifiedAffiliateId()
    {   
        // Arrange
        var orders = _fixture.CreateMany<OrderDTOOutput>(3).ToPagedList();
        var parameters = new OrderParameters();
        var affiliateId = _fixture.Create<int>();
        
        _mockOrderService.GetOrdersByAffiliateId(Arg.Any<int>(), Arg.Any<OrderParameters>()).Returns(orders);
        // Act
        var result = await _orderController.GetOrdersByAffiliate(affiliateId, parameters);
        var obj = result.Result as ObjectResult;
        
        // Assert
        result.Result.Should().NotBeNull();
        result.Should().BeOfType<ActionResult<IEnumerable<OrderDTOOutput>>>();
        result.Result.Should().BeOfType<OkObjectResult>()
            .Which.StatusCode.Should().Be(200);
        obj.Value.Should().BeEquivalentTo(orders.ToList());

        var httpContext = _orderController.ControllerContext.HttpContext;
        httpContext.Response.Headers.ContainsKey("X-Pagination").Should().BeTrue();
        await _mockOrderService.Received(1).GetOrdersByAffiliateId(Arg.Any<int>(), Arg.Any<OrderParameters>());
    }

    [Fact]
    public async Task GetOrder_ShouldReturn200OkObjectResultWithOrders_WhenOrderExists()
    {
        // Arrange
        var order = _fixture.Create<OrderDTOOutput>();
        var orderId = _fixture.Create<int>();
        var orderResult = Result<OrderDTOOutput>.Success(order);
        
        _mockOrderService.GetOrderById(Arg.Any<int>())
            .Returns(orderResult);
        
        // Act
        var result = await _orderController.Get(orderId);
        var obj = result.Result as ObjectResult;

        // Assert
        result.Result.Should().NotBeNull();
        result.Should().BeOfType<ActionResult<OrderDTOOutput>>();
        result.Result.Should().BeOfType<OkObjectResult>()
            .Which.StatusCode.Should().Be(200);
        obj.Value.Should().BeEquivalentTo(orderResult.value);

        await _mockOrderService.Received(1).GetOrderById(Arg.Any<int>());
    }
    
    [Fact]
    public async Task GetOrder_ShouldReturn404NotFoundObjectResultWithErrorResponse_WhenOrderDoesNotExists()
    {
        // Arrange
        var orderId = _fixture.Create<int>();
        var orderResult = Result<OrderDTOOutput>.Failure(OrderErrors.NotFound);
        
        _mockOrderService.GetOrderById(Arg.Any<int>())
            .Returns(orderResult);
        
        // Act
        var result = await _orderController.Get(orderId);
        var obj = result.Result as ObjectResult;

        // Assert
        result.Result.Should().NotBeNull();
        result.Should().BeOfType<ActionResult<OrderDTOOutput>>();
        result.Result.Should().BeOfType<NotFoundObjectResult>()
            .Which.StatusCode.Should().Be(404);
        obj.Value.Should().BeEquivalentTo(orderResult.GenerateErrorResponse());

        await _mockOrderService.Received(1).GetOrderById(Arg.Any<int>());
    }

    [Fact]
    public async Task Post_ShouldReturn201CreatedAtRouteResultWithOrder_WhenOrderCreatedSuccessfully()
    {
        // Arrange
        var orderInput = _fixture.Create<OrderDTOInput>();
        var order = _fixture.Create<OrderDTOOutput>();
        var orderResult = Result<OrderDTOOutput>.Success(order);

        _mockOrderService.CreateOrder(Arg.Any<OrderDTOInput>()).Returns(orderResult);

        // Act
        var result = await _orderController.Post(orderInput);
        var obj = result.Result as ObjectResult;

        // Assert
        result.Result.Should().NotBeNull();
        result.Should().BeOfType<ActionResult<OrderDTOOutput>>();
        result.Result.Should().BeOfType<CreatedAtRouteResult>()
            .Which.StatusCode.Should().Be(201);
        obj.Value.Should().BeEquivalentTo(orderResult.value);
        
        await _mockOrderService.Received(1).CreateOrder(Arg.Any<OrderDTOInput>());
    }
    
    [Fact]
    public async Task Post_ShouldReturn400BadRequestResultWithErrorResponse_WhenOrderCreatedWithErrors()
    {
        // Arrange
        var orderInput = _fixture.Create<OrderDTOInput>();
        var error = _fixture.Create<Error>();
        var orderResult = Result<OrderDTOOutput>.Failure(error);

        _mockOrderService.CreateOrder(Arg.Any<OrderDTOInput>()).Returns(orderResult);

        // Act
        var result = await _orderController.Post(orderInput);
        var obj = result.Result as ObjectResult;

        // Assert
        result.Result.Should().NotBeNull();
        result.Should().BeOfType<ActionResult<OrderDTOOutput>>();
        result.Result.Should().BeOfType<BadRequestObjectResult>()
            .Which.StatusCode.Should().Be(400);
        obj.Value.Should().BeEquivalentTo(orderResult.GenerateErrorResponse());
        
        await _mockOrderService.Received(1).CreateOrder(Arg.Any<OrderDTOInput>());
    }

    [Fact]
    public async Task Put_ShouldReturn200OkResultWithSuccessResponse_WhenOrderUpdatedSuccessfully()
    {
        // Arrange
        var orderInput = _fixture.Create<OrderDTOInput>();
        var orderId = orderInput.OrderId;
        var orderOutput = _fixture.Create<OrderDTOOutput>();
        var orderResult = Result<OrderDTOOutput>.Success(orderOutput);
        
        _mockOrderService.UpdateOrder(Arg.Any<OrderDTOInput>(), Arg.Any<int>())
            .Returns(orderResult);

        // Act
        var result = await _orderController.Put(orderId, orderInput);
        var obj = result.Result as ObjectResult;

        // Assert
        result.Result.Should().NotBeNull();
        result.Should().BeOfType<ActionResult<OrderDTOOutput>>();
        result.Result.Should().BeOfType<OkObjectResult>()
            .Which.StatusCode.Should().Be(200);
        obj.Value.Should().BeEquivalentTo($"Order with id = {orderResult.value.OrderId} has been update successfully");
        
        await _mockOrderService.Received(1).UpdateOrder(Arg.Any<OrderDTOInput>(), Arg.Any<int>());
    }
    
    [Fact]
    public async Task Put_ShouldReturn404NotFoundResultWithErrorResponse_WhenOrderUpdatedSuccessfully()
    {
        // Arrange
        var orderInput = _fixture.Create<OrderDTOInput>();
        var orderId = orderInput.OrderId;
        var error = new Error("NotFoundError", "NotFoundError", HttpStatusCode.NotFound);
        var orderResult = Result<OrderDTOOutput>.Failure(error);
        
        _mockOrderService.UpdateOrder(Arg.Any<OrderDTOInput>(), Arg.Any<int>())
            .Returns(orderResult);

        // Act
        var result = await _orderController.Put(orderId, orderInput);
        var obj = result.Result as ObjectResult;

        // Assert
        result.Result.Should().NotBeNull();
        result.Should().BeOfType<ActionResult<OrderDTOOutput>>();
        result.Result.Should().BeOfType<NotFoundObjectResult>()
            .Which.StatusCode.Should().Be(404);
        obj.Value.Should().BeEquivalentTo(orderResult.GenerateErrorResponse());
            
        await _mockOrderService.Received(1).UpdateOrder(Arg.Any<OrderDTOInput>(), Arg.Any<int>());
    }
    
    [Fact]
    public async Task Put_ShouldReturn400BadRequestResultWithErrorResponse_WhenOrderUpdatedSWithErrors()
    {
        // Arrange
        var orderInput = _fixture.Create<OrderDTOInput>();
        var orderId = orderInput.OrderId;
        var error = new Error("BadRequestError", "BadRequestError", HttpStatusCode.BadRequest);
        var orderResult = Result<OrderDTOOutput>.Failure(error);
        
        _mockOrderService.UpdateOrder(Arg.Any<OrderDTOInput>(), Arg.Any<int>())
            .Returns(orderResult);

        // Act
        var result = await _orderController.Put(orderId, orderInput);
        var obj = result.Result as ObjectResult;

        // Assert
        result.Result.Should().NotBeNull();
        result.Should().BeOfType<ActionResult<OrderDTOOutput>>();
        result.Result.Should().BeOfType<BadRequestObjectResult>()
            .Which.StatusCode.Should().Be(400);
        obj.Value.Should().BeEquivalentTo(orderResult.GenerateErrorResponse());
        
        await _mockOrderService.Received(1).UpdateOrder(Arg.Any<OrderDTOInput>(), Arg.Any<int>());
    }

    [Fact]
    public async Task Delete_ShouldReturn200OkObjectResultWithSuccessResponse_WhenOrderDeletedSuccessfully()
    {
        // Arrange
        var order = _fixture.Create<OrderDTOOutput>();
        var orderId = order.OrderId;
        var orderResult = Result<OrderDTOOutput>.Success(order);

        _mockOrderService.DeleteOrder(Arg.Any<int>()).Returns(orderResult);
        
        // Act
        var result = await _orderController.Delete(orderId);
        var obj = result.Result as ObjectResult;
        
        // Assert
        result.Result.Should().NotBeNull();
        result.Should().BeOfType<ActionResult<OrderDTOOutput>>();
        result.Result.Should().BeOfType<OkObjectResult>()
            .Which.StatusCode.Should().Be(200);
        obj.Value.Should().BeEquivalentTo($"Order with id = {orderResult.value.OrderId} has been deleted successfully");
        
        await _mockOrderService.Received(1).DeleteOrder(Arg.Any<int>());
    }
    
    [Fact]
    public async Task Delete_ShouldReturn404NotFoundResultWithErrorResponse_WhenOrderIsInvalid()
    {
        // Arrange
        var orderId = _fixture.Create<int>();
        var error = _fixture.Create<Error>();
        var orderResult = Result<OrderDTOOutput>.Failure(error);

        _mockOrderService.DeleteOrder(Arg.Any<int>()).Returns(orderResult);
        
        // Act
        var result = await _orderController.Delete(orderId);
        var obj = result.Result as ObjectResult;
        
        // Assert
        result.Result.Should().NotBeNull();
        result.Should().BeOfType<ActionResult<OrderDTOOutput>>();
        result.Result.Should().BeOfType<NotFoundObjectResult>()
            .Which.StatusCode.Should().Be(404);
        obj.Value.Should().BeEquivalentTo(orderResult.GenerateErrorResponse());
        
        await _mockOrderService.Received(1).DeleteOrder(Arg.Any<int>());
    }
    
    [Fact]
    public async Task GetProducts_ShouldReturn200OkObjectResultWithProductsOfSpecifiedOrder_WhenOrderIsValid()
    {
        // Arrange
        var products = _fixture.CreateMany<ProductDTOOutput>(3);
        var orderId = _fixture.Create<int>();
        var orderResult = Result<IEnumerable<ProductDTOOutput>>.Success(products);

        _mockOrderService.GetProductsByOrderId(Arg.Any<int>()).Returns(orderResult);
        
        // Act
        var result = await _orderController.GetProducts(orderId);
        var obj = result.Result as ObjectResult;
        
        // Assert
        result.Result.Should().NotBeNull();
        result.Should().BeOfType<ActionResult<IEnumerable<ProductDTOOutput>>>();
        result.Result.Should().BeOfType<OkObjectResult>()
            .Which.StatusCode.Should().Be(200);
        obj.Value.Should().BeEquivalentTo(orderResult.value);
        
        await _mockOrderService.Received(1).GetProductsByOrderId(Arg.Any<int>());
    }
    
    [Fact]
    public async Task GetProducts_ShouldReturn404NotFoundResultWithErrorResponse_WhenOrderIsInvalid()
    {
        // Arrange
        var error = _fixture.Create<Error>();
        var orderId = _fixture.Create<int>();
        var orderResult = Result<IEnumerable<ProductDTOOutput>>.Failure(error);

        _mockOrderService.GetProductsByOrderId(Arg.Any<int>()).Returns(orderResult);
        
        // Act
        var result = await _orderController.GetProducts(orderId);
        var obj = result.Result as ObjectResult;
        
        // Assert
        result.Result.Should().NotBeNull();
        result.Should().BeOfType<ActionResult<IEnumerable<ProductDTOOutput>>>();
        result.Result.Should().BeOfType<NotFoundObjectResult>()
            .Which.StatusCode.Should().Be(404);
        obj.Value.Should().BeEquivalentTo(orderResult.GenerateErrorResponse());
        
        await _mockOrderService.Received(1).GetProductsByOrderId(Arg.Any<int>());
    }

    [Fact]
    public async Task GetOrderReport_ShouldReturn200OkObjectResultWithOrderReport_WhenOrdersThatMatcherSpecifiedDateExists()
    {
        // Arrange
        var ordersReport = _fixture.Create<OrderReportDTO>();
        var initialDate = DateTime.Now;
        var finalDate = DateTime.Now.AddMonths(1);
        
        _mockOrderService.GetOrderReport(Arg.Any<DateTime>(), Arg.Any<DateTime>()).Returns(ordersReport);
        
        // Act
        var result = await _orderController.GetOrderReport(initialDate, finalDate);
        var obj = result.Result as ObjectResult;
        
        // Assert
        result.Result.Should().NotBeNull();
        result.Should().BeOfType<ActionResult<OrderReportDTO>>();
        result.Result.Should().BeOfType<OkObjectResult>()
            .Which.StatusCode.Should().Be(200);
        obj.Value.Should().BeEquivalentTo(ordersReport);

        await _mockOrderService.Received(1).GetOrderReport(Arg.Any<DateTime>(), Arg.Any<DateTime>());
    }

    [Fact]
    public async Task AddProduct_ShouldReturn200OkObjectResultWithProductAddedResponse_WhenOrderIsValid()
    {
        // Arrange
        var orderId = _fixture.Create<int>();
        var productId = _fixture.Create<int>();
        var amount = _fixture.Create<decimal>();
        var order = _fixture.Create<OrderProductsDTO>();
        var orderResult = Result<OrderProductsDTO>.Success(order);
        
        _mockOrderService.AddProduct(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<decimal>())
            .Returns(orderResult);
        
        // Act
        var result = await _orderController.AddProduct(orderId, productId, amount);
        var obj = result.Result as ObjectResult;
        
        // Assert
        result.Result.Should().NotBeNull();
        result.Should().BeOfType<ActionResult<OrderDTOOutput>>();
        result.Result.Should().BeOfType<OkObjectResult>()
            .Which.StatusCode.Should().Be(200);
        obj.Value.Should().BeEquivalentTo($"Product with id = {productId} was added successfully on Order with id = {orderId}");

        await _mockOrderService.Received(1).AddProduct(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<decimal>());
    }
    
    [Fact]
    public async Task AddProduct_ShouldReturn404NotFoundResultWithErrorResponse_WhenOrderOrProductAreInvalid()
    {
        // Arrange
        var orderId = _fixture.Create<int>();
        var productId = _fixture.Create<int>();
        var amount = _fixture.Create<decimal>();
        var error = new Error("NotFoundError", "NotFoundError", HttpStatusCode.NotFound);
        var orderResult = Result<OrderProductsDTO>.Failure(error);
        
        _mockOrderService.AddProduct(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<decimal>())
            .Returns(orderResult);
        
        // Act
        var result = await _orderController.AddProduct(orderId, productId, amount);
        var obj = result.Result as ObjectResult;
        
        // Assert
        result.Result.Should().NotBeNull();
        result.Should().BeOfType<ActionResult<OrderDTOOutput>>();
        result.Result.Should().BeOfType<NotFoundObjectResult>()
            .Which.StatusCode.Should().Be(404);
        obj.Value.Should().BeEquivalentTo(orderResult.GenerateErrorResponse());
        
        await _mockOrderService.Received(1).AddProduct(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<decimal>());
    }
    
    [Fact]
    public async Task AddProduct_ShouldReturn400BadRequestResultWithProductAddedResponse_WhenOrderOrProductAreValid()
    {
        // Arrange
        var orderId = _fixture.Create<int>();
        var productId = _fixture.Create<int>();
        var amount = _fixture.Create<decimal>();
        var error = new Error("BadRequestError", "BadRequestError", HttpStatusCode.BadRequest);
        var orderResult = Result<OrderProductsDTO>.Failure(error);
        
        _mockOrderService.AddProduct(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<decimal>())
            .Returns(orderResult);
        
        // Act
        var result = await _orderController.AddProduct(orderId, productId, amount);
        var obj = result.Result as ObjectResult;
        
        // Assert
        result.Result.Should().NotBeNull();
        result.Should().BeOfType<ActionResult<OrderDTOOutput>>();
        result.Result.Should().BeOfType<BadRequestObjectResult>()
            .Which.StatusCode.Should().Be(400);
        obj.Value.Should().BeEquivalentTo(orderResult.GenerateErrorResponse());
            
        await _mockOrderService.Received(1).AddProduct(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<decimal>());
    }
    
    [Fact]
    public async Task RemoveProduct_ShouldReturn200OkObjectResultWithSuccessResponse_WhenOrderOrProductAreInvalid()
    {
        // Arrange
        var productId = _fixture.Create<int>();
        var orderId = _fixture.Create<int>();
        var order = _fixture.Create<OrderProductsDTO>();
        var orderResult = Result<OrderProductsDTO>.Success(order);

        _mockOrderService.RemoveProduct(Arg.Any<int>(), Arg.Any<int>()).Returns(orderResult);
        
        // Act
        var result = await _orderController.RemoveProduct(orderId, productId);
        var obj = result.Result as ObjectResult;
        
        // Assert
        result.Result.Should().NotBeNull();
        result.Should().BeOfType<ActionResult<OrderProductsDTO>>();
        result.Result.Should().BeOfType<OkObjectResult>()
            .Which.StatusCode.Should().Be(200);
        obj.Value.Should().BeEquivalentTo($"Product with id = {productId} was removed from Order with id = {orderId} successfully");
        
        await _mockOrderService.Received(1).RemoveProduct(Arg.Any<int>(), Arg.Any<int>());
    }
    
    [Fact]
    public async Task RemoveProduct_ShouldReturn404NotFoundResultWithErrorResponse_WhenOrderOrProductAreInvalid()
    {
        // Arrange
        var productId = _fixture.Create<int>();
        var orderId = _fixture.Create<int>();
        var error = _fixture.Create<Error>();
        var orderResult = Result<OrderProductsDTO>.Failure(error);

        _mockOrderService.RemoveProduct(Arg.Any<int>(), Arg.Any<int>()).Returns(orderResult);
        
        // Act
        var result = await _orderController.RemoveProduct(orderId, productId);
        var obj = result.Result as ObjectResult;
        
        // Assert
        result.Result.Should().NotBeNull();
        result.Should().BeOfType<ActionResult<OrderProductsDTO>>();
        result.Result.Should().BeOfType<NotFoundObjectResult>()
            .Which.StatusCode.Should().Be(404);
        obj.Value.Should().BeEquivalentTo(orderResult.GenerateErrorResponse());
            
        await _mockOrderService.Received(1).RemoveProduct(Arg.Any<int>(), Arg.Any<int>());
    }
    
    [Fact]
    public async Task SentOrder_ShouldReturn200OkObjectResultWithSuccessResponse_WhenOrderIsSentSuccessfully()
    {
        // Arrange
        var orderId = _fixture.Create<int>();
        var order = _fixture.Create<OrderDTOOutput>();
        var orderResult = Result<OrderDTOOutput>.Success(order);

        _mockOrderService.SentOrder(Arg.Any<int>()).Returns(orderResult);
        
        // Act
        var result = await _orderController.SentOrder(orderId);
        var obj = result.Result as ObjectResult;
        
        // Assert
        result.Result.Should().NotBeNull();
        result.Should().BeOfType<ActionResult<OrderDTOOutput>>();
        result.Result.Should().BeOfType<OkObjectResult>()
            .Which.StatusCode.Should().Be(200);
        obj.Value.Should().BeEquivalentTo($"Order with id = {orderResult.value.OrderId} sent successfully");
        
        await _mockOrderService.Received(1).SentOrder(Arg.Any<int>());
    }
    
    [Fact]
    public async Task SentOrder_ShouldReturn404NotFoundResultWithErrorResponse_WhenOrderIsSentWithErrors()
    {
        // Arrange
        var orderId = _fixture.Create<int>();
        var error = _fixture.Create<Error>();
        var orderResult = Result<OrderDTOOutput>.Failure(error);

        _mockOrderService.SentOrder(Arg.Any<int>()).Returns(orderResult);
        
        // Act
        var result = await _orderController.SentOrder(orderId);
        var obj = result.Result as ObjectResult;
        
        // Assert
        result.Result.Should().NotBeNull();
        result.Should().BeOfType<ActionResult<OrderDTOOutput>>();
        result.Result.Should().BeOfType<NotFoundObjectResult>()
            .Which.StatusCode.Should().Be(404);
        obj.Value.Should().BeEquivalentTo(orderResult.GenerateErrorResponse());
            
        await _mockOrderService.Received(1).SentOrder(Arg.Any<int>());
    }
    
    [Fact]
    public async Task FinishOrder_ShouldReturn200OkObjectResultWithSuccessResponse_WhenOrderIsFinishedSuccessfully()
    {
        // Arrange
        var orderId = _fixture.Create<int>();
        var order = _fixture.Create<OrderDTOOutput>();
        var orderResult = Result<OrderDTOOutput>.Success(order);

        _mockOrderService.FinishOrder(Arg.Any<int>()).Returns(orderResult);
        
        // Act
        var result = await _orderController.FinishOrder(orderId);
        var obj = result.Result as ObjectResult;
        
        // Assert
        result.Result.Should().NotBeNull();
        result.Should().BeOfType<ActionResult<OrderDTOOutput>>();
        result.Result.Should().BeOfType<OkObjectResult>()
            .Which.StatusCode.Should().Be(200);
        obj.Value.Should().BeEquivalentTo($"Order with id = {orderResult.value.OrderId} finished successfully");
        
        await _mockOrderService.Received(1).FinishOrder(Arg.Any<int>());
    }
    
    [Fact]
    public async Task FinishOrder_ShouldReturn404NotFoundResultWithErrorResponse_WhenOrderIsFinishedWithErrors()
    {
        // Arrange
        var orderId = _fixture.Create<int>();
        var error = _fixture.Create<Error>();
        var orderResult = Result<OrderDTOOutput>.Failure(error);

        _mockOrderService.FinishOrder(Arg.Any<int>()).Returns(orderResult);
        
        // Act
        var result = await _orderController.FinishOrder(orderId);
        var obj = result.Result as ObjectResult;
        
        // Assert
        result.Result.Should().NotBeNull();
        result.Should().BeOfType<ActionResult<OrderDTOOutput>>();
        result.Result.Should().BeOfType<NotFoundObjectResult>()
            .Which.StatusCode.Should().Be(404);
        obj.Value.Should().BeEquivalentTo(orderResult.GenerateErrorResponse());
            
        await _mockOrderService.Received(1).FinishOrder(Arg.Any<int>());
    }
}