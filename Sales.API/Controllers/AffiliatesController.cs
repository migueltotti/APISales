using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sales.Application.DTOs.AffiliateDTO;
using Sales.Application.DTOs.CategoryDTO;
using Sales.Application.Interfaces;
using Sales.Application.Parameters.Extension;
using Sales.Application.Parameters.ModelsParameters;

namespace Sales.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AffiliatesController(IAffiliateService service) : Controller
{
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<AffiliateDTOOutput>>> Get([FromQuery] AffiliateParameters parameters)
    {
        var affiliates = await service.GetAllAffiliate(parameters);

        var metadata = affiliates.GenerateMetadataHeader();
        
        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(metadata));
        
        return Ok(affiliates.ToList());
    }

    [HttpGet("{id:int:min(1)}", Name = "GetAffiliate")]
    public async Task<ActionResult<AffiliateDTOOutput>> GetAffiliate(int id)  
    {
        var result = await service.GetAffiliateBy(c => c.AffiliateId == id);

        return result.isSuccess switch
        {
            true => Ok(result.value),
            false => NotFound(result.GenerateErrorResponse())
        };
    }

    [HttpPost]
    [Authorize("AdminOnly")]
    public async Task<ActionResult<AffiliateDTOOutput>> Post(AffiliateDTOInput affiliateDtoInput)
    {
        var result = await service.CreateAffiliate(affiliateDtoInput);
        
        return result.isSuccess switch
        {
            true => new CreatedAtRouteResult("GetAffiliate",
                new { id = result.value.AffiliateId }, result.value),
            false => BadRequest(result.GenerateErrorResponse())
        };
    }

    [HttpPut("{id:int:min(1)}")]
    [Authorize("AdminOnly")]
    public async Task<ActionResult<AffiliateDTOOutput>> Put(int id, [FromBody] AffiliateDTOInput affiliateDtoInput)
    {
        var result = await service.UpdateAffiliate(affiliateDtoInput, id);

        switch(result.isSuccess)
        {
            case true:
                return Ok($"Affiliate with id = {result.value.AffiliateId} was updated successfully");
            case false:
                if (result.error.HttpStatusCode == System.Net.HttpStatusCode.NotFound)
                    return NotFound(result.GenerateErrorResponse());
                
                return BadRequest(result.GenerateErrorResponse());
        };
    }
    
    [HttpDelete("{id:int:min(1)}")]
    [Authorize("AdminOnly")]
    public async Task<ActionResult<AffiliateDTOOutput>> Delete(int id)
    {
        var result = await service.DeleteAffiliate(id);

        return result.isSuccess switch
        {
            true => Ok($"Affiliate with id = {result.value.AffiliateId} was deleted successfully"),
            false => NotFound(result.GenerateErrorResponse())
        };
    }
}