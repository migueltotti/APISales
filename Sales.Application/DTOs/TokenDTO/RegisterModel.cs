namespace Sales.Application.DTOs.TokenDTO;

public class RegisterModel
{
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }

    public string GenerateUserName()
    {
        return Username.Replace(" ", "") 
               + "-"
               + Email[1]
               + Email[2]
               + Email[0]
               + Email[8]
               + Email[2]
               + Email[2];
    }
}