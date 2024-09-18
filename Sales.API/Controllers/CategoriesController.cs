using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Sales.Application.DTOs.CategoryDTO;
using Sales.Application.Interfaces;
using Sales.Application.Parameters;
using Sales.Application.Parameters.Extension;
using Sales.Application.Parameters.ModelsParameters;
using Sales.Application.Parameters.ModelsParameters.CategoryParameters;
using Sales.Application.Parameters.ModelsParameters.ProductParameters;

namespace Sales.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoriesController(ICategoryService service) : Controller
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
    public async Task<ActionResult<IEnumerable<CategoryDTOOutput>>> GetCategoriesByName([FromQuery] CategoryFilterName parameters)
    {
        var categories = await service.GetCategoriesByName(parameters);

        var metadata = categories.GenerateMetadataHeader();
        
        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(metadata));
        
        return Ok(categories.ToList());
    }

    [HttpGet("{id:int:min(1)}", Name = "GetCategory")]
    public async Task<ActionResult<CategoryDTOOutput>> GetCategory(int id)  
    {
        var result = await service.GetCategoryBy(c => c.CategoryId == id);

        return result.isSuccess switch
        {
            true => Ok(result.value),
            false => NotFound(result.GenerateErrorResponse())
        };
    }
    
    [HttpGet("{id:int:min(1)}/products")]
    public async Task<ActionResult<CategoryDTOOutput>> GetCategoryProducts(int id, [FromQuery] QueryStringParameters parameters)  
    {
        var categoryProducts = await service.GetProducts(id, parameters);

        var metadata = categoryProducts.GenerateMetadataHeader();
        
        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(metadata));

        return Ok(categoryProducts.ToList());
    }
    
    [HttpGet("{id:int:min(1)}/products/value")]
    public async Task<ActionResult<CategoryDTOOutput>> GetCategoryProductsByValue(int id, [FromQuery] ProductFilterValue parameters)  
    {
        var categoryProducts = await service.GetProductsByValue(id, parameters);

        var metadata = categoryProducts.GenerateMetadataHeader();
        
        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(metadata));

        return Ok(categoryProducts.ToList());
    }

    [HttpPost]
    public async Task<ActionResult<CategoryDTOOutput>> Post(CategoryDTOInput categoryDtoInput)
    {
        var result = await service.CreateCategory(categoryDtoInput);
        
        return result.isSuccess switch
        {
            true => new CreatedAtRouteResult("GetCategory",
                new { id = result.value.CategoryId }, result.value),
            false => BadRequest(result.GenerateErrorResponse())
        };
    }

    [HttpPut("{id:int:min(1)}")]
    public async Task<ActionResult<CategoryDTOOutput>> Put(int id, [FromBody] CategoryDTOInput categoryDtoInput)
    {
        var result = await service.UpdateCategory(categoryDtoInput, id);

        switch(result.isSuccess)
        {
            case true:
                return Ok($"Category with id = {result.value.CategoryId} was updated successfully");
            case false:
                if (result.error.HttpStatusCode == System.Net.HttpStatusCode.NotFound)
                    return NotFound(result.GenerateErrorResponse());
                
                return BadRequest(result.GenerateErrorResponse());
        };
    }
    
    [HttpDelete("{id:int:min(1)}")]
    public async Task<ActionResult<CategoryDTOOutput>> Delete(int id)
    {
        var result = await service.DeleteCategory(id);

        return result.isSuccess switch
        {
            true => Ok($"Category with id = {result.value.CategoryId} was deleted successfully"),
            false => NotFound(result.GenerateErrorResponse())
        };
    }
}