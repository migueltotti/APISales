using System.Linq.Expressions;
using System.Text;
using Sales.Application.DTOs.UserDTO;
using Sales.Application.Parameters.ModelsParameters;
using Sales.Application.ResultPattern;
using Sales.Domain.Models;
using X.PagedList;

namespace Sales.Application.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserDTOOutput>> GetAllUsers();
    Task<IPagedList<UserDTOOutput>> GetAllUsers(UserParameters parameters);
    Task<Result<UserDTOOutput>> GetUserBy(Expression<Func<User, bool>> expression);
    Task<Result<UserDTOOutput>> CreateUser(UserDTOInput user);
    Task<Result<UserDTOOutput>> UpdateUser(UserDTOInput user, int id);
    Task<Result<UserDTOOutput>> DeleteUser(int? id);
    //Task UserLogin();
    // Pensar sobre incluir esse serviço no UserService ou deixar apenas para o TokenService e o controlador de autenticacao
}