using System.Linq.Expressions;
using Sales.Application.DTOs.UserDTO;
using Sales.Application.Interfaces;
using Sales.Domain.Models;

namespace Sales.Application.Services;

public class UserService : IUserService
{
    public Task<IEnumerable<UserDTOOutput>> GetAllUsers()
    {
        throw new NotImplementedException();
    }

    public Task<UserDTOOutput> GetUserBy(Expression<Func<User, bool>> expression)
    {
        throw new NotImplementedException();
    }

    public Task CreateUser(UserDTOInput user)
    {
        throw new NotImplementedException();
    }

    public Task UpdateUser(UserDTOInput user)
    {
        throw new NotImplementedException();
    }

    public Task DeleteUser(int? id)
    {
        throw new NotImplementedException();
    }
}