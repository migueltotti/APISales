using ApiSales.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ApiSales.Models;

namespace ApiSales.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductsController(IUnitOfWork _uof) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> Get()
    {
        return Ok(await _uof.ProductRepository.GetAllAsync());
    }

    // GET api/<OrdersController>/5
    [HttpGet("{id:int:min(1)}", Name = "GetProduct")]
    public async Task<ActionResult<Product>> Get(int id)
    {
        var product = await _uof.ProductRepository.GetAsync(p => p.ProductId == id);

        if (product is null)
        {
            return BadRequest("Incorrect Data: product not found");
        }

        return Ok(product);
    }

    // POST api/<OrdersController>
    [HttpPost]
    public async Task<ActionResult<Product>> Post([FromBody] Product product)
    {
        if (product is null)
        {
            return BadRequest("Incorrect Data: null");
        }

        var productCreated = _uof.ProductRepository.Create(product);
        await _uof.CommitChanges();

        return new CreatedAtRouteResult("GetProduct",
            new { id = productCreated.ProductId },
            productCreated);
    }

    // PUT api/<OrdersController>/5
    [HttpPut("{id:int:min(1)}")]
    public async Task<ActionResult<Product>> Put(int id, [FromBody] Product product)
    {
        if (product is null)
        {
            return BadRequest("Incorrect Data: null");
        }

        if (id != product.ProductId)
        {
            return BadRequest("Incorrect Data: id mismatch");
        }

        var productUpdated = _uof.ProductRepository.Update(product);
        await _uof.CommitChanges();

        return Ok(productUpdated);
    }

    // DELETE api/<OrdersController>/5
    [HttpDelete("{id:int:min(1)}")]
    public async Task<ActionResult<Product>> Delete(int id)
    {
        var product = await _uof.ProductRepository.GetAsync(p => p.ProductId == id);
        
        if (product is null)
        {
            return BadRequest("Incorrect Data: product not found");
        }

        var productDeleted = _uof.ProductRepository.Delete(product);
        await _uof.CommitChanges();

        return Ok(productDeleted);
    }
}