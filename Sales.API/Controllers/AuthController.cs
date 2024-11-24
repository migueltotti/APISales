using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Sales.Application.DTOs.TokenDTO;
using Sales.Application.Interfaces;
using Sales.Infrastructure.Identity;

namespace Sales.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly ITokenService _tokenService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _config;
    private readonly ILogger<AuthController> _logger;
    private readonly IValidator<LoginModel> _loginValidator;
    private readonly IValidator<RegisterModel> _registerValidator;

    public AuthController(ITokenService tokenService, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration config, ILogger<AuthController> logger, IValidator<LoginModel> loginValidator, IValidator<RegisterModel> registerValidator)
    {
        _tokenService = tokenService;
        _userManager = userManager;
        _roleManager = roleManager;
        _config = config;
        _logger = logger;
        _registerValidator = registerValidator;
        _loginValidator = loginValidator;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        var result = await _loginValidator.ValidateAsync(model);

        if (!result.IsValid)
        {
            _logger.LogWarning(
                "Request failed {@Error}, {@RequestName}, {@DateTime}",
                new { Status = "Error", Message = "Login model invalid input" },
                nameof(_loginValidator.ValidateAsync),
                DateTime.Now
            );
            return BadRequest(result.Errors);
        }
            
        
        var user = await _userManager.FindByEmailAsync(model.Email!);

        if (user is not null && await _userManager.CheckPasswordAsync(user, model.Password!))
        {
            var userRoles = await _userManager.GetRolesAsync(user);

            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            
            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }
            
            var token = _tokenService.GenerateAccessToken(authClaims, _config);
            
            var refreshToken = _tokenService.GenerateRefreshToken();
            
            _ = int.TryParse(_config["JWT:RefreshTokenValidityInMinutes"],
                                            out int refreshTokenValidityInMinutes);
            
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(refreshTokenValidityInMinutes);
            
            await _userManager.UpdateAsync(user);

            return Ok(new
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                RefreshToken = refreshToken,
                Expiration = token.ValidTo
            });
        }
        
        _logger.LogWarning(
            "Request failed {@Error}, {@RequestName}, {@DateTime}",
            new { Status = "Error", Message = "Unauthorized access token" },
            nameof(_userManager.FindByEmailAsync),
            DateTime.Now
        );
        return Unauthorized();
    }

    [HttpPost("register")]
    [Authorize("AdminOnly")]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        var result = await _registerValidator.ValidateAsync(model);

        if (!result.IsValid)
        {
            _logger.LogWarning(
                "Request failed {@Error}, {@RequestName}, {@DateTime}",
                new { Status = "Error", Message = "Register model invalid input" },
                nameof(_registerValidator.ValidateAsync),
                DateTime.Now
            );
            return BadRequest(result.Errors);
        }
        
        var userExists = await _userManager.FindByEmailAsync(model.Email!);

        if (userExists is not null)
        {
            _logger.LogWarning(
                "Request failed {@Error}, {@RequestName}, {@DateTime}",
                new { Status = "Error", Message = "User with the same Email already exists!" },
                nameof(_userManager.FindByEmailAsync),
                DateTime.Now
            );
            return StatusCode(StatusCodes.Status500InternalServerError,
                new Response { Status = "Error", Message = $"User with the same Email already exists!" });
        }

        ApplicationUser user = new()
        {
            Email = model.Email,
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = model.GenerateUserName()
        };

        var resultUser = await _userManager.CreateAsync(user, model.Password);

        if (!resultUser.Succeeded)
        {
            _logger.LogWarning(
                "Request failed {@Error}, {@RequestName}, {@DateTime}",
                new { Status = "Error", Message = "User creation failed!" },
                nameof(_userManager.CreateAsync),
                DateTime.Now
            );
            return StatusCode((int)HttpStatusCode.InternalServerError,
                new Response { Status = "Error", Message = "User creation failed!" });
        }
        
        return StatusCode(StatusCodes.Status201Created,
            new Response { Status = "Success", Message = $"User created successfully!" });
    }

    [HttpPost("refreshToken")]
    public async Task<IActionResult> RefreshToken(TokenModel tokenModel)
    {
        if (tokenModel is null)
        {
            _logger.LogWarning(
                "Request failed {@Error}, {@RequestName}, {@DateTime}",
                new { Status = "Error", Message = "Token model is null" },
                nameof(RefreshToken),
                DateTime.Now
            );
            return BadRequest("Invalid client request");
        }
            
        
        string? accessToken =  tokenModel.AccessToken
                                ?? throw new ArgumentNullException(nameof(tokenModel));
        
        string? refreshToken =  tokenModel.RefreshToken
                               ?? throw new ArgumentNullException(nameof(tokenModel));
        
        var principal = _tokenService.GetPrincipalFromExpiredToken(accessToken!, _config);

        if (principal is null)
        {
            _logger.LogWarning(
                "Request failed {@Error}, {@RequestName}, {@DateTime}",
                new { Status = "Error", Message = "Invalid access/refresh token" },
                nameof(_tokenService.GetPrincipalFromExpiredToken),
                DateTime.Now
            );
            return BadRequest("Invalid access/refresh token");
        }
        
        string username = principal.Identity.Name.Replace(" ", "");
        
        var user = await _userManager.FindByNameAsync(username!);

        if (user is null || user.RefreshToken != refreshToken
                         || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            _logger.LogWarning(
                "Request failed {@Error}, {@RequestName}, {@DateTime}",
                new { Status = "Error", Message = "Invalid access/refresh token" },
                nameof(_userManager.FindByNameAsync),
                DateTime.Now
            );
            
            return BadRequest("Invalid access/refresh token");
        }
        
        var newAccessToken = _tokenService.GenerateAccessToken(principal.Claims.ToList(), _config);
        
        var newRefreshToken = _tokenService.GenerateRefreshToken();
        
        user.RefreshToken = newRefreshToken;
        await _userManager.UpdateAsync(user);

        return new ObjectResult(new
        {
            accessToken = new JwtSecurityTokenHandler().WriteToken(newAccessToken),
            refreshToken = newRefreshToken
        });
    }

    [HttpPost("revoke/{username}")]
    [Authorize()]
    public async Task<IActionResult> Revoke(string username)
    {
        var user = await _userManager.FindByNameAsync(username.Replace(" ", ""));

        if (user is null)
        {
            _logger.LogWarning(
                "Request failed {@Error}, {@RequestName}, {@DateTime}",
                new { Status = "Error", Message = "Invalid client request" },
                nameof(_userManager.FindByNameAsync),
                DateTime.Now
            );
            
            return BadRequest("Invalid client request");
        }
        
        user.RefreshToken = null;
        
        await _userManager.UpdateAsync(user);
        
        return NoContent();
    }

    [HttpPost("createRole")]
    [Authorize("AdminOnly")]
    public async Task<IActionResult> CreateRole(string roleName)
    {
        var roleExists = await _roleManager.RoleExistsAsync(roleName);

        if (!roleExists)
        {
            var roleResult = await _roleManager.CreateAsync(new IdentityRole(roleName));

            if (roleResult.Succeeded)
            {
                return StatusCode(StatusCodes.Status200OK,
                    new Response { Status = "Success", Message = $"Role {roleName} added successfully" });
            }
            else
            {
                _logger.LogWarning(
                    "Request failed {@Error}, {@RequestName}, {@DateTime}",
                    new { Status = "Error", Message = "Issue adding the new role" },
                    nameof(_roleManager.CreateAsync),
                    DateTime.Now
                );
                return StatusCode(StatusCodes.Status400BadRequest,
                    new Response { Status = "Error", Message = $"Issue adding the new {roleName} role" });
            }
        }
        
        _logger.LogWarning(
            "Request failed {@Error}, {@RequestName}, {@DateTime}",
            new { Status = "Error", Message = "Role already exists" },
            nameof(_roleManager.RoleExistsAsync),
            DateTime.Now
        );
        return StatusCode(StatusCodes.Status400BadRequest,
            new Response { Status = "Error", Message = "Role already exists" });
    }

    [HttpPost("addUserToRole")]
    [Authorize()]
    public async Task<IActionResult> AddUserToRole(string email, string roleName)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user is not null)
        {
            var result = await _userManager.AddToRoleAsync(user, roleName);

            if (result.Succeeded)
            {
                return StatusCode(StatusCodes.Status200OK,
                    new Response { Status = "Success", Message = $"User {user.Email} added to the {roleName} role" });
            }
            else
            {
                _logger.LogWarning(
                    "Request failed {@Error}, {@RequestName}, {@DateTime}",
                    new { Status = "Error", Message = "Error: Unable to add user to the role" },
                    nameof(_userManager.AddToRoleAsync),
                    DateTime.Now
                );
                return StatusCode(StatusCodes.Status400BadRequest,
                    new Response { Status = "Error", Message = $"Error: Unable to add user {user.Email} to the {roleName} role" });
            }
        }
        
        _logger.LogWarning(
            "Request failed {@Error}, {@RequestName}, {@DateTime}",
            new { Status = "Error", Message = "Unable to find user" },
            nameof(_userManager.FindByEmailAsync),
            DateTime.Now
        );
        return BadRequest(new {error = "Unable to find user"});
    }
}