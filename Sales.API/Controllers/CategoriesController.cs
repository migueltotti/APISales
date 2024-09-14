using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Sales.Application.DTOs.CategoryDTO;
using Sales.Application.Interfaces;
using Sales.Application.ResultPattern;
using Sales.Domain.Interfaces;

namespace Sales.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoriesController(ICategoryService service) : Controller
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryDTOOutput>>> Get()
    {
        return Ok(await service.GetAllCategories());
    }

    [HttpGet("{id:int:min(1)}", Name = "GetCategory")]
    public async Task<ActionResult<CategoryDTOOutput>> GetCategory(int id)  
    {
        var result = await service.GetCategoryBy(c => c.CategoryId == id);

        return result.isSuccess switch
        {
            true => Ok(result.value),
            false => NotFound(result.error.Description)
        };
    }

    [HttpPost]
    public async Task<ActionResult<CategoryDTOOutput>> Post(CategoryDTOInput categoryDtoInput)
    {
        var result = await service.CreateCategory(categoryDtoInput);
        
        return result.isSuccess switch
        {
            true => new CreatedAtRouteResult("GetCategory",
                new { id = result.value.CategoryId }, result.value),
            false => BadRequest(result.error.Description)
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
                    return NotFound(result.error.Description);
                
                return BadRequest(result.error.Description);
        };
    }
    
    [HttpDelete("{id:int:min(1)}")]
    public async Task<ActionResult<CategoryDTOOutput>> Delete(int id)
    {
        var result = await service.DeleteCategory(id);

        return result.isSuccess switch
        {
            true => Ok($"Category with id = {result.value.CategoryId} was deleted successfully"),
            false => NotFound(result.error.Description)
        };
    }
}