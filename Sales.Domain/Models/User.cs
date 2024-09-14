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
    public DateTime DateBirth { get; private set; }
    
    public Role Role { get; private set; }

    // User 1 : n Order
    public ICollection<Order>? Orders { get; private set; }

    public User(int userId, string? name, string? email, string? password, string? cpf, DateTime dateBirth, Role role)
    {
        UserId = userId;
        Name = name;
        Email = email;
        Password = password;
        Cpf = cpf;
        DateBirth = dateBirth;
        Role = role;
    }

    public User(string? name, string? email, string? password, string? cpf, DateTime dateBirth, Role role)
    {
        Name = name;
        Email = email;
        Password = password;
        Cpf = cpf;
        DateBirth = dateBirth;
        Role = role;
    }
}