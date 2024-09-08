using Sales.API.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Sales.API.DTOs.ProductDTO;
using Sales.API.Models;
using Sales.API.Repositories.Interfaces;

namespace Sales.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductsController(IUnitOfWork _uof, IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDTOOutput>>> Get()    
    {
        var products = await _uof.ProductRepository.GetAllAsync();

        var productsDto = mapper.Map<IEnumerable<ProductDTOOutput>>(products);
        
        return Ok(productsDto);
    }
    
    [HttpGet("{id:int:min(1)}", Name = "GetProduct")]
    public async Task<ActionResult<ProductDTOOutput>> Get(int id)
    {
        var product = await _uof.ProductRepository.GetAsync(p => p.ProductId == id);

        if (product is null)
        {
            return BadRequest("Incorrect Data: product not found");
        }

        var productDto = mapper.Map<ProductDTOOutput>(product);
        
        return Ok(productDto);
    }
    
    [HttpPost]
    public async Task<ActionResult<ProductDTOOutput>> Post([FromBody] ProductDTOInput productDtoInput)  
    {
        if (productDtoInput is null)
        {
            return BadRequest("Incorrect Data: null");
        }

        var product = mapper.Map<Product>(productDtoInput);

        var productCreated = _uof.ProductRepository.Create(product);
        await _uof.CommitChanges();

        var productDtoCreated = mapper.Map<ProductDTOOutput>(productCreated);   

        return new CreatedAtRouteResult("GetProduct",
            new { id = productDtoCreated.ProductId },
            productDtoCreated);
    }
    
    [HttpPut("{id:int:min(1)}")]
    public async Task<ActionResult<ProductDTOOutput>> Put(int id, [FromBody] ProductDTOOutput productDtoInput)  
    {
        if (productDtoInput is null)
        {
            return BadRequest("Incorrect Data: null");
        }

        if (id != productDtoInput.ProductId)
        {
            return BadRequest("Incorrect Data: id mismatch");
        }

        var product = mapper.Map<Product>(productDtoInput);

        var productUpdated = _uof.ProductRepository.Update(product);
        await _uof.CommitChanges();

        var productDtoUpdated = mapper.Map<ProductDTOOutput>(productUpdated);

        return Ok(productDtoUpdated);
    }

    // DELETE api/<OrdersController>/5
    [HttpDelete("{id:int:min(1)}")]
    public async Task<ActionResult<ProductDTOOutput>> Delete(int id)
    {
        var product = await _uof.ProductRepository.GetAsync(p => p.ProductId == id);
        
        if (product is null)
        {
            return BadRequest("Incorrect Data: product not found");
        }

        var productDeleted = _uof.ProductRepository.Delete(product);
        await _uof.CommitChanges();

        var productDtoDeleted = mapper.Map<ProductDTOOutput>(productDeleted);

        return Ok(productDtoDeleted);
    }
}