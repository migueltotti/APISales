using ApiSales.Models;
using ApiSales.Repositories;
using ApiSales.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ApiSales.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoriesController(IUnitOfWork _uof) : Controller
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Category>>> Get()
    {
        var categories = await _uof.CategoryRepository.GetAllAsync();

        return Ok(categories);
    }

    [HttpGet("{id:int:min(1)}", Name = "GetCategory")]
    public async Task<ActionResult<Category>> GetCategory(int id)
    {
        var category = await _uof.CategoryRepository.GetAsync(c => c.CategoryId == id);

        if (category is null)
        {
            return NotFound($"Category with id = {id} NotFound!");
        }

        return Ok(category);
    }

    [HttpPost]
    public async Task<ActionResult<Category>> Post(Category category)
    {
        if (category is null)
        {
            return BadRequest("Incorrect Data: null");
        }

        var categoryCreated = _uof.CategoryRepository.Create(category);
        await _uof.CommitChanges();

        return new CreatedAtRouteResult(nameof(GetCategory),
            new { id = categoryCreated.CategoryId },
            categoryCreated);
    }

    [HttpPut("{id:int:min(1)}")]
    public async Task<ActionResult<Category>> Put(int id, [FromBody] Category category)
    {
        if (category is null)
        {
            return BadRequest("Incorrect Data: null"); 
        }

        if (id != category.CategoryId)
        {
            return BadRequest("Incorrect Data: id mismatch");
        }

        var categoryUpdated = _uof.CategoryRepository.Update(category);
        await _uof.CommitChanges();

        return Ok(categoryUpdated);
    }
    
    [HttpDelete("{id:int:min(1)}")]
    public async Task<ActionResult<Category>> Delete(int id)
    {
        var category = await _uof.CategoryRepository.GetAsync(c => c.CategoryId == id);

        if (category is null)
        {
            return BadRequest("Incorrect Data: null"); 
        }
        
        var categoryDeleted = _uof.CategoryRepository.Delete(category);
        await _uof.CommitChanges();

        return Ok(categoryDeleted);
    }
}