using System.Net;
using System.Text;
using System.Text.Json;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Sales.Application.DTOs.TokenDTO;
using Sales.Application.DTOs.UserDTO;
using Sales.Application.Interfaces;
using Sales.Application.Parameters;
using Sales.Application.Parameters.Extension;
using Sales.Application.Parameters.ModelsParameters;
using Sales.Domain.Interfaces;
using Sales.Domain.Models;
using Sales.Domain.Models.Enums;
using Sales.Infrastructure.Identity;

namespace Sales.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController(IUserService _service, UserManager<ApplicationUser> _userManager) : ControllerBase
{

    [HttpGet("getUsers")]
    [Authorize("AdminEmployeeOnly")]
    public async Task<ActionResult<IEnumerable<UserDTOOutput>>> Get([FromQuery] QueryStringParameters parameters)
    {
        var usersPaged = await _service.GetAllUsers(parameters);

        var metadata = usersPaged.GenerateMetadataHeader();
        
        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(metadata));
        
        return Ok(usersPaged.ToList());
    }
    
    [HttpGet("name")]
    [Authorize("AdminEmployeeOnly")]
    public async Task<ActionResult<IEnumerable<UserDTOOutput>>> GetUsersByName([FromQuery] UserParameters parameters)
    {
        var usersPaged = await _service.GetUsersWithFilter("name", parameters);
        
        var metadata = usersPaged.GenerateMetadataHeader();
        
        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(metadata));
        
        return Ok(usersPaged.ToList());
    }
    
    [HttpGet("cpf")]
    [Authorize("AdminEmployeeOnly")]
    public async Task<ActionResult<UserDTOOutput>> GetUsersByCpf([FromQuery] UserParameters parameters)
    {
        var result = await _service.GetUserBy(u => u.Cpf == parameters.Cpf);

        return result.isSuccess switch
        {
            true => Ok(result.value),
            false => NotFound(result.GenerateErrorResponse())
        };
    }
    
    [HttpGet("role")]
    [Authorize("AdminEmployeeOnly")]
    public async Task<ActionResult<IEnumerable<UserDTOOutput>>> GetUsersByRole([FromQuery] UserParameters parameters)
    {
        var usersPaged = await _service.GetUsersWithFilter("role", parameters);

        var metadata = usersPaged.GenerateMetadataHeader();
        
        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(metadata));
        
        return Ok(usersPaged.ToList());
    }

    [HttpGet("{id:int:min(1)}", Name = "GetUser")]
    [Authorize("AdminEmployeeOnly")]
    public async Task<ActionResult<UserDTOOutput>> GetUser(int id)
    {
        var result = await _service.GetUserBy(u => u.UserId == id);

        return result.isSuccess switch
        {
            true => Ok(result.value),
            false => NotFound(result.GenerateErrorResponse())
        };
    }

    // Users unauthorized can create a User
    
    [HttpPost]
    public async Task<ActionResult<UserDTOOutput>> Post(UserDTOInput userDtoInput)
    {
        var result = await _service.CreateUser(userDtoInput);

        if (result.isSuccess)
        {
            ApplicationUser user = new()
            {
                Email = userDtoInput.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = userDtoInput.Name.Replace(" ", "")
            };

            var resultUser = await _userManager.CreateAsync(user, userDtoInput.Password);

            if (!resultUser.Succeeded)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError,
                    new Response { Status = "Error", Message = "User creation failed" });
            }
            
            var resultUserRole = await _userManager.AddToRoleAsync(user, userDtoInput.Role.ToString());

            if (!resultUserRole.Succeeded)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError,
                    new Response { Status = "Error", Message = "Assign user to role failed" });
            }
        }

        switch (result.isSuccess)
        {
            case true:
                return CreatedAtRoute("GetUser",
                    new { id = result.value.UserId }, result.value);
            case false:
                if (result.error.HttpStatusCode == HttpStatusCode.BadRequest)
                    return BadRequest(result.GenerateErrorResponse());
                return StatusCode((int)HttpStatusCode.InternalServerError,
                    new Response { Status = "Error", Message = "User with this email already exists" });
        };
    }
    
    [HttpPut("{id:int:min(1)}")]
    [Authorize("AllowAnyUser")]
    public async Task<ActionResult<UserDTOOutput>> Put(UserDTOInput userDtoInput, int id)
    {
        var result = await _service.UpdateUser(userDtoInput, id);

        switch (result.isSuccess)
        {
            case true:
                return Ok($"User with id = {result.value.UserId} was updated successfully");
            case false:
                if(result.error.HttpStatusCode == HttpStatusCode.NotFound)
                    return NotFound(result.GenerateErrorResponse());
                
                return BadRequest(result.GenerateErrorResponse());
        }
    }

    [HttpDelete("{id:int:min(1)}")]
    [Authorize("AllowAnyUser")]
    public async Task<ActionResult<UserDTOOutput>> Delete(int id)
    {
        var result = await _service.DeleteUser(id);
        
        return result.isSuccess switch
        {
            true => Ok($"User with id = {result.value.UserId} was deleted successfully"),
            false => NotFound(result.GenerateErrorResponse())
        };
    }
}