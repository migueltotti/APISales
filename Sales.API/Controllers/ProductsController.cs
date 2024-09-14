using System.Net;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Sales.Application.DTOs.ProductDTO;
using Sales.Application.Interfaces;
using Sales.Domain.Models;
using Sales.Domain.Interfaces;

namespace Sales.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductsController(IProductService _service, IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDTOOutput>>> Get()    
    {
        return Ok(await _service.GetAllProducts());
    }
    
    [HttpGet("{id:int:min(1)}", Name = "GetProduct")]
    public async Task<ActionResult<ProductDTOOutput>> Get(int id)
    {
        var result = await _service.GetProductBy(p => p.ProductId == id);

        return result.isSuccess switch
        {
            true => Ok(result.value),
            false => NotFound(result.error.Description)
        };
    }
    
    [HttpPost]
    public async Task<ActionResult<ProductDTOOutput>> Post([FromBody] ProductDTOInput productDtoInput)  
    {
        var result = await _service.CreateProduct(productDtoInput);

        return result.isSuccess switch
        {
            true => new CreatedAtRouteResult("GetProduct",
                new { id = result.value.ProductId }, result.value),
            false => BadRequest(result.error.Description)
        };
    }
    
    [HttpPut("{id:int:min(1)}")]
    public async Task<ActionResult<ProductDTOOutput>> Put(int id, [FromBody] ProductDTOInput productDtoInput)
    {
        var result = await _service.UpdateProduct(productDtoInput, id);

        switch (result.isSuccess)
        {
            case true:
                return Ok($"Product with id = {result.value.ProductId} was updated successfully");
            case false:
                if (result.error.HttpStatusCode == HttpStatusCode.NotFound)
                    return NotFound(result.error.Description);
                
                return BadRequest(result.error.Description);
        }
    }

    // DELETE api/<OrdersController>/5
    [HttpDelete("{id:int:min(1)}")]
    public async Task<ActionResult<ProductDTOOutput>> Delete(int id)
    {
        var result = await _service.DeleteProduct(id);
        
        return result.isSuccess switch
        {
            true => Ok($"Category with id = {result.value.CategoryId} was deleted successfully"),
            false => NotFound(result.error.Description)
        };
    }
}