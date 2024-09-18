using System.Net;
using System.Text;
using System.Text.Json;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Sales.Application.DTOs.UserDTO;
using Sales.Application.Interfaces;
using Sales.Application.Parameters.Extension;
using Sales.Application.Parameters.ModelsParameters;
using Sales.Domain.Interfaces;
using Sales.Domain.Models;

namespace Sales.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController(IUserService _service) : ControllerBase
{

    [HttpGet("getUsers")]
    public async Task<ActionResult<IEnumerable<UserDTOOutput>>> Get([FromQuery] UserParameters parameters)
    {
        var usersPaged = await _service.GetAllUsers(parameters);

        var metadata = usersPaged.GenerateMetadataHeader();
        
        Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(metadata));
        
        return Ok(usersPaged.ToList());
    }

    [HttpGet("{id:int:min(1)}", Name = "GetUser")]
    public async Task<ActionResult<UserDTOOutput>> GetUser(int id)
    {
        var result = await _service.GetUserBy(u => u.UserId == id);

        return result.isSuccess switch
        {
            true => Ok(result.value),
            false => NotFound(result.GenerateErrorResponse())
        };
    }

    [HttpGet("Orders/{id:int:min(1)}")]
    public async Task<ActionResult<UserDTOOutput>> GetUserOrders(int id)
    {
        /*var userOrder = await _uof.UserRepository.GetUserOrders(id);

        if (userOrder is null)
        {
            return NotFound($"User with id = {id} not found");
        }

        var userDtoOrder = mapper.Map<UserDTOOutput>(userOrder);

        return Ok(userDtoOrder);*/
        return Ok("Not implemented");
    }

    [HttpGet("Orders")]
    public async Task<ActionResult<IEnumerable<UserDTOOutput>>> GetUsersOrders()
    {
        /*var usersOrders = await _uof.UserRepository.GetUsersOrders();

        var usersDtoOrders = mapper.Map<IEnumerable<UserDTOOutput>>(usersOrders);
        
        return Ok(usersDtoOrders);*/
        return Ok("Not implemented");
    }

    [HttpPost]
    public async Task<ActionResult<UserDTOOutput>> Post(UserDTOInput userDtoInput)
    {
        var result = await _service.CreateUser(userDtoInput);
        
        return result.isSuccess switch
        {
            true => CreatedAtRoute("GetUser", 
                new { id = result.value.UserId }, result.value),
            false => BadRequest(result.GenerateErrorResponse())
        };
    }

    [HttpPut("{id:int:min(1)}")]
    public async Task<ActionResult<UserDTOOutput>> Put(UserDTOInput userDtoInput, int id)
    {
        var result = await _service.UpdateUser(userDtoInput, id);

        switch (result.isSuccess)
        {
            case true:
                return Ok($"Category with id = {result.value.UserId} was updated successfully");
            case false:
                if(result.error.HttpStatusCode == HttpStatusCode.NotFound)
                    return NotFound(result.GenerateErrorResponse());
                
                return BadRequest(result.GenerateErrorResponse());
        }
    }

    [HttpDelete("{id:int:min(1)}")]
    public async Task<ActionResult<UserDTOOutput>> Delete(int id)
    {
        var result = await _service.DeleteUser(id);
        
        return result.isSuccess switch
        {
            true => Ok($"Category with id = {result.value.UserId} was deleted successfully"),
            false => NotFound(result.GenerateErrorResponse())
        };
    }
}