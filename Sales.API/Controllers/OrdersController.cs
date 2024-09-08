using Sales.API.DTOs.ProductDTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Sales.API.DTOs.OrderDTO;
using Sales.API.Models;
using Sales.API.Repositories.Interfaces;

namespace Sales.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrdersController(IUnitOfWork _uof, IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderDTOOutput>>> Get()
    {
        var orders = await _uof.OrderRepository.GetAllAsync();

        var ordersDto = mapper.Map<IEnumerable<OrderDTOOutput>>(orders);
        
        return Ok(ordersDto);
    }

    // GET api/<OrdersController>/5
    [HttpGet("{id:int:min(1)}", Name = "GetOrder")]
    public async Task<ActionResult<OrderDTOOutput>> Get(int id)
    {
        var order = await _uof.OrderRepository.GetAsync(o => o.OrderId == id);

        if (order is null)
        {
            return NotFound($"Order with id = {id} NotFound!");
        }

        var orderDto = mapper.Map<OrderDTOOutput>(order);

        return Ok(orderDto);
    }

    // POST api/<OrdersController>
    [HttpPost]
    public async Task<ActionResult<OrderDTOOutput>> Post([FromBody] OrderDTOInput orderDtoInput)
    {
        if (orderDtoInput is null)
        {
            return BadRequest("Incorrect Data: null");
        }

        var order = mapper.Map<Order>(orderDtoInput);

        var orderCreated = _uof.OrderRepository.Create(order);
        await _uof.CommitChanges();

        var orderDtoCreated = mapper.Map<OrderDTOOutput>(orderCreated);

        return new CreatedAtRouteResult("GetOrder",
            new { id = orderDtoCreated.OrderId },
            orderDtoCreated);
    }

    // PUT api/<OrdersController>/5
    [HttpPut("{id:int:min(1)}")]
    public async Task<ActionResult<OrderDTOOutput>> Put(int id, [FromBody] OrderDTOInput orderDtoInput)
    {
        if (orderDtoInput is null)
        {
            return BadRequest("Incorrect Data: null");
        }

        if (id != orderDtoInput.OrderId)
        {
            return BadRequest("Incorrect Data: id mismatch");
        }

        var order = mapper.Map<Order>(orderDtoInput);

        var orderUpdated = _uof.OrderRepository.Update(order);
        await _uof.CommitChanges();

        var orderDtoUpdated = mapper.Map<OrderDTOOutput>(orderUpdated);

        return Ok(orderDtoUpdated);
    }

    // DELETE api/<OrdersController>/5
    [HttpDelete("{id:int:min(1)}")]
    public async Task<ActionResult<OrderDTOOutput>> Delete(int id)
    {
        var order = await _uof.OrderRepository.GetAsync(o => o.OrderId == id);
        
        if (order is null)
        {
            return BadRequest("Incorrect Data: order not found");
        }

        var orderDeleted = _uof.OrderRepository.Delete(order);
        await _uof.CommitChanges();

        var orderDtoDeleted = mapper.Map<OrderDTOOutput>(orderDeleted);

        return Ok(orderDtoDeleted);
    }
}

