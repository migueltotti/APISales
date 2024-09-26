using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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

    public AuthController(ITokenService tokenService, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration config, ILogger<AuthController> logger)
    {
        _tokenService = tokenService;
        _userManager = userManager;
        _roleManager = roleManager;
        _config = config;
        _logger = logger;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
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
        
        return Unauthorized();
    }

    [HttpPost("refreshToken")]
    public async Task<IActionResult> RefreshToken(TokenModel tokenModel)
    {
        if (tokenModel is null)
            return BadRequest("Invalid client request");
        
        string? accessToken =  tokenModel.AccessToken
                                ?? throw new ArgumentNullException(nameof(tokenModel));
        
        string? refreshToken =  tokenModel.RefreshToken
                               ?? throw new ArgumentNullException(nameof(tokenModel));
        
        var principal = _tokenService.GetPrincipalFromExpiredToken(accessToken!, _config);

        if (principal is null)
            return BadRequest("Invalid access/refresh token");
        
        string username = principal.Identity.Name;
        
        var user = await _userManager.FindByNameAsync(username!);

        if (user is null || user.RefreshToken != refreshToken
                         || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
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
    public async Task<IActionResult> Revoke(string username)
    {
        var user = await _userManager.FindByNameAsync(username);
        
        if(user is null)
            return BadRequest("Invalid client request");
        
        user.RefreshToken = null;
        
        await _userManager.UpdateAsync(user);
        
        return NoContent();
    }

    [HttpPost("createRole")]
    public async Task<IActionResult> CreateRole(string roleName)
    {
        var roleExists = await _roleManager.RoleExistsAsync(roleName);

        if (!roleExists)
        {
            var roleResult = await _roleManager.CreateAsync(new IdentityRole(roleName));

            if (roleResult.Succeeded)
            {
                _logger.LogInformation(1, "Role Added");
                return StatusCode(StatusCodes.Status200OK,
                    new Response { Status = "Success", Message = $"Role {roleName} added successfully" });
            }
            else
            {
                _logger.LogInformation(2, "Error");
                return StatusCode(StatusCodes.Status400BadRequest,
                    new Response { Status = "Error", Message = $"Issue adding the new {roleName} role" });
            }
        }
        
        _logger.LogInformation(3, "Error");
        return StatusCode(StatusCodes.Status400BadRequest,
            new Response { Status = "Error", Message = "Role already exists" });
    }

    [HttpPost("addUserToRole")]
    public async Task<IActionResult> AddUserToRole(string email, string roleName)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user is not null)
        {
            var result = await _userManager.AddToRoleAsync(user, roleName);

            if (result.Succeeded)
            {
                _logger.LogInformation(1, $"User {user.Email} added to the {roleName} role");
                return StatusCode(StatusCodes.Status200OK,
                    new Response { Status = "Success", Message = $"User {user.Email} added to the {roleName} role" });
            }
            else
            {
                _logger.LogInformation(2, $"Error: Unable to add user {user.Email} to the {roleName} role");
                return StatusCode(StatusCodes.Status400BadRequest,
                    new Response { Status = "Error", Message = $"Error: Unable to add user {user.Email} to the {roleName} role" });
            }
        }
        
        return BadRequest(new {error = "Unable to find user"});
    }
}