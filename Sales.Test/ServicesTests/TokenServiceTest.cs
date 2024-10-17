using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using Sales.Application.Services;

namespace Sales.Test.ServicesTests;

public class TokenServiceTest
{
    private readonly TokenService _tokenService;
    private readonly IConfiguration _mockConfiguration;
    private readonly JwtSecurityTokenHandler _mockHandler;

    public TokenServiceTest()
    {
        _mockConfiguration = Substitute.For<IConfiguration>();
        _mockHandler = Substitute.For<JwtSecurityTokenHandler>();
        
        _tokenService = new TokenService();
    }

    [Fact]
    public void GenerateAccessToken_ShouldReturnAccessToken()
    {
        // Arrange
        var key = Guid.NewGuid().ToString();
        var tokenValidityTime = 10d;
        var validAudience = "ValidAudience";
        var validIssuer = "ValidIssuer";
        
        _mockConfiguration.GetSection("JWT")
            .GetValue<string>("SecretKey")
            .Returns(key);
        _mockConfiguration.GetSection("JWT")
            .GetValue<double>("TokenValidityInMinutes")
            .Returns(tokenValidityTime);
        _mockConfiguration.GetSection("JWT")
            .GetValue<string>("ValidAudience")
            .Returns(validAudience);
        _mockConfiguration.GetSection("JWT")
            .GetValue<string>("ValidIssuer")
            .Returns(validIssuer);
        
        // Act
        
        // Assert
    }
}