using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sales.Application.DTOs.WorkDayDTO;
using Sales.Application.Interfaces;

namespace Sales.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WorkDaysController : ControllerBase
{
    private readonly ILogger<WorkDaysController> _logger;
    private readonly IWorkDayService _service;

    public WorkDaysController(IWorkDayService service, ILogger<WorkDaysController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpGet("{id:int:min(1)}")]
    [Authorize]
    public async Task<ActionResult<WorkDayDTOOutput>> GetWorkDay(int id)
    {
        var result = await _service.GetWorkDayByIdAsync(id);
        
        switch(result.isSuccess)
        {
            case true:
                return Ok(result.value);
            case false:
                _logger.LogWarning(
                    "Request failed {@Error}, {@RequestName}, {@DateTime}",
                    result.error,
                    nameof(_service.GetWorkDayByIdAsync),
                    DateTime.Now
                );
                return NotFound(result.GenerateErrorResponse());
        }
    }
    
    [HttpGet("Date")]
    [Authorize]
    public async Task<ActionResult<WorkDayDTOOutput>> GetWorkDayByDate([FromQuery] DateTime date)
    {
        var result = await _service.GetWorkDayByDateAsync(date);
        
        switch(result.isSuccess)
        {
            case true:
                return Ok(result.value);
            case false:
                _logger.LogWarning(
                    "Request failed {@Error}, {@RequestName}, {@DateTime}",
                    result.error,
                    nameof(_service.GetWorkDayByIdAsync),
                    DateTime.Now
                );
                return NotFound(result.GenerateErrorResponse());
        }
    }
    
    [HttpPost("StartWorkDay")]
    [Authorize]
    public async Task<ActionResult<WorkDayDTOOutput>> StartWorkDay([FromBody] WorkDayDTOInput input, [FromQuery] int employeeId)
    {
        var result = await _service.StartWorkDay(input, employeeId);
        
        switch(result.isSuccess)
        {
            case true:
                return Ok(result.value);
            case false:
                _logger.LogWarning(
                    "Request failed {@Error}, {@RequestName}, {@DateTime}",
                    result.error,
                    nameof(_service.GetWorkDayByIdAsync),
                    DateTime.Now
                );
                if(result.error.HttpStatusCode == HttpStatusCode.NotFound)
                    return NotFound(result.GenerateErrorResponse());
                
                return BadRequest(result.GenerateErrorResponse());
        }
    }
    
    [HttpPost("FinishWorkDay/{workDayId:int:min(1)}")]
    [Authorize]
    public async Task<ActionResult<WorkDayDTOOutput>> FinishWorkDay(int workDayId, [FromQuery] int employeeId)
    {
        var result = await _service.FinishWorkDay(workDayId, employeeId);
        
        switch(result.isSuccess)
        {
            case true:
                return Ok(result.value);
            case false:
                _logger.LogWarning(
                    "Request failed {@Error}, {@RequestName}, {@DateTime}",
                    result.error,
                    nameof(_service.GetWorkDayByIdAsync),
                    DateTime.Now
                );
                if(result.error.HttpStatusCode == HttpStatusCode.NotFound)
                    return NotFound(result.GenerateErrorResponse());
                
                return BadRequest(result.GenerateErrorResponse());
        }
    }
}