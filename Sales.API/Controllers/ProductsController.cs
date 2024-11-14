using System.Net;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Sales.Application.DTOs.ProductDTO;
using Sales.Application.Interfaces;
using Sales.Application.Parameters;
using Sales.Application.Parameters.Extension;
using Sales.Application.Parameters.ModelsParameters;

namespace Sales.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductsController(IProductService _service, ILogger<ProductsController> _logger) : ControllerBase
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
        //var result = await _service.GetProductBy(p => p.ProductId == id);
        var result = await _service.GetProductById(id);
        
        switch(result.isSuccess)
        {
            case true:
                return Ok(result.value);
            case false:
                _logger.LogWarning(
                    "Request failed {@Error}, {@RequestName}, {@DateTime}",
                    result.error,
                    nameof(_service.GetProductById),
                    DateTime.Now
                );
                return NotFound(result.GenerateErrorResponse());
        };
    }
    
    [HttpPost]
    [Authorize("AdminOnly")]
    public async Task<ActionResult<ProductDTOOutput>> Post([FromBody] ProductDTOInput productDtoInput)  
    {
        var result = await _service.CreateProduct(productDtoInput);
        
        switch(result.isSuccess)
        {
            case true:
                return new CreatedAtRouteResult("GetProduct",
                    new { id = result.value.ProductId }, result.value);
            case false:
                _logger.LogWarning(
                    "Request failed {@Error}, {@RequestName}, {@DateTime}",
                    result.error,
                    nameof(_service.CreateProduct),
                    DateTime.Now
                );
                return BadRequest(result.GenerateErrorResponse());
        }
    }
    
    [HttpPut("{id:int:min(1)}")]
    [Authorize("AdminOnly")]
    public async Task<ActionResult<ProductDTOOutput>> Put(int id, [FromBody] ProductDTOInput productDtoInput)
    {
        _logger.LogInformation("Entrando no metodo do servico");
        var result = await _service.UpdateProduct(productDtoInput, id);

        switch (result.isSuccess)
        {
            case true:
                return Ok(new { message = $"Product with id = {result.value.ProductId} was updated successfully" });
            case false:
                _logger.LogWarning(
                   "Request failed {@Error}, {@RequestName}, {@DateTime}",
                   result.error,
                   nameof(_service.UpdateProduct),
                   DateTime.Now
                );
                if (result.error.HttpStatusCode == HttpStatusCode.NotFound)
                    return NotFound(result.error.Description);
                
                return BadRequest(result.GenerateErrorResponse());
        }
    }
    
    [HttpDelete("{id:int:min(1)}")]
    [Authorize("AdminOnly")]
    public async Task<ActionResult<ProductDTOOutput>> Delete(int id)
    {
        var result = await _service.DeleteProduct(id);
        
        switch(result.isSuccess)
        {
            case true:
                return Ok( new { message = $"Category with id = {result.value.ProductId} was deleted successfully" });
            case false:
                _logger.LogWarning(
                    "Request failed {@Error}, {@RequestName}, {@DateTime}",
                    result.error,
                    nameof(_service.DeleteProduct),
                    DateTime.Now
                );
                return NotFound(result.GenerateErrorResponse());
        }
    }
}