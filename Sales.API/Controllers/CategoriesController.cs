using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Sales.API.Filter;
using Sales.Application.DTOs.CategoryDTO;
using Sales.Application.DTOs.ProductDTO;
using Sales.Application.Interfaces;
using Sales.Application.Parameters;
using Sales.Application.Parameters.Extension;
using Sales.Application.Parameters.ModelsParameters;
using Sales.Application.Parameters.ModelsParameters;

namespace Sales.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoriesController(ICategoryService service, ILogger<CategoriesController> _logger) : Controller
{    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryDTOOutput>>> Get([FromQuery] QueryStringParameters parameters)
    {
        var categories = await service.GetAllCategories(parameters);

        var metadata = categories.GenerateMetadataHeader();
        
        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(metadata));
        
        return Ok(categories.ToList());
    }
    
    [HttpGet("name")]
    public async Task<ActionResult<IEnumerable<CategoryDTOOutput>>> GetCategoriesByName([FromQuery] CategoryParameters parameters)
    {
        var categories = await service.GetCategoriesWithFilter("name", parameters);

        var metadata = categories.GenerateMetadataHeader();
        
        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(metadata));
        
        return Ok(categories.ToList());
    }

    [HttpGet("{id:int:min(1)}", Name = "GetCategory")]
    public async Task<ActionResult<CategoryDTOOutput>> GetCategory(int id)
    {
        //var result = await service.GetCategoryBy(c => c.CategoryId == id);
        var result = await service.GetCategoryById(id);
        
        switch(result.isSuccess)
        {
            case true:
                return Ok(result.value);
            case false:
                _logger.LogWarning(
                    "Request failed {@Error}, {@RequestName}, {@DateTime}",
                    result.error,
                    nameof(service.GetCategoryById),
                    DateTime.Now
                );
                return NotFound(result.GenerateErrorResponse());
        };
    }
    
    [HttpGet("{id:int:min(1)}/products")]
    public async Task<ActionResult<IEnumerable<ProductDTOOutput>>> GetCategoryProducts(int id, [FromQuery] QueryStringParameters parameters)  
    {
        var categoryProducts = await service.GetProducts(id, parameters);

        var metadata = categoryProducts.GenerateMetadataHeader();
        
        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(metadata));

        return Ok(categoryProducts.ToList());
    }
    
    [HttpGet("{id:int:min(1)}/products/value")]
    public async Task<ActionResult<IEnumerable<ProductDTOOutput>>> GetCategoryProductsByValue(int id, [FromQuery] ProductParameters parameters)  
    {
        var categoryProducts = await service.GetProductsByValue(id, parameters);

        var metadata = categoryProducts.GenerateMetadataHeader();
        
        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(metadata));

        return Ok(categoryProducts.ToList());
    }

    [HttpPost]
    [Authorize("AdminOnly")]
    public async Task<ActionResult<CategoryDTOOutput>> Post(CategoryDTOInput categoryDtoInput)
    {
        var result = await service.CreateCategory(categoryDtoInput);
        
        switch(result.isSuccess)
        {
            case true:
                return  new CreatedAtRouteResult("GetCategory",
                    new { id = result.value.CategoryId }, result.value);
            case false:
                _logger.LogWarning(
                    "Request failed {@Error}, {@RequestName}, {@DateTime}",
                    result.error,
                    nameof(service.CreateCategory),
                    DateTime.Now
                );
                return BadRequest(result.GenerateErrorResponse());
        };
    }

    [HttpPut("{id:int:min(1)}")]
    [Authorize("AdminOnly")]
    public async Task<ActionResult<CategoryDTOOutput>> Put(int id, [FromBody] CategoryDTOInput categoryDtoInput)
    {
        var result = await service.UpdateCategory(categoryDtoInput, id);

        switch(result.isSuccess)
        {
            case true:
                return Ok($"Category with id = {result.value.CategoryId} was updated successfully");
            case false:
                _logger.LogWarning(
                    "Request failed {@Error}, {@RequestName}, {@DateTime}",
                    result.error,
                    nameof(service.UpdateCategory),
                    DateTime.Now
                );
                
                if (result.error.HttpStatusCode == System.Net.HttpStatusCode.NotFound)
                    return NotFound(result.GenerateErrorResponse());
                
                return BadRequest(result.GenerateErrorResponse());
        };
    }
    
    [HttpDelete("{id:int:min(1)}")]
    [Authorize("AdminOnly")]
    public async Task<ActionResult<CategoryDTOOutput>> Delete(int id)
    {
        var result = await service.DeleteCategory(id);
        
        switch(result.isSuccess)
        {
            case true:
                return Ok($"Category with id = {result.value.CategoryId} was deleted successfully");
            case false:
                _logger.LogWarning(
                    "Request failed {@Error}, {@RequestName}, {@DateTime}",
                    result.error,
                    nameof(service.DeleteCategory),
                    DateTime.Now
                );
                return NotFound(result.GenerateErrorResponse());
        };
    }
}