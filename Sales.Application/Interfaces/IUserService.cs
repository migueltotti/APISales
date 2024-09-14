using System.Linq.Expressions;
using Sales.Application.DTOs.UserDTO;
using Sales.Application.ResultPattern;
using Sales.Domain.Models;

namespace Sales.Application.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserDTOOutput>> GetAllUsers();
    Task<Result<UserDTOOutput>> GetUserBy(Expression<Func<User, bool>> expression);
    Task<Result<UserDTOOutput>> CreateUser(UserDTOInput user);
    Task<Result<UserDTOOutput>> UpdateUser(UserDTOInput user, int id);
    Task<Result<UserDTOOutput>> DeleteUser(int? id);
    //Task UserLogin();
    // Pensar sobre incluir esse serviço no UserService ou deixar apenas para o TokenService e o controlador de autenticacao
}