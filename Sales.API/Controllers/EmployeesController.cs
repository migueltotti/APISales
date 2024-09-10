using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Sales.Application.DTOs.UserDTO;
using Sales.Domain.Interfaces;
using Sales.Domain.Models;

namespace Sales.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController(IUnitOfWork _uof, IMapper mapper) : ControllerBase
{

    [HttpGet("getUsers")]
    public async Task<ActionResult<IEnumerable<UserDTOOutput>>> Get()
    {
        var users = await _uof.UserRepository.GetAllAsync();

        var usersDto = mapper.Map<IEnumerable<UserDTOOutput>>(users);
        
        return Ok(usersDto);
    }

    [HttpGet("{id:int:min(1)}", Name = "GetUser")]
    public async Task<ActionResult<UserDTOOutput>> GetUser(int id)
    {
        var user = await _uof.UserRepository.GetAsync(e => e.UserId == id);

        if (user is null)
        {
            return NotFound($"User with id = {id} not found");
        }
        
        var userDto = mapper.Map<UserDTOOutput>(user);

        return Ok(userDto);
    }

    [HttpGet("Orders/{id:int:min(1)}")]
    public async Task<ActionResult<UserDTOOutput>> GetUserOrders(int id)
    {
        var userOrder = await _uof.UserRepository.GetUserOrders(id);

        if (userOrder is null)
        {
            return NotFound($"User with id = {id} not found");
        }

        var userDtoOrder = mapper.Map<UserDTOOutput>(userOrder);

        return Ok(userDtoOrder);
    }

    [HttpGet("Orders")]
    public async Task<ActionResult<IEnumerable<UserDTOOutput>>> GetUsersOrders()
    {
        var usersOrders = await _uof.UserRepository.GetUsersOrders();

        var usersDtoOrders = mapper.Map<IEnumerable<UserDTOOutput>>(usersOrders);
        
        return Ok(usersDtoOrders);
    }

    [HttpPost]
    public async Task<ActionResult<UserDTOOutput>> Post(UserDTOInput userDtoInput)
    {
        if (userDtoInput is null)
        {
            return BadRequest("User is null!!");
        }

        var user = mapper.Map<User>(userDtoInput);

        var userCreated = _uof.UserRepository.Create(user);
        await _uof.CommitChanges();

        var userDtoCreated = mapper.Map<UserDTOOutput>(userCreated);

        return new CreatedAtRouteResult("GetUser", 
            new { id = userDtoCreated.UserId },
            userDtoCreated);
    }

    [HttpPut("{id:int:min(1)}")]
    public async Task<ActionResult<UserDTOOutput>> Put(UserDTOInput userDtoInput, int id)
    {
        if (userDtoInput is null)
        {
            return BadRequest("Incorrect Data");
        }

        if (userDtoInput.UserId != id)
        {
            return BadRequest("Id does not match User Id");
        }

        var user = mapper.Map<User>(userDtoInput);

        var userUpdate = _uof.UserRepository.Update(user);
        await _uof.CommitChanges();

        var userDtoUpdated = mapper.Map<UserDTOOutput>(userUpdate);

        return Ok(userDtoUpdated);
    }

    [HttpDelete("{id:int:min(1)}")]
    public async Task<ActionResult<UserDTOOutput>> Delete(int id)
    {
        var user = await _uof.UserRepository.GetAsync(e => e.UserId == id);

        if (user is null)
        {
            return NotFound($"User with id = {id} Not Found!");
        }

        var userDeleted = _uof.UserRepository.Delete(user);
        await _uof.CommitChanges();

        var userDtoDeleted = mapper.Map<UserDTOOutput>(userDeleted);

        return Ok(userDtoDeleted);
    }
}