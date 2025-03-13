using System.Linq.Expressions;
using System.Text;
using Sales.Application.DTOs.UserDTO;
using Sales.Application.Parameters;
using Sales.Application.Parameters.ModelsParameters;
using Sales.Application.ResultPattern;
using Sales.Domain.Models;
using X.PagedList;

namespace Sales.Application.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserDTOOutput>> GetAllUsers();
    Task<IPagedList<UserDTOOutput>> GetAllUsers(QueryStringParameters parameters);
    Task<IPagedList<UserDTOOutput>> GetUsersWithFilter(string filter, UserParameters parameters);
    Task<IPagedList<UserDTOOutput>> GetUsersByPoints(UserParameters parameters);
    Task<IPagedList<UserDTOOutput>> GetUsersByAffiliation(UserParameters parameters);
    Task<IPagedList<UserDTOOutput>> GetUsersByOrdersNotCompleted(UserParameters parameters);
    Task<Result<UserDTOOutput>> GetUserById(int id);
    Task<Result<UserDTOOutput>> GetUserByEmail(string email);
    Task<Result<UserDTOOutput>> GetUserBy(Expression<Func<User, bool>> expression);
    Task<Result<UserDTOOutput>> CreateUser(UserDTOInput user);
    Task<Result<(UserDTOOutput, Dictionary<string, string>)>> UpdateUser(UserDTOInput user, int id);
    Task<Result<UserDTOOutput>> UpdateUserPassword(int userId, string oldPassword, string newPassword);
    Task<Result<UserDTOOutput>> UpdateUserRole(int userId, string role);
    Task<Result<UserDTOOutput>> DeleteUser(int? id);
    
}