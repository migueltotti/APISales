using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Sales.Application.DTOs.OrderDTO;
using Sales.Application.DTOs.ProductDTO;
using Sales.Application.Interfaces;
using Sales.Application.Parameters;
using Sales.Application.Parameters.Extension;
using Sales.Application.Parameters.ModelsParameters;
using Sales.Domain.Models;
using Sales.Domain.Interfaces;

namespace Sales.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrdersController(IOrderService _service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderDTOOutput>>> Get([FromQuery] QueryStringParameters parameters)
    {
        var ordersPaged = await _service.GetAllOrders(parameters);
        
        var metadata = ordersPaged.GenerateMetadataHeader();
        
        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(metadata));
        
        return Ok(ordersPaged.ToList());
    }
    
    [HttpGet("value")]
    public async Task<ActionResult<IEnumerable<OrderDTOOutput>>> GetOrderByValue([FromQuery] OrderParameters parameters)
    {
        var ordersPaged = await _service.GetOrdersWithFilter("value", parameters);
        
        var metadata = ordersPaged.GenerateMetadataHeader();
        
        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(metadata));
        
        return Ok(ordersPaged.ToList());
    }
    
    
    
    [HttpGet("Date")]
    public async Task<ActionResult<IEnumerable<OrderDTOOutput>>> GetOrderByDate([FromQuery] OrderParameters parameters)
    {
        var ordersPaged = await _service.GetOrdersWithFilter("date", parameters);
        
        var metadata = ordersPaged.GenerateMetadataHeader();
        
        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(metadata));
        
        return Ok(ordersPaged.ToList());
    }
    
    [HttpGet("Product")]
    public async Task<ActionResult<IEnumerable<OrderDTOOutput>>> GetOrderByProduct([FromQuery] OrderParameters parameters)
    {
        var ordersPaged = await _service.GetOrdersByProduct(parameters);
        
        var metadata = ordersPaged.GenerateMetadataHeader();
        
        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(metadata));
        
        return Ok(ordersPaged.ToList());
    }
    
    [HttpGet("{id:int:min(1)}", Name = "GetOrder")]
    public async Task<ActionResult<OrderDTOOutput>> Get(int id)
    {
        var result = await _service.GetOrderBy(o => o.OrderId == id);

        return result.isSuccess switch
        {
            true => Ok(result.value),
            false => NotFound(result.GenerateErrorResponse())
        };
    }
    
    [HttpPost]
    public async Task<ActionResult<OrderDTOOutput>> Post([FromBody] OrderDTOInput orderDtoInput)
    {
        var result = await _service.CreateOrder(orderDtoInput);

        return result.isSuccess switch
        {
            true => new CreatedAtRouteResult("GetOrder",
                new { id = result.value.OrderId }, result.value),
            false => BadRequest(result.GenerateErrorResponse())
        };
    }
    
    [HttpPut("{id:int:min(1)}")]
    public async Task<ActionResult<OrderDTOOutput>> Put(int id, [FromBody] OrderDTOInput orderDtoInput)
    {
        var result = await _service.UpdateOrder(orderDtoInput, id);

        switch (result.isSuccess)
        {
            case true:
                return Ok($"Order with id = {result.value.OrderId} has been update successfully");
            case false:
                if(result.error.HttpStatusCode == HttpStatusCode.NotFound)
                    return NotFound(result.GenerateErrorResponse());
                
                return BadRequest(result.GenerateErrorResponse());
        }
    }
    
    [HttpDelete("{id:int:min(1)}")]
    public async Task<ActionResult<OrderDTOOutput>> Delete(int id)
    {
        var result = await _service.DeleteOrder(id);
        
        return result.isSuccess switch
        {
            true => Ok($"Order with id = {result.value.OrderId} has been deleted successfully"),
            false => NotFound(result.GenerateErrorResponse())
        };
    }
    
    [HttpGet]
    [Route("{orderId:int:min(1)}/GetProducts")]
    public async Task<ActionResult<IEnumerable<ProductDTOOutput>>> GetProducts(int orderId)
    {
        var result = await _service.GetProductsByOrderId(orderId);

        return result.isSuccess switch
        {
            true => Ok(result.value),
            false => NotFound(result.GenerateErrorResponse())
        };
    }

    [HttpGet]
    [Route("OrderReport/{minDate:datetime}/{maxDate:datetime}")]
    public async Task<OrderReportDTO> GetOrderReport(DateTime minDate, DateTime maxDate)
    {
        var orderReport = await _service.GetOrderReport(minDate, maxDate);
        
        return orderReport;
    }
    
    [HttpPost]
    [Route("{orderId:int:min(1)}/AddProduct/{productId:int:min(1)}")]
    public async Task<ActionResult<OrderDTOOutput>> AddProduct(int orderId, int productId)
    {
        var result = await _service.AddProduct(orderId, productId);
        
        return result.isSuccess switch
        {
            true => Ok($"Product with id = {productId} was added successfully on Order with id = {orderId}"),
            false => NotFound(result.GenerateErrorResponse())
        };
    }

    [HttpDelete]
    [Route("{orderId:int:min(1)}/RemoveProduct/{productId:int:min(1)}")]
    public async Task<ActionResult<OrderProductDTO>> RemoveProduct(int orderId, int productId)
    {
        var result = await _service.RemoveProduct(orderId, productId);

        return result.isSuccess switch
        {
            true => Ok($"Product with id = {productId} was removed from Order with id = {orderId} successfully"),
            false => NotFound(result.GenerateErrorResponse())
        };
    }
    
    
}

