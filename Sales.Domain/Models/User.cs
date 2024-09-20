using System.Collections.ObjectModel;
using Sales.Domain.Models.Enums;

namespace Sales.Domain.Models;

public sealed class User
{
    public int UserId { get; private set; }
    public string? Name { get; private set; }
    public string? Email { get; private set; }
    public string? Password { get; private set; }
    public string? Cpf { get; private set; }
    public decimal Points { get; private set; }
    public DateTime DateBirth { get; private set; }
    
    public Role Role { get; private set; }

    // User 1 : n Order
    public ICollection<Order>? Orders { get; private set; }
    
    // User n : 1 Affiliate
    public int AffiliateId { get; private set; }    
    public Affiliate? Affiliate { get; private set; }

    public User(int userId, string? name, string? email, string? password, string? cpf, decimal points, DateTime dateBirth, Role role, int affiliateId)
    {
        UserId = userId;
        Name = name;
        Email = email;
        Password = password;
        Cpf = cpf;
        Points = points;
        DateBirth = dateBirth;
        Role = role;
        AffiliateId = affiliateId;
    }

    public User(string? name, string? email, string? password, string? cpf, decimal points, DateTime dateBirth, Role role, int affiliateId)
    {
        Name = name;
        Email = email;
        Password = password;
        Cpf = cpf;
        Points = points;
        DateBirth = dateBirth;
        Role = role;
        AffiliateId = affiliateId;
    }
}