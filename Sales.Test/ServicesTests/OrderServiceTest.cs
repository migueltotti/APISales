using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using AutoFixture;
using AutoMapper;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using NSubstitute;
using Sales.Application.DTOs.OrderDTO;
using Sales.Application.DTOs.ProductDTO;
using Sales.Application.Interfaces;
using Sales.Application.Parameters;
using Sales.Application.Parameters.ModelsParameters;
using Sales.Application.ResultPattern;
using Sales.Application.Services;
using Sales.Domain.Interfaces;
using Sales.Domain.Models;
using Sales.Domain.Models.Enums;
using Sales.Infrastructure.Migrations.MySQLMigrations;
using X.PagedList;
using X.PagedList.Extensions;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace Sales.Test.ServicesTests;

public class OrderServiceTest
{
    private readonly IOrderService _orderService;
    private readonly IUnitOfWork _mockUof;
    private readonly IValidator<OrderDTOInput> _mockValidator;
    private readonly IMapper _mockMapper;
    private readonly IOrderFilterFactory _mockOrderFilterFactory;
    private readonly Fixture _fixture;

    public OrderServiceTest()
    {
        _mockUof = Substitute.For<IUnitOfWork>();
        _mockValidator = Substitute.For<IValidator<OrderDTOInput>>();
        _mockMapper = Substitute.For<IMapper>();
        _mockOrderFilterFactory = Substitute.For<IOrderFilterFactory>();
        _fixture = new Fixture();

        _orderService = new OrderService(
            _mockUof,
            _mockValidator,
            _mockMapper,
            _mockOrderFilterFactory
        );
    }

    [Fact]
    public async Task GetAllOrders_ShouldReturnAllOrders()
    {
        // Arrange
        var orders = _fixture.CreateMany<Order>(3).ToList();
        var ordersDto = _fixture.CreateMany<OrderDTOOutput>(3).ToList();

        _mockUof.OrderRepository.GetAllAsync().Returns(orders);
        _mockMapper.Map<IEnumerable<OrderDTOOutput>>(Arg.Any<IEnumerable<Order>>())
            .Returns(ordersDto);

        // Act
        var result = await _orderService.GetAllOrders();

        // Assert
        result.Should().BeEquivalentTo(ordersDto);
        
        await _mockUof.OrderRepository.Received(1).GetAllAsync();
        _mockMapper.Received(1).Map<IEnumerable<OrderDTOOutput>>(Arg.Any<IEnumerable<Order>>());
    }
    
    [Fact]
    public async Task GetAllOrdersPaged_ShouldReturnAllOrdersPaged()
    {
        // Arrange
        var parameters = new QueryStringParameters();
        var orders = _fixture.CreateMany<Order>(3).ToList();
        var ordersDto = _fixture.CreateMany<OrderDTOOutput>(3).ToList();

        _mockUof.OrderRepository.GetAllAsync().Returns(orders);
        _mockMapper.Map<IEnumerable<OrderDTOOutput>>(Arg.Any<IEnumerable<Order>>())
            .Returns(ordersDto);

        // Act
        var result = await _orderService.GetAllOrders(parameters);

        // Assert
        result.Should().BeEquivalentTo(ordersDto.ToPagedList(parameters.PageNumber, parameters.PageSize));
        
        await _mockUof.OrderRepository.Received(1).GetAllAsync();
        _mockMapper.Received(1).Map<IEnumerable<OrderDTOOutput>>(Arg.Any<IEnumerable<Order>>());
    }
    
    [Fact]
    public async Task GetOrdersWithFilter_ShouldReturnOrdersThatMatchesFilter()
    {
        // Arrange
        var filter = "filter";
        var parameters = new OrderParameters();
        var orders = _fixture.CreateMany<Order>(3).ToList();
        var ordersDto = _fixture.CreateMany<OrderDTOOutput>(3).ToList();

        _mockUof.OrderRepository.GetAllAsync().Returns(orders);
        _mockMapper.Map<IEnumerable<OrderDTOOutput>>(Arg.Any<IEnumerable<Order>>())
            .Returns(ordersDto);
        _mockOrderFilterFactory.GetStrategy(Arg.Any<string>())
            .ApplyFilter(Arg.Any<IEnumerable<OrderDTOOutput>>(), Arg.Any<OrderParameters>())
            .Returns(ordersDto);

        // Act
        var result = await _orderService.GetOrdersWithFilter(filter, parameters);

        // Assert
        result.Should().BeEquivalentTo(ordersDto.ToPagedList(parameters.PageNumber, parameters.PageSize));
        
        await _mockUof.OrderRepository.Received(1).GetAllAsync();
        _mockMapper.Received(1).Map<IEnumerable<OrderDTOOutput>>(Arg.Any<IEnumerable<Order>>());
        _mockOrderFilterFactory.GetStrategy(Arg.Any<string>())
            .ApplyFilter(Arg.Any<IEnumerable<OrderDTOOutput>>(), Arg.Any<OrderParameters>());
    }
    
    [Fact]
    public async Task GetOrdersByUserId_ShouldReturnOrdersByUserId()
    {
        // Arrange
        var userId = _fixture.Create<int>();
        var parameters = new QueryStringParameters();
        var orders = _fixture.CreateMany<Order>(3).ToList();
        var ordersDto = _fixture.CreateMany<OrderDTOOutput>(3).ToList();

        _mockUof.OrderRepository.GetAllAsync().Returns(orders);
        _mockMapper.Map<IEnumerable<OrderDTOOutput>>(Arg.Any<IEnumerable<Order>>())
            .Returns(ordersDto);

        // Act
        var result = await _orderService.GetOrdersByUserId(userId, parameters);

        // Assert
        result.Should().BeOfType<PagedList<OrderDTOOutput>>();
        
        await _mockUof.OrderRepository.Received(1).GetAllAsync();
        _mockMapper.Received(1).Map<IEnumerable<OrderDTOOutput>>(Arg.Any<IEnumerable<Order>>());
    }

    [Fact]
    public async Task GetOrdersByProduct_ShouldReturnAllOrdersThatHaveSpecifiedProduct()
    {
        // Arrange
        var parameters = new OrderParameters()
        {
            ProductName = "ProductName"
        };
        var orders = _fixture.CreateMany<Order>(3).ToList();
        var ordersDto = _fixture.CreateMany<OrderDTOOutput>(3).ToList();
        
        _mockUof.OrderRepository.GetOrdersByProduct(Arg.Any<string>()).Returns(orders);
        _mockMapper.Map<IEnumerable<OrderDTOOutput>>(Arg.Any<IEnumerable<Order>>())
            .Returns(ordersDto);
        
        // Act
        var result = await _orderService.GetOrdersByProduct(parameters);
        
        // Assert
        result.Should().BeEquivalentTo(ordersDto.ToPagedList(parameters.PageNumber, parameters.PageSize));
    }
    
    [Fact]
    public async Task GetOrdersByAffiliateId_ShouldReturnAllOrdersMadeByUsersThatHaveSpecifiedAffiliateId()
    {
        // Arrange
        var affiliateId = _fixture.Create<int>();
        var parameters = new OrderParameters();
        var orders = _fixture.CreateMany<Order>(3).ToList();
        var ordersDto = _fixture.CreateMany<OrderDTOOutput>(3).ToList();
        
        _mockUof.OrderRepository.GetOrdersByAffiliateId(Arg.Any<int>()).Returns(orders);
        _mockMapper.Map<IEnumerable<OrderDTOOutput>>(Arg.Any<IEnumerable<Order>>())
            .Returns(ordersDto);
        
        // Act
        var result = await _orderService.GetOrdersByAffiliateId(affiliateId, parameters);
        
        // Assert
        result.Should().BeEquivalentTo(ordersDto.ToPagedList(parameters.PageNumber, parameters.PageSize));
    }

    [Fact]
    public async Task GetOrderBy_ShouldReturnOrderThatMatchesExpression()
    {
        // Arrange
        var order = _fixture.Create<Order>();
        var orderDto = _fixture.Create<OrderDTOOutput>();

        _mockUof.OrderRepository.GetAsync(Arg.Any<Expression<Func<Order, bool>>>())
            .Returns(order);
        _mockMapper.Map<OrderDTOOutput>(Arg.Any<Order>()).Returns(orderDto);

        // Act
        var result = await _orderService.GetOrderBy(o => o.OrderId == order.OrderId);
        
        // Assert
        result.isSuccess.Should().BeTrue();
        result.value.Should().BeEquivalentTo(orderDto);

        await _mockUof.OrderRepository.Received(1).GetAsync(Arg.Any<Expression<Func<Order, bool>>>());
        _mockMapper.Received(1).Map<OrderDTOOutput>(Arg.Any<Order>());
    }
    
    [Fact]
    public async Task GetOrderBy_ShouldReturnNotFoundWithNotFoundResponse_WhenOrderDoesNotExist()
    {
        // Arrange
        Order order = null;

        _mockUof.OrderRepository.GetAsync(Arg.Any<Expression<Func<Order, bool>>>())
            .Returns(order);
        
        // Act
        var result = await _orderService.GetOrderBy(o => o.OrderId == order.OrderId);
        
        // Assert
        result.isSuccess.Should().BeFalse();
        result.error.Should().BeEquivalentTo(OrderErrors.NotFound);

        await _mockUof.OrderRepository.Received(1).GetAsync(Arg.Any<Expression<Func<Order, bool>>>());
    }

    [Fact]
    public async Task CreateOrder_ShouldReturnCreatedOrder()
    {
        // Arrange
        var orderInput = _fixture.Create<OrderDTOInput>();
        var validationResult = new ValidationResult()
        {
            Errors = new List<ValidationFailure>()
        };
        var order = _fixture.Create<Order>();
        var orderOutput = _fixture.Create<OrderDTOOutput>();

        _mockValidator.ValidateAsync(Arg.Any<OrderDTOInput>()).Returns(validationResult);
        _mockMapper.Map<Order>(Arg.Any<OrderDTOInput>()).Returns(order);
        _mockUof.OrderRepository.Create(Arg.Any<Order>()).Returns(order);
        _mockMapper.Map<OrderDTOOutput>(Arg.Any<Order>()).Returns(orderOutput);
        
        // Act
        var result = await _orderService.CreateOrder(orderInput);

        // Assert
        result.isSuccess.Should().BeTrue();
        result.value.Should().BeEquivalentTo(orderOutput);
        
        await _mockValidator.Received(1).ValidateAsync(Arg.Any<OrderDTOInput>());
        _mockMapper.Received(1).Map<Order>(Arg.Any<OrderDTOInput>());
        _mockUof.OrderRepository.Received(1).Create(Arg.Any<Order>());
        _mockMapper.Received(1).Map<OrderDTOOutput>(Arg.Any<Order>());
    }
    
    [Fact]
    public async Task CreateOrder_ShouldReturnBadRequestWithIncorrectFormatDataResponse_WhenOrderInputIsInvalid()
    {
        // Arrange
        var orderInput = _fixture.Create<OrderDTOInput>();
        var validationResult = new ValidationResult()
        {
            Errors = new List<ValidationFailure>()
            {
                new ValidationFailure("IncorrectFormatData", "IncorrectFormatData")
            }
        };

        _mockValidator.ValidateAsync(Arg.Any<OrderDTOInput>()).Returns(validationResult);
        
        // Act
        var result = await _orderService.CreateOrder(orderInput);

        // Assert
        result.isSuccess.Should().BeFalse();
        result.error.Should().BeEquivalentTo(OrderErrors.IncorrectFormatData);
        
        await _mockValidator.Received(1).ValidateAsync(Arg.Any<OrderDTOInput>());
    }
    
    [Fact]
    public async Task CreateOrder_ShouldReturnBadRequestWithDataIsNullResponse_WhenOrderInputIsNull()
    {
        // Arrange
        OrderDTOInput orderInput = null;
        
        // Act
        var result = await _orderService.CreateOrder(orderInput);

        // Assert
        result.isSuccess.Should().BeFalse();
        result.error.Should().BeEquivalentTo(OrderErrors.DataIsNull);
    }
    
    [Fact]
    public async Task UpdateOrder_ShouldReturnUpdatedOrder()
    {
        // Arrange
        var orderInput = _fixture.Create<OrderDTOInput>();
        var orderId = orderInput.OrderId;
        var validationResult = new ValidationResult()
        {
            Errors = new List<ValidationFailure>()
        };
        var order = _fixture.Create<Order>();
        var orderOutput = _fixture.Create<OrderDTOOutput>();

        _mockValidator.ValidateAsync(Arg.Any<OrderDTOInput>()).Returns(validationResult);
        _mockUof.OrderRepository.GetAsync(Arg.Any<Expression<Func<Order, bool>>>()).Returns(order);
        _mockMapper.Map<Order>(Arg.Any<OrderDTOInput>()).Returns(order);
        _mockUof.OrderRepository.Update(Arg.Any<Order>()).Returns(order);
        _mockMapper.Map<OrderDTOOutput>(Arg.Any<Order>()).Returns(orderOutput);
        
        // Act
        var result = await _orderService.UpdateOrder(orderInput, orderId);

        // Assert
        result.isSuccess.Should().BeTrue();
        result.value.Should().BeEquivalentTo(orderOutput);
        
        await _mockValidator.Received(1).ValidateAsync(Arg.Any<OrderDTOInput>());
        await _mockUof.OrderRepository.Received(1).GetAsync(Arg.Any<Expression<Func<Order, bool>>>());
        _mockMapper.Received(1).Map<Order>(Arg.Any<OrderDTOInput>());
        _mockUof.OrderRepository.Received(1).Update(Arg.Any<Order>());
        _mockMapper.Received(1).Map<OrderDTOOutput>(Arg.Any<Order>());
    }
    
    [Fact]
    public async Task UpdateOrder_ShouldReturnNotFoundWithNotFoundResponse_WhenOrderDoesNotExist()
    {
        // Arrange
        var orderInput = _fixture.Create<OrderDTOInput>();
        var orderId = orderInput.OrderId;
        var validationResult = new ValidationResult()
        {
            Errors = new List<ValidationFailure>()
        };
        Order order = null;

        _mockValidator.ValidateAsync(Arg.Any<OrderDTOInput>()).Returns(validationResult);
        _mockUof.OrderRepository.GetAsync(Arg.Any<Expression<Func<Order, bool>>>()).Returns(order);
        
        // Act
        var result = await _orderService.UpdateOrder(orderInput, orderId);

        // Assert
        result.isSuccess.Should().BeFalse();
        result.error.Should().BeEquivalentTo(OrderErrors.NotFound);
        
        await _mockValidator.Received(1).ValidateAsync(Arg.Any<OrderDTOInput>());
        await _mockUof.OrderRepository.Received(1).GetAsync(Arg.Any<Expression<Func<Order, bool>>>());
    }
    
    [Fact]
    public async Task UpdateOrder_ShouldReturnBadRequestWithIncorrectDataFormatResponse_WhenOrderInputIsInvalid()
    {
        // Arrange
        var orderInput = _fixture.Create<OrderDTOInput>();
        var orderId = orderInput.OrderId;
        var validationResult = new ValidationResult()
        {
            Errors = new List<ValidationFailure>()
            {
                new ValidationFailure("IncorrectFormatData", "IncorrectFormatData")
            }
        };

        _mockValidator.ValidateAsync(Arg.Any<OrderDTOInput>()).Returns(validationResult);
        
        // Act
        var result = await _orderService.UpdateOrder(orderInput, orderId);

        // Assert
        result.isSuccess.Should().BeFalse();
        result.error.Should().BeEquivalentTo(OrderErrors.IncorrectFormatData);
        
        await _mockValidator.Received(1).ValidateAsync(Arg.Any<OrderDTOInput>());
    }
    
    [Fact]
    public async Task UpdateOrder_ShouldReturnBadRequestWithIdMismatchResponse_WhenOrderAndOrderIdMismatches()
    {
        // Arrange
        var orderInput = _fixture.Create<OrderDTOInput>();
        var orderId = _fixture.Create<int>();
        
        // Act
        var result = await _orderService.UpdateOrder(orderInput, orderId);

        // Assert
        result.isSuccess.Should().BeFalse();
        result.error.Should().BeEquivalentTo(OrderErrors.IdMismatch);
    }
    
    [Fact]
    public async Task UpdateOrder_ShouldReturnBadRequestWithDataIsNullResponse_WhenOrderInputIsNull()
    {
        // Arrange
        OrderDTOInput orderInput = null;
        var orderId = _fixture.Create<int>();
        
        // Act
        var result = await _orderService.UpdateOrder(orderInput, orderId);

        // Assert
        result.isSuccess.Should().BeFalse();
        result.error.Should().BeEquivalentTo(OrderErrors.DataIsNull);
    }

    [Fact]
    public async Task DeleteProduct_ShouldReturnDeletedProduct()
    {
        // Arrange
        var orderId = _fixture.Create<int>();
        var order = _fixture.Create<Order>();
        var orderDeletedDto = _fixture.Create<OrderDTOOutput>();
        
        _mockUof.OrderRepository.GetAsync(Arg.Any<Expression<Func<Order, bool>>>())
            .Returns(order);
        _mockUof.OrderRepository.Delete(Arg.Any<Order>())
            .Returns(order);
        _mockMapper.Map<OrderDTOOutput>(Arg.Any<Order>()).Returns(orderDeletedDto);
        
        // Act
        var result = await _orderService.DeleteOrder(orderId);

        // Assert
        result.isSuccess.Should().BeTrue();
        result.value.Should().BeEquivalentTo(orderDeletedDto);

        await _mockUof.OrderRepository.Received(1).GetAsync(Arg.Any<Expression<Func<Order, bool>>>());
        _mockUof.OrderRepository.Received(1).Delete(Arg.Any<Order>());
        _mockMapper.Received(1).Map<OrderDTOOutput>(Arg.Any<Order>());
    }
    
    [Fact]
    public async Task DeleteProduct_ShouldReturnNotFoundResultWithNotFoundResponse_WhenOrderDoesNotExist()
    {
        // Arrange
        var orderId = _fixture.Create<int>();
        Order order = null;
        
        _mockUof.OrderRepository.GetAsync(Arg.Any<Expression<Func<Order, bool>>>())
            .Returns(order);
        
        // Act
        var result = await _orderService.DeleteOrder(orderId);

        // Assert
        result.isSuccess.Should().BeFalse();
        result.error.Should().BeEquivalentTo(OrderErrors.NotFound);

        await _mockUof.OrderRepository.Received(1).GetAsync(Arg.Any<Expression<Func<Order, bool>>>());
    }

    [Fact]
    public async Task SentOrder_ShouldReturnSentOrder()
    {
        // Arrange
        var orderId = _fixture.Create<int>();
        //var orderProducts = _fixture.CreateMany<Product>(3).ToList();
        var order = new Order(
            orderId,
            10m,
            DateTime.Now,
            1);
        order.Products.Add(_fixture.Create<Product>());
        var products = new List<Product>();
        var orderDto = _fixture.Create<OrderDTOOutput>();
        
        
        _mockUof.OrderRepository.GetOrderProductsById(Arg.Any<int>()).Returns(order);
        _mockUof.OrderRepository.GetProducts(Arg.Any<int>()).Returns(products);
        _mockUof.OrderRepository.Update(Arg.Any<Order>()).Returns(order);
        _mockMapper.Map<OrderDTOOutput>(Arg.Any<Order>()).Returns(orderDto);

        // Act
        var result = await _orderService.SentOrder(orderId);

        // Assert
        result.isSuccess.Should().BeTrue();
        result.value.Should().BeEquivalentTo(orderDto);
        
        await _mockUof.OrderRepository.Received(1).GetOrderProductsById(Arg.Any<int>());
        await _mockUof.OrderRepository.Received(1).GetProducts(Arg.Any<int>());
        _mockUof.OrderRepository.Received(1).Update(Arg.Any<Order>());
    }
    
    [Fact]
    public async Task SentOrder_ShouldReturnBadRequestWithProductsUnavailableResponse_WhenProductsAreNoMoreAvailable()
    {
        // Arrange
        var orderId = _fixture.Create<int>();
        //var orderProducts = _fixture.CreateMany<Product>(3).ToList();
        var order = new Order(
            orderId,
            10m,
            DateTime.Now,
            1);
        var products = new List<Product>()
        {
            new Product("Nome", "Description", 1m, TypeValue.Kg, "image.url", 0, 1)
        };
        
        _mockUof.OrderRepository.GetOrderProductsById(Arg.Any<int>()).Returns(order);
        _mockUof.OrderRepository.GetProducts(Arg.Any<int>()).Returns(products);

        // Act
        var result = await _orderService.SentOrder(orderId);

        // Assert
        result.isSuccess.Should().BeFalse();
        result.error.Should().BeEquivalentTo(OrderErrors.ProductsStockUnavailable);
        result.validationFailures.Should().NotBeEmpty();
        
        await _mockUof.OrderRepository.Received(1).GetOrderProductsById(Arg.Any<int>());
        await _mockUof.OrderRepository.Received(1).GetProducts(Arg.Any<int>());
    }

    [Fact] public async Task SentOrder_ShouldReturnBadRequestWithOrderFinishedOrSentResponse_WhenOrderHasAlreadyBeenSent()
    {
        // Arrange
        var orderId = _fixture.Create<int>();
        //var orderProducts = _fixture.CreateMany<Product>(3).ToList();
        var order = new Order(
            orderId,
            10m,
            DateTime.Now,
            1,
            Status.Sent);
        
        _mockUof.OrderRepository.GetOrderProductsById(Arg.Any<int>()).Returns(order);

        // Act
        var result = await _orderService.SentOrder(orderId);

        // Assert
        result.isSuccess.Should().BeFalse();
        result.error.Should().BeEquivalentTo(OrderErrors.OrderFinishedOrSent);
        
        await _mockUof.OrderRepository.Received(1).GetOrderProductsById(Arg.Any<int>());
    }
    
    [Fact] public async Task SentOrder_ShouldReturnNotFoundWithNotFoundNullResponse_WhenOrderDoesNotExist()
    {
        // Arrange
        var orderId = _fixture.Create<int>();
        //var orderProducts = _fixture.CreateMany<Product>(3).ToList();
        Order order = null;
        
        _mockUof.OrderRepository.GetOrderProductsById(Arg.Any<int>()).Returns(order);

        // Act
        var result = await _orderService.SentOrder(orderId);

        // Assert
        result.isSuccess.Should().BeFalse();
        result.error.Should().BeEquivalentTo(OrderErrors.NotFound);
        
        await _mockUof.OrderRepository.Received(1).GetOrderProductsById(Arg.Any<int>());
    }

    [Fact]
    public async Task FinishOrder_ShouldReturnFinishedOrder()
    {
        // Arrange
        var orderId = _fixture.Create<int>();
        var order = new Order(
            orderId,
            10m,
            DateTime.Now,
            1,
            Status.Sent);
        var orderDto = _fixture.Create<OrderDTOOutput>();
        
        _mockUof.OrderRepository.GetOrderProductsById(Arg.Any<int>()).Returns(order);
        _mockMapper.Map<OrderDTOOutput>(Arg.Any<Order>()).Returns(orderDto);
        
        // Act
        var result = await _orderService.FinishOrder(orderId);
        
        // Assert
        result.isSuccess.Should().BeTrue();
        result.value.Should().BeEquivalentTo(orderDto);
        
        await _mockUof.OrderRepository.Received(1).GetOrderProductsById(Arg.Any<int>());
        _mockMapper.Received(1).Map<OrderDTOOutput>(Arg.Any<Order>());
    }
    
    [Fact]
    public async Task FinishOrder_ShouldReturnBadRequestWithOrderFinishedOrSentResponse_WhenOrderHasAlreadyBeenFinished()
    {
        // Arrange
        var orderId = _fixture.Create<int>();
        var order = new Order(
            orderId,
            10m,
            DateTime.Now,
            1,
            Status.Finished);
        
        _mockUof.OrderRepository.GetOrderProductsById(Arg.Any<int>()).Returns(order);
        
        // Act
        var result = await _orderService.FinishOrder(orderId);
        
        // Assert
        result.isSuccess.Should().BeFalse();
        result.error.Should().BeEquivalentTo(OrderErrors.OrderFinishedOrSent);
        
        await _mockUof.OrderRepository.Received(1).GetOrderProductsById(Arg.Any<int>());
    }
    
    [Fact]
    public async Task FinishOrder_ShouldReturnBadRequestWithOrderNotSentResponse_WhenOrderIsPreparing()
    {
        // Arrange
        var orderId = _fixture.Create<int>();
        var order = new Order(
            orderId,
            10m,
            DateTime.Now,
            1,
            Status.Preparing);
        
        _mockUof.OrderRepository.GetOrderProductsById(Arg.Any<int>()).Returns(order);
        
        // Act
        var result = await _orderService.FinishOrder(orderId);
        
        // Assert
        result.isSuccess.Should().BeFalse();
        result.error.Should().BeEquivalentTo(OrderErrors.OrderNotSent);
        
        await _mockUof.OrderRepository.Received(1).GetOrderProductsById(Arg.Any<int>());
    }
    
    [Fact]
    public async Task FinishOrder_ShouldReturnNotFoundWithNotFoundResponse_WhenOrderDoesNotExists()
    {
        // Arrange
        var orderId = _fixture.Create<int>();
        Order order = null;
        
        _mockUof.OrderRepository.GetOrderProductsById(Arg.Any<int>()).Returns(order);
        
        // Act
        var result = await _orderService.FinishOrder(orderId);
        
        // Assert
        result.isSuccess.Should().BeFalse();
        result.error.Should().BeEquivalentTo(OrderErrors.NotFound);
        
        await _mockUof.OrderRepository.Received(1).GetOrderProductsById(Arg.Any<int>());
    }

    [Fact]
    public async Task AddProduct_ShouldReturnOrderWithProductAdded()
    {
        // Arrange
        var orderId = _fixture.Create<int>();
        var amount = 1m;
        var order = new Order(
            orderId,
            10m,
            DateTime.Now,
            1);
        var product = new Product(
            154,
            "Product",
            "Product",
            10m,
            TypeValue.Uni,
            "image.jpg",
            1,
            1);
        var orderProductDto = _fixture.Create<OrderProductDTO>();
        
        _mockUof.OrderRepository.GetAsync(Arg.Any<Expression<Func<Order, bool>>>()).Returns(order);
        _mockUof.ProductRepository.GetAsync(Arg.Any<Expression<Func<Product, bool>>>()).Returns(product);
        _mockUof.OrderRepository.AddProduct(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<decimal>()).Returns(1);
        _mockMapper.Map<OrderProductDTO>(Arg.Any<Order>()).Returns(orderProductDto);
        
        // Act
        var result = await _orderService.AddProduct(orderId, product.ProductId, amount);
        
        // Assert
        result.isSuccess.Should().BeTrue();
        result.value.Should().BeEquivalentTo(orderProductDto);
        
        await _mockUof.OrderRepository.Received(1).GetAsync(Arg.Any<Expression<Func<Order, bool>>>());
        await _mockUof.ProductRepository.Received(1).GetAsync(Arg.Any<Expression<Func<Product, bool>>>());
        await _mockUof.OrderRepository.Received(1).AddProduct(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<decimal>());
        _mockMapper.Received(1).Map<OrderProductDTO>(Arg.Any<Order>());
    }
    
    [Fact]
    public async Task AddProduct_ShouldReturnBadRequestWithDataIsNull_WhenNoRowsWereAffectedWhenTryingToAddProduct()
    {
        // Arrange
        var orderId = _fixture.Create<int>();
        var amount = 1m;
        var order = new Order(
            orderId,
            10m,
            DateTime.Now,
            1);
        var product = _fixture.Create<Product>();
        
        _mockUof.OrderRepository.GetAsync(Arg.Any<Expression<Func<Order, bool>>>()).Returns(order);
        _mockUof.ProductRepository.GetAsync(Arg.Any<Expression<Func<Product, bool>>>()).Returns(product);
        _mockUof.OrderRepository.AddProduct(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<decimal>()).Returns(0);
        
        // Act
        var result = await _orderService.AddProduct(orderId, product.ProductId, amount);
        
        // Assert
        result.isSuccess.Should().BeFalse();
        result.error.Should().BeEquivalentTo(OrderErrors.NoRowsAffected);
        
        await _mockUof.OrderRepository.Received(1).GetAsync(Arg.Any<Expression<Func<Order, bool>>>());
        await _mockUof.ProductRepository.Received(1).GetAsync(Arg.Any<Expression<Func<Product, bool>>>());
        await _mockUof.OrderRepository.Received(1).AddProduct(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<decimal>());
    }
    
    [Fact]
    public async Task AddProduct_ShouldReturnBadRequestWithStockUnavailable_WhenProductQuantityIsZero()
    {
        // Arrange
        var orderId = _fixture.Create<int>();
        var amount = 1m;
        var order = new Order(
            orderId,
            10m,
            DateTime.Now,
            1);
        var product = new Product("Nome", "Description", 1m, TypeValue.Uni, "image.url", 0, 1);
        
        _mockUof.OrderRepository.GetAsync(Arg.Any<Expression<Func<Order, bool>>>()).Returns(order);
        _mockUof.ProductRepository.GetAsync(Arg.Any<Expression<Func<Product, bool>>>()).Returns(product);
        
        // Act
        var result = await _orderService.AddProduct(orderId, product.ProductId, amount);
        
        // Assert
        result.isSuccess.Should().BeFalse();
        result.error.Should().BeEquivalentTo(ProductErrors.StockUnavailable);
        
        await _mockUof.OrderRepository.Received(1).GetAsync(Arg.Any<Expression<Func<Order, bool>>>());
        await _mockUof.ProductRepository.Received(1).GetAsync(Arg.Any<Expression<Func<Product, bool>>>());
    }
    
    [Fact]
    public async Task AddProduct_ShouldReturnNotFoundWithNotFoundResponse_WhenProductDoesNotExists()
    {
        // Arrange
        var orderId = _fixture.Create<int>();
        var amount = 1m;
        var order = new Order(
            orderId,
            10m,
            DateTime.Now,
            1);
        var productId = _fixture.Create<int>();
        Product product = null;
        
        _mockUof.OrderRepository.GetAsync(Arg.Any<Expression<Func<Order, bool>>>()).Returns(order);
        _mockUof.ProductRepository.GetAsync(Arg.Any<Expression<Func<Product, bool>>>()).Returns(product);
        
        // Act
        var result = await _orderService.AddProduct(orderId, productId, amount);
        
        // Assert
        result.isSuccess.Should().BeFalse();
        result.error.Should().BeEquivalentTo(ProductErrors.NotFound);
        
        await _mockUof.OrderRepository.Received(1).GetAsync(Arg.Any<Expression<Func<Order, bool>>>());
        await _mockUof.ProductRepository.Received(1).GetAsync(Arg.Any<Expression<Func<Product, bool>>>());
    }
    
    [Fact]
    public async Task AddProduct_ShouldReturnBadRequestWithOrderFinishedOrSent_WhenOrderHasAlreadyBeenFinishedOrSent()
    {
        // Arrange
        var orderId = _fixture.Create<int>();
        var amount = 1m;
        var order = new Order(
            orderId,
            10m,
            DateTime.Now,
            1,
            Status.Sent);
        var productId = _fixture.Create<int>();
        
        _mockUof.OrderRepository.GetAsync(Arg.Any<Expression<Func<Order, bool>>>()).Returns(order);
        
        // Act
        var result = await _orderService.AddProduct(orderId, productId, amount);
        
        // Assert
        result.isSuccess.Should().BeFalse();
        result.error.Should().BeEquivalentTo(OrderErrors.OrderFinishedOrSent);
        
        await _mockUof.OrderRepository.Received(1).GetAsync(Arg.Any<Expression<Func<Order, bool>>>());
    }
    
    [Fact]
    public async Task AddProduct_ShouldReturnNotFoundWithNotFoundResponse_WhenOrderDoesNotExists()
    {
        // Arrange
        var orderId = _fixture.Create<int>();
        var amount = 1m;
        Order order = null;
        var productId = _fixture.Create<int>();
        
        _mockUof.OrderRepository.GetAsync(Arg.Any<Expression<Func<Order, bool>>>()).Returns(order);
        
        // Act
        var result = await _orderService.AddProduct(orderId, productId, amount);
        
        // Assert
        result.isSuccess.Should().BeFalse();
        result.error.Should().BeEquivalentTo(OrderErrors.NotFound);
        
        await _mockUof.OrderRepository.Received(1).GetAsync(Arg.Any<Expression<Func<Order, bool>>>());
    }
    
    [Fact]
    public async Task GeProductsByOrderId_ShouldReturnProductsThatBelongToSpecifiedOrder()
    {
        // Arrange
        var orderId = _fixture.Create<int>();
        var order = _fixture.Create<Order>();
        var products = _fixture.CreateMany<Product>(3);
        var productsDto = _fixture.CreateMany<ProductDTOOutput>(3);
        
        _mockUof.OrderRepository.GetAsync(Arg.Any<Expression<Func<Order, bool>>>()).Returns(order);
        _mockUof.OrderRepository.GetProducts(Arg.Any<int>()).Returns(products);
        _mockMapper.Map<IEnumerable<ProductDTOOutput>>(products).Returns(productsDto);
        
        // Act
        var result = await _orderService.GetProductsByOrderId(orderId);
        
        // Assert
        result.isSuccess.Should().BeTrue();
        result.value.Should().BeEquivalentTo(productsDto);
        
        await _mockUof.OrderRepository.Received(1).GetAsync(Arg.Any<Expression<Func<Order, bool>>>());
        await _mockUof.OrderRepository.Received(1).GetProducts(Arg.Any<int>());
        _mockMapper.Received(1).Map<IEnumerable<ProductDTOOutput>>(products);
    }
    
    [Fact]
    public async Task GeProductsByOrderId_ShouldReturnNotFoundWithProducsNotFoundResponse_WhenOrderDontHaveProducts()
    {
        // Arrange
        var orderId = _fixture.Create<int>();
        var order = _fixture.Create<Order>();
        var products = new List<Product>();
        
        _mockUof.OrderRepository.GetAsync(Arg.Any<Expression<Func<Order, bool>>>()).Returns(order);
        _mockUof.OrderRepository.GetProducts(Arg.Any<int>()).Returns(products);
        
        // Act
        var result = await _orderService.GetProductsByOrderId(orderId);
        
        // Assert
        result.isSuccess.Should().BeFalse();
        result.error.Should().BeEquivalentTo(OrderErrors.ProductsNotFound);
        
        await _mockUof.OrderRepository.Received(1).GetAsync(Arg.Any<Expression<Func<Order, bool>>>());
        await _mockUof.OrderRepository.Received(1).GetProducts(Arg.Any<int>());
    }
    
    [Fact]
    public async Task GeProductsByOrderId_ShouldReturnNotFoundWithProducsNotFoundResponse_WhenOrderDoesNotExists()
    {
        // Arrange
        var orderId = _fixture.Create<int>();
        Order order = null;
        
        _mockUof.OrderRepository.GetAsync(Arg.Any<Expression<Func<Order, bool>>>()).Returns(order);
        
        // Act
        var result = await _orderService.GetProductsByOrderId(orderId);
        
        // Assert
        result.isSuccess.Should().BeFalse();
        result.error.Should().BeEquivalentTo(OrderErrors.NotFound);
        
        await _mockUof.OrderRepository.Received(1).GetAsync(Arg.Any<Expression<Func<Order, bool>>>());
    }

    [Fact]
    public async Task RemoveProduct_ShouldReturnOrderWithoutSpecifiedProduct()
    {
        // Arrange
        var orderId = _fixture.Create<int>();
        var productId = _fixture.Create<int>();
        var order = _fixture.Create<Order>();
        var productValueAmount = _fixture.CreateMany<ProductInfo>(3);
        var orderProductDto = _fixture.Create<OrderProductDTO>();
        
        _mockUof.OrderRepository.GetAsync(Arg.Any<Expression<Func<Order, bool>>>()).Returns(order);
        _mockUof.OrderRepository.GetProductValueAndAmount(Arg.Any<int>(), Arg.Any<int>()).Returns(productValueAmount);
        _mockUof.OrderRepository.RemoveProduct(Arg.Any<int>(), Arg.Any<int>()).Returns(1);
        _mockMapper.Map<OrderProductDTO>(Arg.Any<Order>()).Returns(orderProductDto);
        
        // Act
        var result = await _orderService.RemoveProduct(orderId, productId);
        
        // Assert
        result.isSuccess.Should().BeTrue();
        result.value.Should().BeEquivalentTo(orderProductDto);
        
        await _mockUof.OrderRepository.Received(1).GetAsync(Arg.Any<Expression<Func<Order, bool>>>());
        await _mockUof.OrderRepository.Received(1).GetProductValueAndAmount(Arg.Any<int>(), Arg.Any<int>());
        await _mockUof.OrderRepository.Received(1).RemoveProduct(Arg.Any<int>(), Arg.Any<int>());
        _mockMapper.Received(1).Map<OrderProductDTO>(Arg.Any<Order>());
    }
    
    [Fact]
    public async Task RemoveProduct_ShouldReturBadRequestWithNoRowsEfected_WhenOccurredErrorsWhileTryingToRemoveProduct()
    {
        // Arrange
        var orderId = _fixture.Create<int>();
        var productId = _fixture.Create<int>();
        var order = _fixture.Create<Order>();
        var productValueAmount = _fixture.CreateMany<ProductInfo>(3);
        
        _mockUof.OrderRepository.GetAsync(Arg.Any<Expression<Func<Order, bool>>>()).Returns(order);
        _mockUof.OrderRepository.GetProductValueAndAmount(Arg.Any<int>(), Arg.Any<int>()).Returns(productValueAmount);
        _mockUof.OrderRepository.RemoveProduct(Arg.Any<int>(), Arg.Any<int>()).Returns(0);
        
        // Act
        var result = await _orderService.RemoveProduct(orderId, productId);
        
        // Assert
        result.isSuccess.Should().BeFalse();
        result.error.Should().BeEquivalentTo(OrderErrors.ProductNotFound);
        
        await _mockUof.OrderRepository.Received(1).GetAsync(Arg.Any<Expression<Func<Order, bool>>>());
        await _mockUof.OrderRepository.Received(1).GetProductValueAndAmount(Arg.Any<int>(), Arg.Any<int>());
        await _mockUof.OrderRepository.Received(1).RemoveProduct(Arg.Any<int>(), Arg.Any<int>());
    }
    
    [Fact]
    public async Task RemoveProduct_ShouldReturNotFoundWithNotFoundResponse_WhenOrderDoesNotExists()
    {
        // Arrange
        var orderId = _fixture.Create<int>();
        var productId = _fixture.Create<int>();
        Order order = null;
        
        _mockUof.OrderRepository.GetAsync(Arg.Any<Expression<Func<Order, bool>>>()).Returns(order);
        
        // Act
        var result = await _orderService.RemoveProduct(orderId, productId);
        
        // Assert
        result.isSuccess.Should().BeFalse();
        result.error.Should().BeEquivalentTo(OrderErrors.NotFound);
        
        await _mockUof.OrderRepository.Received(1).GetAsync(Arg.Any<Expression<Func<Order, bool>>>());
    }

    [Fact]
    public async Task GetOrderReport_ShouldReturnOrderReportThatMatchesSpecifiedTimeInterval()
    {
        // Arrange
        var initialDate = _fixture.Create<DateTime>();
        var finalDate = initialDate.AddDays(10);
        var orders = _fixture.CreateMany<Order>(4).ToList();
        var ordersDto = _fixture.CreateMany<OrderDTOOutput>(4).ToList();
        var products = _fixture.CreateMany<Product>(3).ToList();
            
        _mockUof.OrderRepository.GetAllAsync().Returns(orders);
        _mockMapper.Map<IEnumerable<OrderDTOOutput>>(Arg.Any<IEnumerable<Order>>()).Returns(ordersDto);
        _mockUof.OrderRepository.GetProductsByDate(Arg.Any<DateTime>(), Arg.Any<DateTime>()).Returns(products);
        
        // Act
        var result = await _orderService.GetOrderReport(initialDate, finalDate);
        
        // Assert
        result.Should().BeOfType<OrderReportDTO>();
        
        await _mockUof.OrderRepository.Received(1).GetAllAsync();
        _mockMapper.Received(1).Map<IEnumerable<OrderDTOOutput>>(Arg.Any<IEnumerable<Order>>());
        await _mockUof.OrderRepository.Received(1).GetProductsByDate(Arg.Any<DateTime>(), Arg.Any<DateTime>());
    }
}