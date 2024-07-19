using ApiSales.DTOs.CategoryDTO;
using ApiSales.Models;
using ApiSales.Repositories.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace ApiSales.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoriesController(IUnitOfWork _uof, IMapper mapper) : Controller
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryDTOOutput>>> Get()
    {
        var categories = await _uof.CategoryRepository.GetAllAsync();

        var categoriesDto = mapper.Map<CategoryDTOOutput>(categories);
        
        return Ok(categoriesDto);
    }

    [HttpGet("{id:int:min(1)}", Name = "GetCategory")]
    public async Task<ActionResult<CategoryDTOOutput>> GetCategory(int id)
    {
        var category = await _uof.CategoryRepository.GetAsync(c => c.CategoryId == id);

        if (category is null)
        {
            return NotFound($"Category with id = {id} NotFound!");
        }

        var categoryDto = mapper.Map<CategoryDTOOutput>(category);

        return Ok(categoryDto);
    }

    [HttpPost]
    public async Task<ActionResult<CategoryDTOOutput>> Post(CategoryDTOInput categoryDtoInput)
    {
        if (categoryDtoInput is null)
        {
            return BadRequest("Incorrect Data: null");
        }

        var category = mapper.Map<Category>(categoryDtoInput);

        var categoryCreated = _uof.CategoryRepository.Create(category);
        await _uof.CommitChanges();

        var categoryDtoCreated = mapper.Map<CategoryDTOOutput>(categoryCreated);

        return new CreatedAtRouteResult(nameof(GetCategory),
            new { id = categoryDtoCreated.CategoryId },
            categoryDtoCreated);
    }

    [HttpPut("{id:int:min(1)}")]
    public async Task<ActionResult<CategoryDTOOutput>> Put(int id, [FromBody] CategoryDTOInput categoryDtoInput)
    {
        if (categoryDtoInput is null)
        {
            return BadRequest("Incorrect Data: null"); 
        }

        if (id != categoryDtoInput.CategoryId)
        {
            return BadRequest("Incorrect Data: id mismatch");
        }

        var category = mapper.Map<Category>(categoryDtoInput);

        var categoryUpdated = _uof.CategoryRepository.Update(category);
        await _uof.CommitChanges();

        var categoryDtoUpdated = mapper.Map<CategoryDTOInput>(categoryUpdated);

        return Ok(categoryDtoUpdated);
    }
    
    [HttpDelete("{id:int:min(1)}")]
    public async Task<ActionResult<CategoryDTOOutput>> Delete(int id)
    {
        var category = await _uof.CategoryRepository.GetAsync(c => c.CategoryId == id);

        if (category is null)
        {
            return BadRequest("Incorrect Data: null"); 
        }
        
        var categoryDeleted = _uof.CategoryRepository.Delete(category);
        await _uof.CommitChanges();

        var categoryDtoDeleted = mapper.Map<CategoryDTOOutput>(categoryDeleted);

        return Ok(categoryDtoDeleted);
    }
}