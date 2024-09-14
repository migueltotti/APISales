using System.Net;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Sales.Application.DTOs.OrderDTO;
using Sales.Application.Interfaces;
using Sales.Domain.Models;
using Sales.Domain.Interfaces;

namespace Sales.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrdersController(IOrderService _service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderDTOOutput>>> Get()
    {
        return Ok(await _service.GetAllOrders());
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
}

