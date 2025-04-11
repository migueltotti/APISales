using System.Net;
using System.Text;
using System.Text.Json;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
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
public class UsersController(IUserService _service,
    IShoppingCartService _shoppingCartService,
    UserManager<ApplicationUser> _userManager,
    ILogger<UsersController> _logger) : ControllerBase
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
        
        switch(result.isSuccess)
        {
            case true:
                return Ok(result.value);
            case false:
                _logger.LogWarning(
                    "Request failed {@Error}, {@RequestName}, {@DateTime}",
                    result.error,
                    nameof(_service.GetUserBy),
                    DateTime.Now
                );
                return NotFound(result.GenerateErrorResponse());
        }
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
        //var result = await _service.GetUserBy(u => u.UserId == id);
        var result = await _service.GetUserById(id);
        
        switch(result.isSuccess)
        {
            case true:
                return Ok(result.value);
            case false:
                _logger.LogWarning(
                    "Request failed {@Error}, {@RequestName}, {@DateTime}",
                    result.error,
                    nameof(_service.GetUserById),
                    DateTime.Now
                );
                return NotFound(result.GenerateErrorResponse());
        }
    }
    
    [HttpGet("email/{email}")]
    [Authorize("AdminEmployeeOnly")]
    public async Task<ActionResult<UserDTOOutput>> GetUserByEmail(string email)
    {
        var result = await _service.GetUserByEmail(email);
        
        switch(result.isSuccess)
        {
            case true:
                return Ok(result.value);
            case false:
                _logger.LogWarning(
                    "Request failed {@Error}, {@RequestName}, {@DateTime}",
                    result.error,
                    nameof(_service.GetUserById),
                    DateTime.Now
                );
                return NotFound(result.GenerateErrorResponse());
        }
    }

    // Users unauthorized can create a User
    
    [HttpPost]
    public async Task<ActionResult<UserDTOOutput>> Post(UserDTOInput userDtoInput)
    {
        var result = await _service.CreateUser(userDtoInput);
        
        if (result.isSuccess)
        {
            var shoppingCartCreated = await _shoppingCartService
                .CreateShoppingCartAsync(result.value!.UserId);
            
            if (!shoppingCartCreated.isSuccess)
            {
                _logger.LogWarning(
                    "Request failed {@Error}, {@RequestName}, {@DateTime}",
                    shoppingCartCreated.error,
                    nameof(_shoppingCartService.CreateShoppingCartAsync),
                    DateTime.Now
                );
                return StatusCode((int)HttpStatusCode.InternalServerError,
                    new Response { Status = "Error", Message = "ShoppingCart creation failed" });
            }
            
            ApplicationUser user = new()
            {
                Email = userDtoInput.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = userDtoInput.GenerateUserName()
            };

            var resultUser = await _userManager.CreateAsync(user, userDtoInput.Password);

            if (!resultUser.Succeeded)
            {
                _logger.LogWarning(
                    "Request failed {@Error}, {@RequestName}, {@DateTime}",
                    resultUser.Errors,
                    nameof(_userManager.CreateAsync),
                    DateTime.Now
                );
                return StatusCode((int)HttpStatusCode.InternalServerError,
                    new Response { Status = "Error", Message = "User creation failed" });
            }
            
            var resultUserRole = await _userManager.AddToRoleAsync(user, userDtoInput.Role.ToString());

            if (!resultUserRole.Succeeded)
            {
                _logger.LogWarning(
                    "Request failed {@Error}, {@RequestName}, {@DateTime}",
                    result.error,
                    nameof(_userManager.AddToRoleAsync),
                    DateTime.Now
                );
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
                _logger.LogWarning(
                    "Request failed {@Error}, {@RequestName}, {@DateTime}",
                    result.error,
                    nameof(_service.CreateUser),
                    DateTime.Now
                );
                if (result.error.HttpStatusCode == HttpStatusCode.BadRequest)
                    return BadRequest(result.GenerateErrorResponse());
                return StatusCode((int)HttpStatusCode.InternalServerError,
                    new Response { Status = "Error", Message = "User with this email already exists" });
        };
    }
    
    [HttpPut("{id:int:min(1)}")]
    [Authorize("AllowAnyUser")]
    public async Task<ActionResult<UserDTOOutput>> Put(UserUpdateDTO userDtoInput, int id)
    {
        var result = await _service.UpdateUser(userDtoInput, id);

        if (!result.isSuccess)
        {
            _logger.LogWarning(
                "Request failed {@Error}, {@RequestName}, {@DateTime}",
                result.error,
                nameof(_service.UpdateUser),
                DateTime.Now
            );
            
            switch (result.error.HttpStatusCode)
            {
                case HttpStatusCode.NotFound:
                    return NotFound(result.GenerateErrorResponse());
                case HttpStatusCode.BadRequest:
                    return BadRequest(result.GenerateErrorResponse());
            }
        }
        
        // UserDataDbContext User Update logic.
        var userForUpdate = await GenerateUpdatedUser(userDtoInput, result.value.Item2);
        
        if(userForUpdate is not null) 
            await _userManager.UpdateAsync(userForUpdate);
        
        return Ok(result.value.Item1);
    }

    [HttpDelete("{userId:int:min(1)}")]
    [Authorize("AllowAnyUser")]
    public async Task<ActionResult<UserDTOOutput>> Delete(int userId)
    {        
        var shoppingCartDeleted = await _shoppingCartService.DeleteShoppingCartAsync(userId);
        
        var deleteUserResult = await _service.DeleteUser(userId);
        
        if (!deleteUserResult.isSuccess)
        {
            _logger.LogWarning(
                "Request failed {@Error}, {@RequestName}, {@DateTime}",
                deleteUserResult.error,
                nameof(_service.UpdateUser),
                DateTime.Now
            );
            return NotFound(deleteUserResult.GenerateErrorResponse());
        }
        
        // UserDataDbContext User Update logic implemented inside Action Method Delete in UsersController.

        var getUserResult = await _userManager.FindByEmailAsync(deleteUserResult.value.Email);
        var deleteUserAuthenticationResult = await _userManager.DeleteAsync(getUserResult);
        
        return Ok($"User with id = {deleteUserResult.value.UserId} was deleted successfully");
    }

    private async Task<ApplicationUser?> GenerateUpdatedUser(UserUpdateDTO userDtoInput, Dictionary<string, string> updatedFields)
    {
        ApplicationUser? userForUpdate;
        
        if (updatedFields.IsNullOrEmpty())
            return null;

        if (updatedFields.TryGetValue("Email", out var email))
        {
            userForUpdate = await _userManager
                .FindByEmailAsync(updatedFields["Email"]);
            
            userForUpdate.Email = userDtoInput.Email;
        }
        else
        {
            userForUpdate = await _userManager
                .FindByEmailAsync(userDtoInput.Email);
        }
        
        if (updatedFields.ContainsKey("Name"))
            userForUpdate.UserName = userDtoInput.GenerateUserName();
            
        return userForUpdate;
    } 
}