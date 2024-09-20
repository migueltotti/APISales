using System.Net;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using System.Text.Json;
using Sales.Application.DTOs.ProductDTO;
using Sales.Application.Interfaces;
using Sales.Application.Parameters;
using Sales.Application.Parameters.Extension;
using Sales.Application.Parameters.ModelsParameters;

namespace Sales.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductsController(IProductService _service, IMapper mapper) : ControllerBase
{
    [HttpGet]   
    public async Task<ActionResult<IEnumerable<ProductDTOOutput>>> Get([FromQuery] QueryStringParameters parameters)    
    {
        var productsPaged = await _service.GetAllProducts(parameters);

        var metadata = productsPaged.GenerateMetadataHeader();
        
        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(metadata));
        
        return Ok(productsPaged.ToList());
    }
    
    [HttpGet("value")]
    public async Task<ActionResult<IEnumerable<ProductDTOOutput>>> GetProductsByValue([FromQuery] ProductParameters parameters)    
    {
        var productsPaged = await _service.GetProductsWithFilter("value", parameters);

        var metadata = productsPaged.GenerateMetadataHeader();
        
        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(metadata));
        
        return Ok(productsPaged.ToList());
    }
    
    [HttpGet("typeValue")]
    public async Task<ActionResult<IEnumerable<ProductDTOOutput>>> GetProductsByTypeValue([FromQuery] ProductParameters parameters)    
    {
        var productsPaged = await _service.GetProductsWithFilter("typevalue", parameters);

        var metadata = productsPaged.GenerateMetadataHeader();
        
        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(metadata));
        
        return Ok(productsPaged.ToList());
    }
    
    [HttpGet("name")]
    public async Task<ActionResult<IEnumerable<ProductDTOOutput>>> GetProductsByName([FromQuery] ProductParameters parameters)    
    {
        var productsPaged = await _service.GetProductsWithFilter("name", parameters);  

        var metadata = productsPaged.GenerateMetadataHeader();
        
        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(metadata));
        
        return Ok(productsPaged.ToList());
    }
    
    [HttpGet("{id:int:min(1)}", Name = "GetProduct")]
    public async Task<ActionResult<ProductDTOOutput>> Get(int id)
    {
        var result = await _service.GetProductBy(p => p.ProductId == id);

        return result.isSuccess switch
        {
            true => Ok(result.value),
            false => NotFound(result.GenerateErrorResponse())
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
            false => BadRequest(result.GenerateErrorResponse())
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
                
                return BadRequest(result.GenerateErrorResponse());
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
            false => NotFound(result.GenerateErrorResponse())
        };
    }
}