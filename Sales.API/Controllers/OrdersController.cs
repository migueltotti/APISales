using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Sales.Application.DTOs.OrderDTO;
using Sales.Application.DTOs.ProductDTO;
using Sales.Application.Interfaces;
using Sales.Application.Parameters;
using Sales.Application.Parameters.Extension;
using Sales.Application.Parameters.ModelsParameters;

namespace Sales.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrdersController(IOrderService _service, ILogger<OrdersController> _logger) : ControllerBase
{
    [HttpGet]
    [Authorize("AdminEmployeeOnly")]
    public async Task<ActionResult<IEnumerable<OrderDTOOutput>>> Get([FromQuery] QueryStringParameters parameters)
    {
        var ordersPaged = await _service.GetAllOrders(parameters);
        
        var metadata = ordersPaged.GenerateMetadataHeader();
        
        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(metadata));
        
        return Ok(ordersPaged.ToList());
    }
    
    [HttpGet("userId/{userId:int:min(1)}")]
    [Authorize(Policy = "AllowAnyUser")]
    public async Task<ActionResult<IEnumerable<OrderDTOOutput>>> GetOrdersByUserId(int userId, [FromQuery] OrderParameters parameters)
    {
        var ordersPaged = await _service.GetOrdersByUserId(userId, parameters);
        
        var metadata = ordersPaged.GenerateMetadataHeader();
        
        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(metadata));
        
        return Ok(ordersPaged.ToList());
    }
    
    [HttpGet("value")]
    [Authorize("AdminEmployeeOnly")]
    public async Task<ActionResult<IEnumerable<OrderDTOOutput>>> GetOrdersByValue([FromQuery] OrderParameters parameters)
    {
        var ordersPaged = await _service.GetOrdersWithFilter("value", parameters);
        
        var metadata = ordersPaged.GenerateMetadataHeader();
        
        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(metadata));
        
        return Ok(ordersPaged.ToList());
    }
    
    [HttpGet("Date")]
    [Authorize("AdminEmployeeOnly")]
    public async Task<ActionResult<IEnumerable<OrderDTOOutput>>> GetOrdersByDate([FromQuery] OrderParameters parameters)
    {
        var ordersPaged = await _service.GetOrdersWithFilter("date", parameters);
        
        var metadata = ordersPaged.GenerateMetadataHeader();
        
        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(metadata));
        
        return Ok(ordersPaged.ToList());
    }
    
    [HttpGet("Product")]
    [Authorize("AdminEmployeeOnly")]
    public async Task<ActionResult<IEnumerable<OrderDTOOutput>>> GetOrdersByProduct([FromQuery] OrderParameters parameters)
    {
        var ordersPaged = await _service.GetOrdersByProduct(parameters);
        
        var metadata = ordersPaged.GenerateMetadataHeader();
        
        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(metadata));
        
        return Ok(ordersPaged.ToList());
    }
    
    [HttpGet("Status")]
    [Authorize("AdminEmployeeOnly")]
    public async Task<ActionResult<IEnumerable<OrderDTOOutput>>> GetOrdersByStatus([FromQuery] OrderParameters parameters)
    {
        var ordersPaged = await _service.GetOrdersWithFilter("status", parameters);
        
        var metadata = ordersPaged.GenerateMetadataHeader();
        
        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(metadata));
        
        return Ok(ordersPaged.ToList());
    }
    
    [HttpGet("Affiliate/{id:int:min(1)}")]
    [Authorize("AdminEmployeeOnly")]
    public async Task<ActionResult<IEnumerable<OrderDTOOutput>>> GetOrdersByAffiliate(int id, [FromQuery] OrderParameters parameters)
    {
        var ordersPaged = await _service.GetOrdersByAffiliateId(id, parameters);
        
        var metadata = ordersPaged.GenerateMetadataHeader();
        
        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(metadata));
        
        return Ok(ordersPaged.ToList());
    }
    
    
    [HttpGet("{id:int:min(1)}", Name = "GetOrder")]
    [Authorize("AllowAnyUser")]
    public async Task<ActionResult<OrderDTOOutput>> Get(int id)
    {
        //var result = await _service.GetOrderBy(o => o.OrderId == id);
        var result = await _service.GetOrderById(id);
        
        switch(result.isSuccess)
        {
            case true:
                return Ok(result.value);
            case false:
                _logger.LogWarning(
                    "Request failed {@Error}, {@RequestName}, {@DateTime}",
                    result.error,
                    nameof(_service.GetOrderById),
                    DateTime.Now
                );
                return NotFound(result.GenerateErrorResponse());
        };
    }
    
    [HttpPost]
    [Authorize("AllowAnyUser")]
    public async Task<ActionResult<OrderDTOOutput>> Post([FromBody] OrderDTOInput orderDtoInput)
    {
        var result = await _service.CreateOrder(orderDtoInput);
        
        switch(result.isSuccess)
        {
            case true:
                return new CreatedAtRouteResult("GetOrder",
                    new { id = result.value.OrderId }, result.value);
            case false:
                _logger.LogWarning(
                    "Request failed {@Error}, {@RequestName}, {@DateTime}",
                    result.error,
                    nameof(_service.CreateOrder),
                    DateTime.Now
                );
                return BadRequest(result.GenerateErrorResponse());
        };
    }
    
    [HttpPut("{id:int:min(1)}")]
    [Authorize("AdminEmployeeOnly")]
    public async Task<ActionResult<OrderDTOOutput>> Put(int id, [FromBody] OrderDTOInput orderDtoInput)
    {
        var result = await _service.UpdateOrder(orderDtoInput, id);

        switch (result.isSuccess)
        {
            case true:
                return Ok($"Order with id = {result.value.OrderId} has been update successfully");
            case false:
                _logger.LogWarning(
                    "Request failed {@Error}, {@RequestName}, {@DateTime}",
                    result.error,
                    nameof(_service.UpdateOrder),
                    DateTime.Now
                );
                
                if(result.error.HttpStatusCode == HttpStatusCode.NotFound)
                    return NotFound(result.GenerateErrorResponse());
                
                return BadRequest(result.GenerateErrorResponse());
        }
    }
    
    [HttpDelete("{id:int:min(1)}")]
    [Authorize("AdminEmployeeOnly")]
    public async Task<ActionResult<OrderDTOOutput>> Delete(int id)
    {
        var result = await _service.DeleteOrder(id);
        
        switch(result.isSuccess)
        {
            case true:
                return Ok($"Order with id = {result.value.OrderId} has been deleted successfully");
            case false:
                _logger.LogWarning(
                    "Request failed {@Error}, {@RequestName}, {@DateTime}",
                    result.error,
                    nameof(_service.DeleteOrder),
                    DateTime.Now
                );
                return NotFound(result.GenerateErrorResponse());
        };
    }
    
    [HttpGet]
    [Route("{orderId:int:min(1)}/GetProducts")]
    [Authorize("AllowAnyUser")]
    public async Task<ActionResult<IEnumerable<ProductDTOOutput>>> GetProducts(int orderId)
    {
        var result = await _service.GetProductsByOrderId(orderId);
        
        switch(result.isSuccess)
        {
            case true:
                return Ok(result.value);
            case false:
                _logger.LogWarning(
                    "Request failed {@Error}, {@RequestName}, {@DateTime}",
                    result.error,
                    nameof(_service.GetProductsByOrderId),
                    DateTime.Now
                );
                return NotFound(result.GenerateErrorResponse());
        };
    }

    [HttpGet]
    [Route("OrderReport")]
    [Authorize("AdminEmployeeOnly")]
    public async Task<ActionResult<OrderReportDTO>> GetOrderReport([FromQuery] DateTime minDate, [FromQuery] DateTime maxDate)
    {
        var orderReport = await _service.GetOrderReport(minDate, maxDate);
        
        return Ok(orderReport);
    }
    
    [HttpPost]
    [Route("{orderId:int:min(1)}/AddProduct/{productId:int:min(1)}")]
    [Authorize("AllowAnyUser")]
    public async Task<ActionResult<OrderDTOOutput>> AddProduct(int orderId, int productId, [FromQuery] decimal amount)
    {
        var result = await _service.AddProduct(orderId, productId, amount);
        
        switch (result.isSuccess)
        {
            case true:
                return Ok($"Product with id = {productId} was added successfully on Order with id = {orderId}");
            case false:
                _logger.LogWarning(
                    "Request failed {@Error}, {@RequestName}, {@DateTime}",
                    result.error,
                    nameof(_service.AddProduct),
                    DateTime.Now
                );
                
                if (result.error.HttpStatusCode is HttpStatusCode.NotFound)
                    return NotFound(result.GenerateErrorResponse());
                
                return BadRequest(result.GenerateErrorResponse());
        };
    }

    [HttpDelete]
    [Route("{orderId:int:min(1)}/RemoveProduct/{productId:int:min(1)}")]
    [Authorize("AllowAnyUser")]
    public async Task<ActionResult<OrderProductDTO>> RemoveProduct(int orderId, int productId)
    {
        var result = await _service.RemoveProduct(orderId, productId);
        
        switch(result.isSuccess)
        {
            case true:
                return Ok($"Product with id = {productId} was removed from Order with id = {orderId} successfully");
            case false:
                _logger.LogWarning(
                    "Request failed {@Error}, {@RequestName}, {@DateTime}",
                    result.error,
                    nameof(_service.RemoveProduct),
                    DateTime.Now
                );
                return NotFound(result.GenerateErrorResponse());
        };
    }
    
    [HttpPost("sent/{orderId:int:min(1)}")]
    [Authorize("AllowAnyUser")]
    public async Task<ActionResult<OrderDTOOutput>> SentOrder(int orderId)
    {
        var result = await _service.SentOrder(orderId);
        
        switch(result.isSuccess)
        {
            case true:
                return Ok($"Order with id = {result.value.OrderId} sent successfully");
            case false:
                _logger.LogWarning(
                    "Request failed {@Error}, {@RequestName}, {@DateTime}",
                    result.error,
                    nameof(_service.SentOrder),
                    DateTime.Now
                );
                return NotFound(result.GenerateErrorResponse());
        };
    }

    [HttpPost("finish/{orderId:int:min(1)}")]
    [Authorize("AdminEmployeeOnly")]
    public async Task<ActionResult<OrderDTOOutput>> FinishOrder(int orderId)
    {
        var result = await _service.FinishOrder(orderId);

        switch(result.isSuccess)
        {
            case true:
                return Ok($"Order with id = {result.value.OrderId} finished successfully");
            case false:
                _logger.LogWarning(
                    "Request failed {@Error}, {@RequestName}, {@DateTime}",
                    result.error,
                    nameof(_service.FinishOrder),
                    DateTime.Now
                );
                return NotFound(result.GenerateErrorResponse());
        };
    }
}

