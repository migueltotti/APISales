using System.Linq.Expressions;
using Sales.Application.DTOs.UserDTO;
using Sales.Domain.Models;

namespace Sales.Application.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserDTOOutput>> GetAllUsers();
    Task<UserDTOOutput> GetUserBy(Expression<Func<User, bool>> expression);
    Task CreateUser(UserDTOInput user);
    Task UpdateUser(UserDTOInput user);
    Task DeleteUser(int? id);
    //Task UserLogin();
    // Pensar sobre incluir esse serviço no UserService ou deixar apenas para o TokenService e o controlador de autenticacao
}