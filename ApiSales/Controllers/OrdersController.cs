using ApiSales.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ApiSales.Models;

namespace ApiSales.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrdersController(IUnitOfWork _uof) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Order>>> Get()
    {
        return Ok(await _uof.OrderRepository.GetAllAsync());
    }

    // GET api/<OrdersController>/5
    [HttpGet("{id:int:min(1)}", Name = "GetOrder")]
    public async Task<ActionResult<Order>> Get(int id)
    {
        var order = await _uof.OrderRepository.GetAsync(o => o.OrderId == id);

        if (order is null)
        {
            return NotFound($"Order with id = {id} NotFound!");
        }

        return Ok(order);
    }

    // POST api/<OrdersController>
    [HttpPost]
    public async Task<ActionResult<Order>> Post([FromBody] Order order)
    {
        if (order is null)
        {
            return BadRequest("Incorrect Data: null");
        }

        var orderCreated = _uof.OrderRepository.Create(order);
        await _uof.CommitChanges();

        return new CreatedAtRouteResult("GetOrder",
            new { id = orderCreated.OrderId },
            orderCreated);
    }

    // PUT api/<OrdersController>/5
    [HttpPut("{id:int:min(1)}")]
    public async Task<ActionResult<Order>> Put(int id, [FromBody] Order order)
    {
        if (order is null)
        {
            return BadRequest("Incorrect Data: null");
        }

        if (id != order.OrderId)
        {
            return BadRequest("Incorrect Data: id mismatch");
        }

        var orderUpdated = _uof.OrderRepository.Update(order);
        await _uof.CommitChanges();

        return Ok(orderUpdated);
    }

    // DELETE api/<OrdersController>/5
    [HttpDelete("{id:int:min(1)}")]
    public async Task<ActionResult<Order>> Delete(int id)
    {
        var order = await _uof.OrderRepository.GetAsync(o => o.OrderId == id);
        
        if (order is null)
        {
            return BadRequest("Incorrect Data: order not found");
        }

        var orderDeleted = _uof.OrderRepository.Delete(order);
        await _uof.CommitChanges();

        return Ok(orderDeleted);
    }
}

