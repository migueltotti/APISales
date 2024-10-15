using System.Data;
using System.Linq.Expressions;
using System.Net;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Sales.API.Controllers;
using Sales.Application.DTOs.TokenDTO;
using Sales.Application.DTOs.UserDTO;
using Sales.Application.Interfaces;
using Sales.Application.Parameters;
using Sales.Application.Parameters.ModelsParameters;
using Sales.Application.ResultPattern;
using Sales.Domain.Models;
using Sales.Infrastructure.Identity;
using X.PagedList.Extensions;

namespace Sales.Test.ControllersTests;

public class UsersControllerTest
{
    private readonly IUserService _mockUserService;
    private readonly UserManager<ApplicationUser> _mockUserManager;
    private readonly Fixture _fixture;
    private readonly UsersController _controller;

    public UsersControllerTest()
    {
        _mockUserService = Substitute.For<IUserService>();
        _mockUserManager = Substitute.For<UserManager<ApplicationUser>>(
            Substitute.For<IUserStore<ApplicationUser>>(),
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null
            );
        _fixture = new Fixture();
        
        _controller = new UsersController(_mockUserService, _mockUserManager)
        {
            ControllerContext =
            {
                HttpContext = new DefaultHttpContext()
            }
        };
    }

    [Fact]
    public async Task Get_ShouldReturnAllUsers()
    {
        // Arrange
        var users = _fixture.CreateMany<UserDTOOutput>(3).ToPagedList();
        var parameters = new QueryStringParameters();
        
        _mockUserService.GetAllUsers(Arg.Any<QueryStringParameters>()).Returns(users);
        
        // Act
        var result = await _controller.Get(parameters);
        var obj = result.Result as ObjectResult;
        
        // Assert
        result.Result.Should().NotBeNull();
        result.Should().BeOfType<ActionResult<IEnumerable<UserDTOOutput>>>();
        result.Result.Should().BeOfType<OkObjectResult>()
            .Which.StatusCode.Should().Be(200);
        obj.Value.Should().BeEquivalentTo(users.ToList());
        
        var httpContext = _controller.ControllerContext.HttpContext;
        httpContext.Response.Headers.ContainsKey("X-Pagination").Should().BeTrue();

        await _mockUserService.Received(1).GetAllUsers(Arg.Any<QueryStringParameters>());
    }
    
    [Fact]
    public async Task GetUsersByName_ShouldReturnAllUsersThatMatchSpecifiedName()
    {
        // Arrange
        var users = _fixture.CreateMany<UserDTOOutput>(3).ToPagedList();
        var parameters = new UserParameters();
        
        _mockUserService.GetUsersWithFilter("name", Arg.Any<UserParameters>()).Returns(users);
        
        // Act
        var result = await _controller.GetUsersByName(parameters);
        var obj = result.Result as ObjectResult;
        
        // Assert
        result.Result.Should().NotBeNull();
        result.Should().BeOfType<ActionResult<IEnumerable<UserDTOOutput>>>();
        result.Result.Should().BeOfType<OkObjectResult>()
            .Which.StatusCode.Should().Be(200);
        obj.Value.Should().BeEquivalentTo(users.ToList());
        
        var httpContext = _controller.ControllerContext.HttpContext;
        httpContext.Response.Headers.ContainsKey("X-Pagination").Should().BeTrue();

        await _mockUserService.Received(1).GetUsersWithFilter("name", Arg.Any<UserParameters>());
    }
    
    [Fact]
    public async Task GetUserByCpf_ShouldReturnUserThatMatchSpecifiedCpf_WhenUserExists()
    {
        // Arrange
        var user = _fixture.Create<UserDTOOutput>();
        var userResult = Result<UserDTOOutput>.Success(user);
        var parameters = new UserParameters();
        
        _mockUserService.GetUserBy(Arg.Any<Expression<Func<User, bool>>>()).Returns(userResult);
        
        // Act
        var result = await _controller.GetUsersByCpf(parameters);
        var obj = result.Result as ObjectResult;
        
        // Assert
        result.Result.Should().NotBeNull();
        result.Should().BeOfType<ActionResult<UserDTOOutput>>();
        result.Result.Should().BeOfType<OkObjectResult>()
            .Which.StatusCode.Should().Be(200);
        obj.Value.Should().BeEquivalentTo(userResult.value);

        await _mockUserService.Received(1).GetUserBy(Arg.Any<Expression<Func<User, bool>>>());
    }
    
    [Fact]
    public async Task GetUserByCpf_ShouldReturnUserThatMatchSpecifiedCpf_WhenUsersDoesNotExist()
    {
        // Arrange
        var error = _fixture.Create<Error>();
        var userResult = Result<UserDTOOutput>.Failure(error);
        var parameters = new UserParameters();
        
        _mockUserService.GetUserBy(Arg.Any<Expression<Func<User, bool>>>()).Returns(userResult);
        
        // Act
        var result = await _controller.GetUsersByCpf(parameters);
        var obj = result.Result as ObjectResult;
        
        // Assert
        result.Result.Should().NotBeNull();
        result.Should().BeOfType<ActionResult<UserDTOOutput>>();
        result.Result.Should().BeOfType<NotFoundObjectResult>()
            .Which.StatusCode.Should().Be(404);
        obj.Value.Should().BeEquivalentTo(userResult.GenerateErrorResponse());

        await _mockUserService.Received(1).GetUserBy(Arg.Any<Expression<Func<User, bool>>>());
    }
    
    [Fact]
    public async Task GetUsersByRole_ShouldReturnAllUsersThatMatchSpecifiedRole()
    {
        // Arrange
        var users = _fixture.CreateMany<UserDTOOutput>(3).ToPagedList();
        var parameters = new UserParameters();
        
        _mockUserService.GetUsersWithFilter("role", Arg.Any<UserParameters>()).Returns(users);
        
        // Act
        var result = await _controller.GetUsersByRole(parameters);
        var obj = result.Result as ObjectResult;
        
        // Assert
        result.Result.Should().NotBeNull();
        result.Should().BeOfType<ActionResult<IEnumerable<UserDTOOutput>>>();
        result.Result.Should().BeOfType<OkObjectResult>()
            .Which.StatusCode.Should().Be(200);
        obj.Value.Should().BeEquivalentTo(users.ToList());
        
        var httpContext = _controller.ControllerContext.HttpContext;
        httpContext.Response.Headers.ContainsKey("X-Pagination").Should().BeTrue();

        await _mockUserService.Received(1).GetUsersWithFilter("role", Arg.Any<UserParameters>());
    }
    
    [Fact]
    public async Task GetUserById_ShouldReturnUserThatMatchSpecifiedId_WhenUserExists()
    {
        // Arrange
        var user = _fixture.Create<UserDTOOutput>();
        var userId = _fixture.Create<int>();
        var userResult = Result<UserDTOOutput>.Success(user);
        
        _mockUserService.GetUserBy(Arg.Any<Expression<Func<User, bool>>>()).Returns(userResult);
        
        // Act
        var result = await _controller.GetUser(userId);
        var obj = result.Result as ObjectResult;
        
        // Assert
        result.Result.Should().NotBeNull();
        result.Should().BeOfType<ActionResult<UserDTOOutput>>();
        result.Result.Should().BeOfType<OkObjectResult>()
            .Which.StatusCode.Should().Be(200);
        obj.Value.Should().BeEquivalentTo(userResult.value);

        await _mockUserService.Received(1).GetUserBy(Arg.Any<Expression<Func<User, bool>>>());
    }
    
    [Fact]
    public async Task GetUserById_ShouldReturnUserThatMatchSpecifiedId_WhenUsersDoesNotExist()
    {
        // Arrange
        var error = _fixture.Create<Error>();
        var userId = _fixture.Create<int>();
        var userResult = Result<UserDTOOutput>.Failure(error);
        
        _mockUserService.GetUserBy(Arg.Any<Expression<Func<User, bool>>>()).Returns(userResult);
        
        // Act
        var result = await _controller.GetUser(userId);
        var obj = result.Result as ObjectResult;
        
        // Assert
        result.Result.Should().NotBeNull();
        result.Should().BeOfType<ActionResult<UserDTOOutput>>();
        result.Result.Should().BeOfType<NotFoundObjectResult>()
            .Which.StatusCode.Should().Be(404);
        obj.Value.Should().BeEquivalentTo(userResult.GenerateErrorResponse());

        await _mockUserService.Received(1).GetUserBy(Arg.Any<Expression<Func<User, bool>>>());
    }

    [Fact]
    public async Task Post_ShouldReturn200OkWithUser_WhenUserCreatedSuccessfully()
    {
        // Arrange
        var user = _fixture.Create<UserDTOInput>();
        var userCreated = _fixture.Create<UserDTOOutput>();
        var userCreatedResult = Result<UserDTOOutput>.Success(userCreated);

        _mockUserService.CreateUser(Arg.Any<UserDTOInput>()).Returns(userCreatedResult);
        _mockUserManager.CreateAsync(Arg.Any<ApplicationUser>(), Arg.Any<string>())
            .Returns(Task.FromResult(IdentityResult.Success));
        _mockUserManager.AddToRoleAsync(Arg.Any<ApplicationUser>(), Arg.Any<string>())
            .Returns(Task.FromResult(IdentityResult.Success));

        // Act
        var result = await _controller.Post(user);
        var obj = result.Result as ObjectResult;
        
        // Assert
        result.Result.Should().NotBeNull();
        result.Should().BeOfType<ActionResult<UserDTOOutput>>();
        result.Result.Should().BeOfType<CreatedAtRouteResult>()
            .Which.StatusCode.Should().Be(201);
        obj.Value.Should().BeEquivalentTo(userCreatedResult.value);

        await _mockUserService.Received(1).CreateUser(Arg.Any<UserDTOInput>());
        await _mockUserManager.Received(1).CreateAsync(Arg.Any<ApplicationUser>(), Arg.Any<string>());
        await _mockUserManager.Received(1).AddToRoleAsync(Arg.Any<ApplicationUser>(), Arg.Any<string>());
    }
    
    [Fact]
    public async Task Post_ShouldReturn400OBadRequestWithErrorResponse_WhenUserCreatedWithErrorsInUserService()
    {
        // Arrange
        var user = _fixture.Create<UserDTOInput>();
        var error = new Error("BadRequestError", "BadRequestError", HttpStatusCode.BadRequest);
        var userCreatedResult = Result<UserDTOOutput>.Failure(error);

        _mockUserService.CreateUser(Arg.Any<UserDTOInput>()).Returns(userCreatedResult);

        // Act
        var result = await _controller.Post(user);
        var obj = result.Result as ObjectResult;
        
        // Assert
        result.Result.Should().NotBeNull();
        result.Should().BeOfType<ActionResult<UserDTOOutput>>();
        result.Result.Should().BeOfType<BadRequestObjectResult>()
            .Which.StatusCode.Should().Be(400);
        obj.Value.Should().BeEquivalentTo(userCreatedResult.GenerateErrorResponse());

        await _mockUserService.Received(1).CreateUser(Arg.Any<UserDTOInput>());
    }
    
    [Fact]
    public async Task Post_ShouldReturn500InternalServerErrorWithErrorResponse_WhenUserCreatedWithErrorsInUserService()
    {
        // Arrange
        var user = _fixture.Create<UserDTOInput>();
        var error = new Error("UserAlreadyExists", "UserAlreadyExists", HttpStatusCode.InternalServerError);
        var userCreatedResult = Result<UserDTOOutput>.Failure(error);

        _mockUserService.CreateUser(Arg.Any<UserDTOInput>()).Returns(userCreatedResult);

        // Act
        var result = await _controller.Post(user);
        var obj = result.Result as ObjectResult;
        
        // Assert
        result.Result.Should().NotBeNull();
        result.Should().BeOfType<ActionResult<UserDTOOutput>>();
        obj.Value.Should().BeEquivalentTo(new Response { Status = "Error", Message = "User with this email already exists" });

        await _mockUserService.Received(1).CreateUser(Arg.Any<UserDTOInput>());
    }
    
    [Fact]
    public async Task Post_ShouldReturn500InternalServerErrorWithErrorResponse_WhenUserCreatedWithErrorsInUserManagerService()
    {
        // Arrange
        var user = _fixture.Create<UserDTOInput>();
        var userCreated = _fixture.Create<UserDTOOutput>();
        var userCreatedResult = Result<UserDTOOutput>.Success(userCreated);

        _mockUserService.CreateUser(Arg.Any<UserDTOInput>()).Returns(userCreatedResult);
        _mockUserManager.CreateAsync(Arg.Any<ApplicationUser>(), Arg.Any<string>())
            .Returns(Task.FromResult(IdentityResult.Failed()));

        // Act
        var result = await _controller.Post(user);
        var obj = result.Result as ObjectResult;
        
        // Assert
        result.Result.Should().NotBeNull();
        result.Should().BeOfType<ActionResult<UserDTOOutput>>();
        result.Result.Should().BeOfType<ObjectResult>()
            .Which.StatusCode.Should().Be(500);
        obj.Value.Should().BeEquivalentTo(new Response { Status = "Error", Message = "User creation failed" });

        await _mockUserService.Received(1).CreateUser(Arg.Any<UserDTOInput>());
        await _mockUserManager.Received(1).CreateAsync(Arg.Any<ApplicationUser>(), Arg.Any<string>());
    }
    
    [Fact]
    public async Task Post_ShouldReturn500InternalServerErrorWithErrorResponse_WhenRoleAddedToUserWithErrorsInUserManagerService()
    {
        // Arrange
        var user = _fixture.Create<UserDTOInput>();
        var userCreated = _fixture.Create<UserDTOOutput>();
        var userCreatedResult = Result<UserDTOOutput>.Success(userCreated);

        _mockUserService.CreateUser(Arg.Any<UserDTOInput>()).Returns(userCreatedResult);
        _mockUserManager.CreateAsync(Arg.Any<ApplicationUser>(), Arg.Any<string>())
            .Returns(Task.FromResult(IdentityResult.Success));
        _mockUserManager.AddToRoleAsync(Arg.Any<ApplicationUser>(), Arg.Any<string>())
            .Returns(Task.FromResult(IdentityResult.Failed()));

        // Act
        var result = await _controller.Post(user);
        var obj = result.Result as ObjectResult;
        
        // Assert
        result.Result.Should().NotBeNull();
        result.Should().BeOfType<ActionResult<UserDTOOutput>>();
        result.Result.Should().BeOfType<ObjectResult>()
            .Which.StatusCode.Should().Be(500);
        obj.Value.Should().BeEquivalentTo(new Response { Status = "Error", Message = "Assign user to role failed" });

        await _mockUserService.Received(1).CreateUser(Arg.Any<UserDTOInput>());
        await _mockUserManager.Received(1).CreateAsync(Arg.Any<ApplicationUser>(), Arg.Any<string>());
        await _mockUserManager.Received(1).AddToRoleAsync(Arg.Any<ApplicationUser>(), Arg.Any<string>());
    }

    [Fact]
    public async Task Put_ShouldReturn200OkWithSuccessResponse_WhenUserUpdatedSuccessfully()
    {
        // Arrange
        var userInput = _fixture.Create<UserDTOInput>();
        var userId = userInput.UserId;
        var user = _fixture.Create<UserDTOOutput>();
        var userResult = Result<UserDTOOutput>.Success(user);
        
        _mockUserService.UpdateUser(Arg.Any<UserDTOInput>(), Arg.Any<int>()).Returns(userResult);

        var result = await _controller.Put(userInput, userId);
        var obj = result.Result as ObjectResult;
        
        // Assert
        result.Result.Should().NotBeNull();
        result.Should().BeOfType<ActionResult<UserDTOOutput>>();
        result.Result.Should().BeOfType<OkObjectResult>()
            .Which.StatusCode.Should().Be(200);
        obj.Value.Should().BeEquivalentTo($"User with id = {userResult.value.UserId} was updated successfully");
        
        await _mockUserService.Received(1).UpdateUser(Arg.Any<UserDTOInput>(), Arg.Any<int>());
    }
    
    [Fact]
    public async Task Put_ShouldReturn404NotFoundWithErrorResponse_WhenUserUpdatedWithErrors()
    {
        // Arrange
        var userInput = _fixture.Create<UserDTOInput>();
        var userId = userInput.UserId;
        var error = new Error("NotFoundError", "NotFoundError", HttpStatusCode.NotFound);
        var userResult = Result<UserDTOOutput>.Failure(error);
        
        _mockUserService.UpdateUser(Arg.Any<UserDTOInput>(), Arg.Any<int>()).Returns(userResult);

        var result = await _controller.Put(userInput, userId);
        var obj = result.Result as ObjectResult;
        
        // Assert
        result.Result.Should().NotBeNull();
        result.Should().BeOfType<ActionResult<UserDTOOutput>>();
        result.Result.Should().BeOfType<NotFoundObjectResult>()
            .Which.StatusCode.Should().Be(404);
        obj.Value.Should().BeEquivalentTo(userResult.GenerateErrorResponse());
        
        await _mockUserService.Received(1).UpdateUser(Arg.Any<UserDTOInput>(), Arg.Any<int>());
    }
    
    [Fact]
    public async Task Put_ShouldReturn400BadRequestWithErrorResponse_WhenUserUpdatedWithErrors()
    {
        // Arrange
        var userInput = _fixture.Create<UserDTOInput>();
        var userId = userInput.UserId;
        var error = new Error("BadRequestError", "BadRequestError", HttpStatusCode.BadRequest);
        var userResult = Result<UserDTOOutput>.Failure(error);
        
        _mockUserService.UpdateUser(Arg.Any<UserDTOInput>(), Arg.Any<int>()).Returns(userResult);

        var result = await _controller.Put(userInput, userId);
        var obj = result.Result as ObjectResult;
        
        // Assert
        result.Result.Should().NotBeNull();
        result.Should().BeOfType<ActionResult<UserDTOOutput>>();
        result.Result.Should().BeOfType<BadRequestObjectResult>()
            .Which.StatusCode.Should().Be(400);
        obj.Value.Should().BeEquivalentTo(userResult.GenerateErrorResponse());
        
        await _mockUserService.Received(1).UpdateUser(Arg.Any<UserDTOInput>(), Arg.Any<int>());
    }
    
    [Fact]
    public async Task Delete_ShouldReturn200OkWithSuccessResponse_WhenUserDeletedSuccessfully()
    {
        // Arrange
        var userId = _fixture.Create<int>();
        var user = _fixture.Create<UserDTOOutput>();
        var userResult = Result<UserDTOOutput>.Success(user);
        
        _mockUserService.DeleteUser(Arg.Any<int>()).Returns(userResult);

        var result = await _controller.Delete(userId);
        var obj = result.Result as ObjectResult;
        
        // Assert
        result.Result.Should().NotBeNull();
        result.Should().BeOfType<ActionResult<UserDTOOutput>>();
        result.Result.Should().BeOfType<OkObjectResult>()
            .Which.StatusCode.Should().Be(200);
        obj.Value.Should().BeEquivalentTo($"User with id = {userResult.value.UserId} was deleted successfully");
        
        await _mockUserService.Received(1).DeleteUser(Arg.Any<int>());
    }
    
    [Fact]
    public async Task Delete_ShouldReturn404NotFoundWithErrorResponse_WhenUserDeletedWithErrors()
    {
        // Arrange
        var userId = _fixture.Create<int>();
        var error = _fixture.Create<Error>();
        var userResult = Result<UserDTOOutput>.Failure(error);
        
        _mockUserService.DeleteUser(Arg.Any<int>()).Returns(userResult);

        var result = await _controller.Delete(userId);
        var obj = result.Result as ObjectResult;
        
        // Assert
        result.Result.Should().NotBeNull();
        result.Should().BeOfType<ActionResult<UserDTOOutput>>();
        result.Result.Should().BeOfType<NotFoundObjectResult>()
            .Which.StatusCode.Should().Be(404);
        obj.Value.Should().BeEquivalentTo(userResult.GenerateErrorResponse());
        
        await _mockUserService.Received(1).DeleteUser(Arg.Any<int>());
    }
}