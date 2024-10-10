using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AutoFixture;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NuGet.Protocol;
using Sales.API.Controllers;
using Sales.Application.DTOs.TokenDTO;
using Sales.Application.Interfaces;
using Sales.Infrastructure.Identity;

namespace Sales.Test.ControllersTests;

public class AuthControllerTest
{
    private readonly AuthController _authController;
    private readonly Fixture _fixture;
    private readonly ITokenService _mockTokenService;
    private readonly UserManager<ApplicationUser> _mockUserManager;
    private readonly RoleManager<IdentityRole> _mockRoleManager;
    private readonly IConfiguration _mockConfiguration;
    private readonly ILogger<AuthController> _mockLogger;
    private readonly IValidator<LoginModel> _mockLoginValidator;
    private readonly IValidator<RegisterModel> _mockRegisterValidator;

    public AuthControllerTest()
    {
        _mockTokenService = Substitute.For<ITokenService>();
        _mockUserManager = Substitute.For<UserManager<ApplicationUser>>(Substitute.For<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);
        _mockRoleManager = Substitute.For<RoleManager<IdentityRole>>(Substitute.For<IRoleStore<IdentityRole>>(), null, null, null, null);
        _mockConfiguration = Substitute.For<IConfiguration>();
        _mockLogger = Substitute.For<ILogger<AuthController>>();
        _mockLoginValidator = Substitute.For<IValidator<LoginModel>>();
        _mockRegisterValidator = Substitute.For<IValidator<RegisterModel>>();
        
        _fixture = new Fixture();

        _authController = new AuthController(
            _mockTokenService,
            _mockUserManager,
            _mockRoleManager,
            _mockConfiguration,
            _mockLogger,
            _mockLoginValidator,
            _mockRegisterValidator)
        {
            ControllerContext =
            {
                HttpContext = new DefaultHttpContext()
            }
        };
    }

    [Fact]
    public async Task Login_ReturnTokenAndRefreshTokenAndExpirationTime_WhenLoginModelIsValid()
    {
        // Arrange
        var login = _fixture.Create<LoginModel>();
        var validationResult = new ValidationResult()
        {
            // IsValid == True caso Erros.Count() seja 0;
            Errors = new List<ValidationFailure>()
        };
        var user = _fixture.Create<ApplicationUser>();
        var roles = _fixture.CreateMany<string>(3).ToList();
        var token = new JwtSecurityToken("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VyX2lkIjoxMjMsImV4cCI6MTcyODU2NjczMCwiaWF0IjoxNzI4NTYzMTMwfQ.ATQhO_Om9PCu64bgw5albOrpddyKf__vFQG_dd96Qu0");
        var refreshToken = _fixture.Create<string>();
        var expireTimeInSeconds = _fixture.Create<int>().ToString();

        _mockLoginValidator.ValidateAsync(Arg.Any<LoginModel>()).Returns(validationResult);
        _mockUserManager.FindByEmailAsync(Arg.Any<string>()).Returns(user);
        _mockUserManager.CheckPasswordAsync(Arg.Any<ApplicationUser>(), Arg.Any<string>()).Returns(true);
        _mockUserManager.GetRolesAsync(Arg.Any<ApplicationUser>()).Returns(roles);
        _mockTokenService.GenerateAccessToken(Arg.Any<List<Claim>>(), Arg.Any<IConfiguration>()).Returns(token);
        _mockTokenService.GenerateRefreshToken().Returns(refreshToken);
        _mockConfiguration[Arg.Any<string>()].Returns(expireTimeInSeconds);
        _mockUserManager.UpdateAsync(Arg.Any<ApplicationUser>()).Returns(Task.FromResult(new IdentityResult()));
        
        // Act
        var result = await _authController.Login(login);
        
        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<OkObjectResult>()
            .Which.StatusCode.Should().Be(200);
        
        await _mockLoginValidator.Received(1).ValidateAsync(Arg.Any<LoginModel>());
        await _mockUserManager.Received(1).FindByEmailAsync(Arg.Any<string>());
        await _mockUserManager.Received(1).CheckPasswordAsync(Arg.Any<ApplicationUser>(), Arg.Any<string>());
        await _mockUserManager.Received(1).GetRolesAsync(Arg.Any<ApplicationUser>());
        _mockTokenService.Received(1).GenerateAccessToken(Arg.Any<List<Claim>>(), Arg.Any<IConfiguration>());
        _mockTokenService.Received(1).GenerateRefreshToken();
        await _mockUserManager.Received(1).UpdateAsync(Arg.Any<ApplicationUser>());

    }

    [Fact]
    public async Task Login_Return400BadRequestResultWithErrorMessage_WhenLoginModelIsInvalid()
    {
        // Arrange
        var login = _fixture.Create<LoginModel>();
        var validationResult = new ValidationResult()
        {
            Errors = new List<ValidationFailure>()
            {
                // exemplo de erro na validacao
                new ValidationFailure("Email", "Email is required")
            }
        };
        _mockLoginValidator.ValidateAsync(Arg.Any<LoginModel>()).Returns(validationResult);
        
        // Act
        var result = await _authController.Login(login);
        
        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<BadRequestObjectResult>()
            .Which.StatusCode.Should().Be(400);
        
        await _mockLoginValidator.Received(1).ValidateAsync(Arg.Any<LoginModel>());
    }

    [Fact]
    public async Task Login_ReturnsUnauthorizedResult_WhenUserIsNotFound()
    {
        // Arrange
        var login = _fixture.Create<LoginModel>();
        var validationResult = new ValidationResult()
        {
            // Errors empty - IsValid true
            Errors = new List<ValidationFailure>()
        };
        ApplicationUser user = null;
        _mockLoginValidator.ValidateAsync(Arg.Any<LoginModel>()).Returns(validationResult);
        _mockUserManager.FindByEmailAsync(Arg.Any<string>()).Returns(user);
        
        // Act
        var result = await _authController.Login(login);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<UnauthorizedResult>();

        await _mockLoginValidator.Received(1).ValidateAsync(Arg.Any<LoginModel>());
        await _mockUserManager.Received(1).FindByEmailAsync(Arg.Any<string>());
    }
    
    [Fact]
    public async Task Login_ReturnsUnauthorizedResult_WhenUserPasswordIsInvalid()
    {
        // Arrange
        var login = _fixture.Create<LoginModel>();
        var validationResult = new ValidationResult()
        {
            // Errors empty - IsValid true
            Errors = new List<ValidationFailure>()
        };
        var user = _fixture.Create<ApplicationUser>();
        _mockLoginValidator.ValidateAsync(Arg.Any<LoginModel>()).Returns(validationResult);
        _mockUserManager.FindByEmailAsync(Arg.Any<string>()).Returns(user);
        _mockUserManager.CheckPasswordAsync(Arg.Any<ApplicationUser>(), Arg.Any<string>()).Returns(false);
        
        // Act
        var result = await _authController.Login(login);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<UnauthorizedResult>();
        
        await _mockLoginValidator.Received(1).ValidateAsync(Arg.Any<LoginModel>());
        await _mockUserManager.Received(1).FindByEmailAsync(Arg.Any<string>());
        await _mockUserManager.Received(1).CheckPasswordAsync(Arg.Any<ApplicationUser>(), Arg.Any<string>());

    }

    [Fact]
    public async Task Register_Returns201CreatedResult_WhenUserIsCreatedSuccessfully()
    {
        // Arrange
        var register = _fixture.Create<RegisterModel>();
        var validationResult = new ValidationResult()
        {
            Errors = new List<ValidationFailure>()
        };
        ApplicationUser user = null;
        var response = new Response { Status = "Success", Message = $"User created successfully!" };
        
        _mockRegisterValidator.ValidateAsync(Arg.Any<RegisterModel>()).Returns(validationResult);
        _mockUserManager.FindByEmailAsync(Arg.Any<string>()).Returns(user);
        _mockUserManager.CreateAsync(Arg.Any<ApplicationUser>(), Arg.Any<string>())
            .Returns(Task.FromResult(IdentityResult.Success));
        
        // Act
        var result = await _authController.Register(register);
        
        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<ObjectResult>()
            .Which.StatusCode.Should().Be(StatusCodes.Status201Created);
        result.Should().BeOfType<ObjectResult>()
            .Which.Value.Should().BeEquivalentTo(response);
        
        await _mockRegisterValidator.Received(1).ValidateAsync(Arg.Any<RegisterModel>());
        await _mockUserManager.Received(1).FindByEmailAsync(Arg.Any<string>());
        await _mockUserManager.Received(1).CreateAsync(Arg.Any<ApplicationUser>(), Arg.Any<string>());
    }
    
    [Fact]
    public async Task Register_Returns500CInternalServerErrorResult_WhenUserIsCreatedWithErros()
    {
        // Arrange
        var register = _fixture.Create<RegisterModel>();
        var validationResult = new ValidationResult()
        {
            Errors = new List<ValidationFailure>()
        };
        ApplicationUser user = null;
        var response = new Response { Status = "Error", Message = "User creation failed!" };
        
        _mockRegisterValidator.ValidateAsync(Arg.Any<RegisterModel>()).Returns(validationResult);
        _mockUserManager.FindByEmailAsync(Arg.Any<string>()).Returns(user);
        _mockUserManager.CreateAsync(Arg.Any<ApplicationUser>(), Arg.Any<string>())
            .Returns(Task.FromResult(IdentityResult.Failed()));
        
        // Act
        var result = await _authController.Register(register);
        
        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<ObjectResult>()
            .Which.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        result.Should().BeOfType<ObjectResult>()
            .Which.Value.Should().BeEquivalentTo(response);
        
        await _mockRegisterValidator.Received(1).ValidateAsync(Arg.Any<RegisterModel>());
        await _mockUserManager.Received(1).FindByEmailAsync(Arg.Any<string>());
        await _mockUserManager.Received(1).CreateAsync(Arg.Any<ApplicationUser>(), Arg.Any<string>());
    }

    [Fact]
    public async Task Register_Returns500InternalServerErrorResult_WhenUserWithTheSameEmailAlreadyExists()
    {
        // Arrange
        var register = _fixture.Create<RegisterModel>();
        var validationResult = new ValidationResult()
        {
            Errors = new List<ValidationFailure>()
        };
        var user = _fixture.Create<ApplicationUser>();
        var response  = new Response { Status = "Error", Message = $"User with the same Email already exists!" };
        _mockRegisterValidator.ValidateAsync(Arg.Any<RegisterModel>()).Returns(validationResult);
        _mockUserManager.FindByEmailAsync(Arg.Any<string>()).Returns(user);

        // Act
        var result = await _authController.Register(register);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<ObjectResult>()
            .Which.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        result.Should().BeOfType<ObjectResult>()
            .Which.Value.Should().BeEquivalentTo(response);
        
        await _mockRegisterValidator.Received(1).ValidateAsync(Arg.Any<RegisterModel>());
        await _mockUserManager.Received(1).FindByEmailAsync(Arg.Any<string>());
    }

    [Fact]
    public async Task Register_Returns400BadRequestResult_WhenRegistersModelIsInvalid()
    {
        // Arrange
        var register = _fixture.Create<RegisterModel>();
        var validationResult = new ValidationResult()
        {
            Errors = new List<ValidationFailure>()
            {
                new ValidationFailure("Email", "Email is required")
            }
        };
        _mockRegisterValidator.ValidateAsync(Arg.Any<RegisterModel>()).Returns(validationResult);

        // Act
        var result = await _authController.Register(register);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<BadRequestObjectResult>()
            .Which.StatusCode.Should().Be(400);
        result.Should().BeOfType<BadRequestObjectResult>()
            .Which.Value.Should().BeEquivalentTo(validationResult.Errors);
        
        await _mockRegisterValidator.Received(1).ValidateAsync(Arg.Any<RegisterModel>());

    }

    [Fact]
    public async Task RefreshToken_ReturnsObjectResultWithAccesTokenAndRefreshToken_WhenTokenAndRefreshTokenIsValid()
    {
        // Arrange
        var tokenModel = _fixture.Create<TokenModel>();
        var name = _fixture.Create<string>();
        var claimsIdentity = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Name, name)
        }, "Token");
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
        var user = _fixture.Create<ApplicationUser>();
        user.RefreshToken = tokenModel.RefreshToken;
        user.RefreshTokenExpiryTime = DateTime.Now.AddHours(10);
        var newToken = new JwtSecurityToken("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VyX2lkIjoxMjMsImV4cCI6MTcyODU2NjczMCwiaWF0IjoxNzI4NTYzMTMwfQ.ATQhO_Om9PCu64bgw5albOrpddyKf__vFQG_dd96Qu0");
        var newRefreshToken = _fixture.Create<string>();
        
        _mockTokenService.GetPrincipalFromExpiredToken(Arg.Any<string>(), _mockConfiguration).Returns(claimsPrincipal);
        _mockUserManager.FindByNameAsync(Arg.Any<string>()).Returns(user);
        _mockTokenService.GenerateAccessToken(Arg.Any<IEnumerable<Claim>>(), _mockConfiguration)
            .Returns(newToken);
        _mockTokenService.GenerateRefreshToken().Returns(newRefreshToken);
        _mockUserManager.UpdateAsync(Arg.Any<ApplicationUser>()).Returns(Task.FromResult(IdentityResult.Success));
        
        // Act
        var result = await _authController.RefreshToken(tokenModel);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<ObjectResult>();
        
        _mockTokenService.Received(1).GetPrincipalFromExpiredToken(Arg.Any<string>(), _mockConfiguration);
        await _mockUserManager.Received(1).FindByNameAsync(Arg.Any<string>());
        _mockTokenService.Received(1).GenerateAccessToken(Arg.Any<IEnumerable<Claim>>(), _mockConfiguration); 
        _mockTokenService.Received(1).GenerateRefreshToken();
        await _mockUserManager.Received(1).UpdateAsync(Arg.Any<ApplicationUser>());
    }
    
    [Fact]
    public async Task RefreshToken_ReturnsBadRequestResultWithErrorResponse_WhenUserIsNotFound()
    {
        // Arrange
        var tokenModel = _fixture.Create<TokenModel>();
        var name = _fixture.Create<string>();
        var claimsIdentity = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Name, name)
        }, "Token");
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
        ApplicationUser user = null;
        
        _mockTokenService.GetPrincipalFromExpiredToken(Arg.Any<string>(), _mockConfiguration).Returns(claimsPrincipal);
        _mockUserManager.FindByNameAsync(Arg.Any<string>()).Returns(user); 
        
        // Act
        var result = await _authController.RefreshToken(tokenModel);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<BadRequestObjectResult>()
            .Which.StatusCode.Should().Be(400);
        result.Should().BeOfType<BadRequestObjectResult>()
            .Which.Value.Should().Be("Invalid access/refresh token");
        
        _mockTokenService.Received(1).GetPrincipalFromExpiredToken(Arg.Any<string>(), _mockConfiguration);
        await _mockUserManager.Received(1).FindByNameAsync(Arg.Any<string>()); 
    }
    
    [Fact]
    public async Task RefreshToken_ReturnsBadRequestResultWithErrorResponse_WhenUserRefreshTokenDoesNotMatchTokenModel()
    {
        // Arrange
        var tokenModel = _fixture.Create<TokenModel>();
        var name = _fixture.Create<string>();
        var claimsIdentity = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Name, name)
        }, "Token");
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
        var user = _fixture.Create<ApplicationUser>();
        
        _mockTokenService.GetPrincipalFromExpiredToken(Arg.Any<string>(), _mockConfiguration).Returns(claimsPrincipal);
        _mockUserManager.FindByNameAsync(Arg.Any<string>()).Returns(user); 
        
        // Act
        var result = await _authController.RefreshToken(tokenModel);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<BadRequestObjectResult>()
            .Which.StatusCode.Should().Be(400);
        result.Should().BeOfType<BadRequestObjectResult>()
            .Which.Value.Should().Be("Invalid access/refresh token");
        
        _mockTokenService.Received(1).GetPrincipalFromExpiredToken(Arg.Any<string>(), _mockConfiguration);
        await _mockUserManager.Received(1).FindByNameAsync(Arg.Any<string>()); 
    }
    
    [Fact]
    public async Task RefreshToken_ReturnsBadRequestResultWithErrorResponse_WhenUserRefreshTokenExpireTimeLessThanOrEqualDateTimeUtcNow()
    {
        // Arrange
        var tokenModel = _fixture.Create<TokenModel>();
        var name = _fixture.Create<string>();
        var claimsIdentity = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Name, name)
        }, "Token");
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
        var user = _fixture.Create<ApplicationUser>();
        user.RefreshToken = tokenModel.RefreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow;
        
        _mockTokenService.GetPrincipalFromExpiredToken(Arg.Any<string>(), _mockConfiguration).Returns(claimsPrincipal);
        _mockUserManager.FindByNameAsync(Arg.Any<string>()).Returns(user); 
        
        // Act
        var result = await _authController.RefreshToken(tokenModel);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<BadRequestObjectResult>()
            .Which.StatusCode.Should().Be(400);
        result.Should().BeOfType<BadRequestObjectResult>()
            .Which.Value.Should().Be("Invalid access/refresh token");
        
        _mockTokenService.Received(1).GetPrincipalFromExpiredToken(Arg.Any<string>(), _mockConfiguration);
        await _mockUserManager.Received(1).FindByNameAsync(Arg.Any<string>()); 

    }

    [Fact]
    public async Task RefreshToken_ReturnsBadRequestResultWithErrorResponse_WhenPrincipalIsNull()
    {
        // Arrange
        var tokenModel = _fixture.Create<TokenModel>();
        ClaimsPrincipal principal = null;
        
        _mockTokenService.GetPrincipalFromExpiredToken(Arg.Any<string>(), _mockConfiguration).Returns(principal);
        
        // Act
        var result = await _authController.RefreshToken(tokenModel);
        
        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<BadRequestObjectResult>()
            .Which.StatusCode.Should().Be(400);
        result.Should().BeOfType<BadRequestObjectResult>()
            .Which.Value.Should().Be("Invalid access/refresh token");
        
        _mockTokenService.Received(1).GetPrincipalFromExpiredToken(Arg.Any<string>(), _mockConfiguration);
    }
    
    [Fact]
    public async Task RefreshToken_ReturnsBadRequestResultWithErrorResponse_WhenTokenModelIsNull()
    {
        // Arrange
        TokenModel tokenModel = null;

        // Act
        var result = await _authController.RefreshToken(tokenModel);
        
        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<BadRequestObjectResult>()
            .Which.StatusCode.Should().Be(400);
        result.Should().BeOfType<BadRequestObjectResult>()
            .Which.Value.Should().Be("Invalid client request");
    }

    [Fact]
    public async Task Revoke_ReturnsNoContentResult_WhenUserExists()
    {
        // Arrange
        var username = _fixture.Create<string>();
        var user = _fixture.Create<ApplicationUser>();
        
        _mockUserManager.FindByNameAsync(Arg.Any<string>()).Returns(user);
        _mockUserManager.UpdateAsync(Arg.Any<ApplicationUser>()).Returns(Task.FromResult(IdentityResult.Success));
        
        // Act
        var result = await _authController.Revoke(username);
        
        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<NoContentResult>();
        
        await _mockUserManager.Received(1).FindByNameAsync(Arg.Any<string>());
        await _mockUserManager.Received(1).UpdateAsync(Arg.Any<ApplicationUser>());
    }
    
    [Fact]
    public async Task Revoke_ReturnsBadRequestResultWithErrorMessage_WhenUserDoesNotExists()
    {
        // Arrange
        var username = _fixture.Create<string>();
        ApplicationUser user = null;
        
        _mockUserManager.FindByNameAsync(Arg.Any<string>()).Returns(user);
        
        // Act
        var result = await _authController.Revoke(username);
        
        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<BadRequestObjectResult>()
            .Which.StatusCode.Should().Be(400);
        result.Should().BeOfType<BadRequestObjectResult>()
            .Which.Value.Should().Be("Invalid client request");

        await _mockUserManager.Received(1).FindByNameAsync(Arg.Any<string>());
    }

    [Fact]
    public async Task CreateRole_Returns200ObjectResultWithSuccessMessage_WhenRoleCreateSuccessfully()
    {
        // Arrange
        var roleName = _fixture.Create<string>();
        var response = new Response { Status = "Success", Message = $"Role {roleName} added successfully" };
        
        _mockRoleManager.RoleExistsAsync(Arg.Any<string>()).Returns(false);
        _mockRoleManager.CreateAsync(Arg.Any<IdentityRole>()).Returns(Task.FromResult(IdentityResult.Success));

        // Act
        var result = await _authController.CreateRole(roleName);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<ObjectResult>()
            .Which.StatusCode.Should().Be(200);
        result.Should().BeOfType<ObjectResult>()
            .Which.Value.Should().BeEquivalentTo(response);

        await _mockRoleManager.Received(1).RoleExistsAsync(Arg.Any<string>());
        await _mockRoleManager.Received(1).CreateAsync(Arg.Any<IdentityRole>());

    }
    
    [Fact]
    public async Task CreateRole_Returns400ObjectResultWithErrorMessage_WhenRoleCreateWithErrors()
    {
        // Arrange
        var roleName = _fixture.Create<string>();
        var response = new Response { Status = "Error", Message = $"Issue adding the new {roleName} role" };
        
        _mockRoleManager.RoleExistsAsync(Arg.Any<string>()).Returns(false);
        _mockRoleManager.CreateAsync(Arg.Any<IdentityRole>()).Returns(Task.FromResult(IdentityResult.Failed()));

        // Act
        var result = await _authController.CreateRole(roleName);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<ObjectResult>()
            .Which.StatusCode.Should().Be(400);
        result.Should().BeOfType<ObjectResult>()
            .Which.Value.Should().BeEquivalentTo(response);
        
        await _mockRoleManager.Received(1).RoleExistsAsync(Arg.Any<string>());
        await _mockRoleManager.Received(1).CreateAsync(Arg.Any<IdentityRole>());

    }
    
    [Fact]
    public async Task CreateRole_Returns400ObjectResultWithSuccessMessage_WhenRoleAlreadyExists()
    {
        // Arrange
        var roleName = _fixture.Create<string>();
        var response = new Response { Status = "Error", Message = "Role already exists" };
        
        _mockRoleManager.RoleExistsAsync(Arg.Any<string>()).Returns(true);
        
        // Act
        var result = await _authController.CreateRole(roleName);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<ObjectResult>()
            .Which.StatusCode.Should().Be(400);
        result.Should().BeOfType<ObjectResult>()
            .Which.Value.Should().BeEquivalentTo(response);
        
        await _mockRoleManager.Received(1).RoleExistsAsync(Arg.Any<string>());
    }

    [Fact]
    public async Task AddUserToRole_Returns200ObjectResultWithSuccessMessage_WhenRoleAddedToUserSuccessfully()
    {
        // Arrange
        var roleName = _fixture.Create<string>();
        var email = _fixture.Create<string>();
        var user = _fixture.Create<ApplicationUser>();
        var response = new Response { Status = "Success", Message = $"User {user.Email} added to the {roleName} role" };

        _mockUserManager.FindByEmailAsync(Arg.Any<string>()).Returns(user);
        _mockUserManager.AddToRoleAsync(Arg.Any<ApplicationUser>(), Arg.Any<string>())
            .Returns(Task.FromResult(IdentityResult.Success));
        
        // Act
        var result = await _authController.AddUserToRole(email, roleName);
        
        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<ObjectResult>()
            .Which.StatusCode.Should().Be(200);
        result.Should().BeOfType<ObjectResult>()
            .Which.Value.Should().BeEquivalentTo(response);
        
        await _mockUserManager.Received(1).FindByEmailAsync(Arg.Any<string>());
        await _mockUserManager.Received(1).AddToRoleAsync(Arg.Any<ApplicationUser>(), Arg.Any<string>());
    }
    
    [Fact]
    public async Task AddUserToRole_Returns400ObjectResultWithErrorMessage_WhenRoleAddedToUserWithErrors()
    {
        // Arrange
        var roleName = _fixture.Create<string>();
        var email = _fixture.Create<string>();
        var user = _fixture.Create<ApplicationUser>();
        var response = new Response { Status = "Error", Message = $"Error: Unable to add user {user.Email} to the {roleName} role" };

        _mockUserManager.FindByEmailAsync(Arg.Any<string>()).Returns(user);
        _mockUserManager.AddToRoleAsync(Arg.Any<ApplicationUser>(), Arg.Any<string>())
            .Returns(Task.FromResult(IdentityResult.Failed()));
        
        // Act
        var result = await _authController.AddUserToRole(email, roleName);
        
        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<ObjectResult>()
            .Which.StatusCode.Should().Be(400);
        result.Should().BeOfType<ObjectResult>()
            .Which.Value.Should().BeEquivalentTo(response);
        
        await _mockUserManager.Received(1).FindByEmailAsync(Arg.Any<string>());
        await _mockUserManager.Received(1).AddToRoleAsync(Arg.Any<ApplicationUser>(), Arg.Any<string>());
    }
    
    [Fact]
    public async Task AddUserToRole_Returns200BadRequestResultWithSuccessMessage_WhenUserDoesNotExists()
    {
        // Arrange
        var roleName = _fixture.Create<string>();
        var email = _fixture.Create<string>();
        ApplicationUser user = null;
        var response = new {error = "Unable to find user"};

        _mockUserManager.FindByEmailAsync(Arg.Any<string>()).Returns(user);
        
        // Act
        var result = await _authController.AddUserToRole(email, roleName);
        
        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<BadRequestObjectResult>()
            .Which.StatusCode.Should().Be(400);
        result.Should().BeOfType<BadRequestObjectResult>()
            .Which.Value.Should().BeEquivalentTo(response);
        
        await _mockUserManager.Received(1).FindByEmailAsync(Arg.Any<string>());
    }
}